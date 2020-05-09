using FileManagementCore.Kernel.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace FileManagementCoreCLI
{
    class Program
    {
        //14743 sector manage 7.38gb
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = 12)]
        public struct aa
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] FAT_ENTRY;
        }
        static void Main(string[] args)
        {
            //BootSector boot = new BootSector();
            //boot.InitBootSector();
           

        }
    }
}
