using System;
using System.Collections.Generic;

namespace StriveUtil
{
    class Program
    {

        private static readonly Tuple<int, int[]> bloodGauge = Tuple.Create(0x4C6E4F8, new int[] { 0x58, 0x4D8, 0x288, 0x178, 0x0, 0x6DC });
        private static readonly Tuple<int, int[]> wallOnP2 = Tuple.Create(0x4C6E4F8, new int[] { 0x110, 0x8, 0xCFA0 });
        private static readonly Tuple<int, int[]> riscOnP2 = Tuple.Create(0x4C6E4F8, new int[] { 0x110, 0x8, 0xC424 });

        static void Main(string[] args)
        {
            var strive = MemoryLib.MemoryHandler.OpenProcessByName("GGST-Win64-Shipping", true);
            IntPtr wallP2Ptr = strive.GetAddressWithOffsets(wallOnP2.Item1, wallOnP2.Item2);
            IntPtr riscP2Ptr = strive.GetAddressWithOffsets(riscOnP2.Item1, riscOnP2.Item2);

            int wallP2;
            int riscP2;
            int newRisc;

            Console.WriteLine("Process found and tool running.");
            // Console.WriteLine("P2WALL: {0:X}, P2RISC: {1:X}", wallP2Ptr, riscP2Ptr);

            while (true)
            {
                wallP2 = strive.ReadMemory<int>(wallP2Ptr);
                riscP2 = strive.ReadMemory<int>(riscP2Ptr);
                newRisc = Math.Min((int)Math.Round(wallP2 * 4.26), 12800);
                strive.WriteMemory<int>(riscP2Ptr, newRisc);
            }
        }
    }
}
