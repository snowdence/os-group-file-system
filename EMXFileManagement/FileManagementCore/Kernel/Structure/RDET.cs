using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementCore.Kernel.Structure
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = 64)]
    public struct SRDETEntry : ICloneable
    {
        public byte FLAG; // 02

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 23)]
        public string FILE_NAME;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string FILE_EXT;

        public byte FILE_ATTRIBUTE; //~ ~02


        public byte REVERSED;

        public byte DMSTIME; //not include 


        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] CREATED_DATETIME ;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] LAST_ACCESS_DATE;


        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        //public byte[] FIRST_CLUSTER_HIGH_WORD;


        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] MODIFIED_DATETIME;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] FIRST_CLUSTER_LOW_WORD;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] FILE_SIZE;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string PASSWORD;
        

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
    //14743 sector manage 7.38gb
    //1 block 512byte
    //128 entry
    //64 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = 4096)]
    public struct SRDET
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public SRDETEntry[] entries; 
    }

    public class RDET
    {
        SRDET _srdet;
        public RDET() {
        }
        public RDET(SRDET rDET)
        {
            this._srdet = rDET;
        }

        public int GetEmptyEntry()
        {
            for (int i = 0; i < 128; i++)
            {
                if (this._srdet.entries[i].FLAG == 0x00)
                {
                    return i;
                }
            }
            return -1;
        }
        //return clone 
        public SRDETEntry ReadEntryPos(int pos)
        {
            return this._srdet.entries[pos];
        }

        
    }
}
