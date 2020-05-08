using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EMXFileManagementCLI
{

    //https://timvw.be/2007/01/20/reading-and-writing-binary-files/
    public class Example
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = 512)]
        struct Test1
        {
            //fixed size = 9 +4 = ~13~
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string Name;
            public int Score;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct Test2
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public Test1[] Items;
        }

        static object Read(Stream stream, Type t)
        {
            byte[] buffer = new byte[Marshal.SizeOf(t)];
            for (int read = 0; read < buffer.Length; read += stream.Read(buffer, read, buffer.Length)) ;
            GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            object o = Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), t);
            gcHandle.Free(); return o;
        }

        static void Write(Stream stream, object o)
        {
            byte[] buffer = new byte[Marshal.SizeOf(o.GetType())];
            GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(o, gcHandle.AddrOfPinnedObject(), true);
            stream.Write(buffer, 0, buffer.Length);
            gcHandle.Free();
        }

        static void Main(string[] args)
        {
            Test1 test1 = new Test1();
            test1.Name = "timvw";
            test1.Score = 99;
            Test1 _test1 = new Test1();
            _test1.Name = "snowdence";
            _test1.Score = 99;
            using (FileStream fileStream = new FileStream(@"test.dat", FileMode.OpenOrCreate))
            {
                Write(fileStream, test1);
                Write(fileStream, _test1);
                fileStream.Seek(0, SeekOrigin.Begin);
                Test1 test2 = (Test1) Read(fileStream, typeof(Test1));
                Test1 _test2 = (Test1) Read(fileStream, typeof(Test1));
               // Read(fileStream, typeof(Test1));
                Console.WriteLine("Name: {0} Score: {1}", test2.Name, test2.Score);
                Console.WriteLine("Name: {0} Score: {1}", _test2.Name, _test2.Score);
            }
            Console.Write("{0}Press any key to continue...", Environment.NewLine);
            Console.ReadKey();
        }

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //unsafe struct ExampleStruct
        //{
        //    public byte b1;
        //    public byte b2;
        //    public int b3;
        //    public fixed byte b4[10];
        //    public ExampleStruct(byte dummy)
        //    {
        //        b1 = 0x01;
        //        b2 = 0x01;
        //        b3 = 0xFF ;
        //        fixed (byte* p = this.b4)
        //        {
        //            *p = (byte)'J';
        //            *(p + 1) = (byte)'o';
        //            *(p + 2) = (byte)'n';
        //            *(p + 3) = 0;
        //        }
        //    }
        //}


        //// Display a byte array, using multiple lines if necessary.
        //public static void WriteMultiLineByteArray(byte[] bytes,
        //    string name)
        //{
        //    const int rowSize = 16;
        //    const string underLine = "--------------------------------";
        //    int iter;

        //    Console.WriteLine(name);
        //    Console.WriteLine(underLine.Substring(0,
        //        Math.Min(name.Length, underLine.Length)));

        //    for (iter = 0; iter < bytes.Length - rowSize; iter += rowSize)
        //    {
        //        Console.Write(
        //            BitConverter.ToString(bytes, iter, rowSize));
        //        Console.WriteLine("-");
        //    }

        //    Console.WriteLine(BitConverter.ToString(bytes, iter));
        //    Console.WriteLine();
        //}


        ////Get bytes from struct
        //static byte[] getBytes<T>(T str) where T :struct
        //{
        //    int size = Marshal.SizeOf(str);
        //    byte[] arr = new byte[size];

        //    IntPtr ptr = Marshal.AllocHGlobal(size);
        //    Marshal.StructureToPtr(str, ptr, true);
        //    Marshal.Copy(ptr, arr, 0, size);
        //    Marshal.FreeHGlobal(ptr);
        //    return arr;
        //}
        //public static byte[] StringToByteArray(string hex)
        //{
        //    return Enumerable.Range(0, hex.Length)
        //                     .Where(x => x % 2 == 0)
        //                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
        //                     .ToArray();
        //}

        //public unsafe static void Main()
        //{
        //    string boot_hex = "";
        //    ExampleStruct ex = new ExampleStruct(0xff);
        //    byte* addr = (byte*)&ex;
        //    int c = sizeof(ExampleStruct);
        //    byte[] list = getBytes<ExampleStruct>(ex);
        //    WriteMultiLineByteArray(list, "EX");
        //    //Console.WriteLine("0x{0:X2}", *(addr+1));
        //    int s = sizeof(ExampleStruct);
        //    for(int i =0; i < s; i++)
        //    {
        //        Console.Write("{0:X2} ", *(addr + i));
        //    }
        //    Console.WriteLine();


        //    Console.WriteLine("Size:      {0}", sizeof(ExampleStruct));
        //    Console.WriteLine("b1 Offset: {0}", &ex.b1 - addr);
        //    Console.WriteLine("b2 Offset: {0}", &ex.b2 - addr);
        //    Console.WriteLine("i3 Offset: {0}", (byte*)&ex.b3 - addr);
        //}
    }
}
