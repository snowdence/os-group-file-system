using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementCore.Kernel.Structure
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = 4096)]
    public struct SCluster
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
        public byte[] data;
       
    }
    public class Cluster
    {
    }
}
