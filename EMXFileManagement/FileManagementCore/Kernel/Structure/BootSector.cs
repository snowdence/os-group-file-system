using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace FileManagementCore.Kernel.Structure
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = 53)]
    public struct SBiosParamBootSector
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BYTE_PER_SECTOR; // 02 00

        public byte SECTORS_PER_CLUSTER;  // 08

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] REVERSE_SECTORS; //sector du phong 0C D2

        public byte NUM_OF_FAT; // 02

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public byte[] NO_USE; // NO use 

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]

        public byte[] NUMBER_OF_HEAD; // ENTRY 00 FF
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]

        public byte[] HIDDEN_SECTOR; // ENTRY 00 00 08 00
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]

        public byte[] TOTAL_SECTOR;//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]

        public byte[] SECTOR_PER_FAT; //00 00 39 97 => 14,743
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]

        public byte[] VERSION;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]

        public byte[] ROOT_CLUSTER; //00 00 00 02  => 2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]

        public byte[] SYS_INFO; //00 01
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]

        public byte[] BACKUP_BOOT_SECTOR; //00 06 => SECTOR 6
                                          //12 BYTE REVERSED
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]

        public byte[] UNUSED_2; //00 06 => SECTOR 6
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = 26)]
    public struct SExtendBiosBootSector
    {
        public byte PHYSIC_DRIVE; //80 => 128
        public byte REVERSE; //00
        public byte SIGNATURE; // 29
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] SERIAL_NUM; //D6 2A 0E D1

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string VOLUMN_NAME;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string FILE_SYSTEM;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = 512)]
    public struct SBootSector
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] jmp;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string OEM_ID;


        [MarshalAs(UnmanagedType.Struct, SizeConst = 53)]
        public SBiosParamBootSector BIOS_PARAM; //53

        [MarshalAs(UnmanagedType.Struct, SizeConst = 26)]
        public SExtendBiosBootSector EXTENDED_BIOS_PARAM;//26

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 420)]
        public byte[] BOOTSTRAP_CODE;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] SIGNATURE; //AA 55
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = 3281 * 512)]
    public struct SBootSystem
    {
        public byte FLAG_SECURE_UNIQUE; //80 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string UNIQUE_ID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string VOL_USER;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string VOL_PASS;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 127)]
        public string VOL_HASH_REVERSE;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3280 * 512)]
        public byte[] data;
    }
    public class BootSector
    {

        public BootSector()
        {

        }
        object Read(Stream stream, Type t)
        {
            byte[] buffer = new byte[Marshal.SizeOf(t)];
            for (int read = 0; read < buffer.Length; read += stream.Read(buffer, read, buffer.Length)) ;
            GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            object o = Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), t);
            gcHandle.Free(); return o;
        }
        void Write(Stream stream, object o)
        {
            var X = Marshal.SizeOf(o.GetType());

            byte[] buffer = new byte[Marshal.SizeOf(o.GetType())];
            GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(o, gcHandle.AddrOfPinnedObject(), true);
            stream.Write(buffer, 0, buffer.Length);
            gcHandle.Free();
        }
        public void InitBootSector()
        {
            SBootSector sb = new SBootSector();
            sb.jmp = new byte[] { 0x00, 0, 0 };
            sb.OEM_ID = "EMX1.0";

            sb.BIOS_PARAM = new SBiosParamBootSector()
            {
                BYTE_PER_SECTOR = new byte[] { 0x00, 0x02 },
                SECTORS_PER_CLUSTER = 0x08,
                REVERSE_SECTORS = new byte[] { 0xD2, 0x0C },
                NUM_OF_FAT = 0x02,
                NO_USE = new byte[9],
                NUMBER_OF_HEAD = new byte[2] { 0xff, 0x00 },
                HIDDEN_SECTOR = new byte[4] { 0x00, 0x08, 0x00, 0x00 },
                TOTAL_SECTOR = new byte[4],
                SECTOR_PER_FAT = new byte[4] { 0x97, 0x39, 0x00, 0x00 },
                VERSION = new byte[4] { 0x00, 0x12, 0x01, 0x22 },
                ROOT_CLUSTER = new byte[4] { 0x02, 0x00, 0x00, 0x00 },
                SYS_INFO = new byte[2] { 0x01, 0x00 },
                BACKUP_BOOT_SECTOR = new byte[2] { 0x06, 0x00 },
                UNUSED_2 = new byte[12]
            };

            sb.EXTENDED_BIOS_PARAM = new SExtendBiosBootSector()
            {
                PHYSIC_DRIVE = 128,
                REVERSE = 0,
                SIGNATURE = 0x29,
                SERIAL_NUM = new byte[4] { 0x23, 0x24, 0x25, 0x26 },
                VOLUMN_NAME = "OCUN0G1",
                FILE_SYSTEM = "EMXFAT"
            };


            sb.BOOTSTRAP_CODE = new byte[420];
            sb.SIGNATURE = new byte[2] { 0x55, 0xAA };
            //short num = BitConverter.ToInt16(sb.BIOS_PARAM.BYTE_PER_SECTOR, 0);
            SBootSystem bootSystem = new SBootSystem();
            SFileAllocationTable fat1 = new SFileAllocationTable();
            using (FileStream fileStream = new FileStream(@"test.dat", FileMode.OpenOrCreate))
            {
                Write(fileStream, sb);
                Write(fileStream, bootSystem);
                Write(fileStream, fat1);
                Write(fileStream, fat1);
                //fileStream.Seek(0, SeekOrigin.Begin);

                // SBootSector test2 = (SBootSector)Read(fileStream, typeof(SBootSector));

                //Console.WriteLine("Name: {0}", test2.EXTENDED_BIOS_PARAM.VOLUMN_NAME);

            }

            // Console.WriteLine(num); 
        }
    }

}