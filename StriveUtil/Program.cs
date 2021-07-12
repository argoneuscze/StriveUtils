using System;

namespace StriveUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = MemoryLib.MemoryHandler.OpenProcessByName("GGST-Win64-Shipping", true);
            IntPtr bloodGauge = test.GetAddressWithOffsets(0x4C6E4F8, new int[] { 0x58, 0x4D8, 0x288, 0x178, 0x0, 0x6DC });
            // var value = test.ReadMemory<int>(bloodGauge);
            // Console.Out.WriteLine(value);
            test.WriteMemory<int>(bloodGauge, 5000);
        }
    }
}
