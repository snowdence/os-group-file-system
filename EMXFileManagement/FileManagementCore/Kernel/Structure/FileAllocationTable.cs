using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementCore.Kernel.Structure
{

    //14743 sector manage 7.38gb
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = 14743*512)]
    public struct SFileAllocationTable {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1887104)]
        public UInt32[] FAT_ENTRY;   
    }

    public class FileAllocationTable
    {
        SFileAllocationTable _fat;
        public FileAllocationTable(SFileAllocationTable fat)
        {
            this._fat = fat;
        }

        byte[] getBytes(SFileAllocationTable str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
        //test this function
        int ReadFatEntry(int n)
        {
            int size = Marshal.SizeOf(this._fat);

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this._fat, ptr, true);
            byte[] arr = new byte[4];

            Marshal.Copy(ptr + n * 4, arr,0  , 4);
            Marshal.FreeHGlobal(ptr);
            return BitConverter.ToInt32(arr, 0); 
        }
        int SetFatEntry(byte[] arr, int n)
        {
            Array.Copy(arr, 0, this._fat.FAT_ENTRY, n * 4, 4);
            return 0;
        }
        
    }
}
