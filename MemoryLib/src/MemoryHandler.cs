using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MemoryLib
{
    public class MemoryHandler
    {
        private Process CurrentProcess { get; }
        private IntPtr ProcHandle { get; }
        private bool Is64Bit { get; }


        private MemoryHandler(Process process, IntPtr procHandle, bool is64Bit)
        {
            CurrentProcess = process;
            ProcHandle = procHandle;
            Is64Bit = is64Bit;
        }

        public static MemoryHandler OpenProcessByName(string name, bool is64Bit)
        {
            Process proc = MemoryUtil.GetProcessByName(name);
            IntPtr procHandle = MemoryUtil.OpenProcess(proc, MemoryUtil.defaultVirtualFlags);

            return new MemoryHandler(proc, procHandle, is64Bit);
        }

        public T ReadMemory<T>(IntPtr address)
        {
            IntPtr bytesRead;
            int typeSize = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[typeSize];

            MemoryUtil.ReadProcessMemory(ProcHandle, address, buffer, typeSize, out bytesRead);

            GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr bufferPtr = bufferHandle.AddrOfPinnedObject();
            T value = Marshal.PtrToStructure<T>(bufferPtr);
            bufferHandle.Free();

            return value;
        }

        public IntPtr GetAddressWithOffsets(int baseOffset, params int[] offsets)
        {
            IntPtr basePtr = GetBaseAddress();
            IntPtr curPtr = IntPtr.Add(basePtr, baseOffset);
            foreach (var offset in offsets)
            {
                curPtr = ResolvePointer(curPtr);
                curPtr = IntPtr.Add(curPtr, offset);
            }
            return curPtr;
        }

        private IntPtr GetBaseAddress()
        {
            return CurrentProcess.MainModule.BaseAddress;
        }

        private IntPtr ResolvePointer(IntPtr address)
        {
            IntPtr ptr;
            if (Is64Bit)
                ptr = new IntPtr(ReadMemory<Int64>(address));
            else
                ptr = new IntPtr(ReadMemory<Int32>(address));
            return ptr;
        }

        ~MemoryHandler()
        {
            if (ProcHandle != IntPtr.Zero)
            {
                MemoryUtil.CloseHandle(ProcHandle);
            }
        }
    }
}