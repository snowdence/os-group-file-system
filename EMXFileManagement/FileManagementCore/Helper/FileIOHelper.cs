using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementCore.Helper
{
    public static class FileIOHelper
    {
        public static long SizeofFile(string path)
        {
            long length = new System.IO.FileInfo(path).Length;
            return length;
        }
        public static void Write(Stream stream, object o)
        {
            var X = Marshal.SizeOf(o.GetType());

            byte[] buffer = new byte[Marshal.SizeOf(o.GetType())];
            GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(o, gcHandle.AddrOfPinnedObject(), true);
            stream.Write(buffer, 0, buffer.Length);
            gcHandle.Free();
        }
        public static object Read(Stream stream, Type t)
        {
            byte[] buffer = new byte[Marshal.SizeOf(t)];
            for (int read = 0; read < buffer.Length; read += stream.Read(buffer, read, buffer.Length));
            GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            object o = Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), t);
            gcHandle.Free(); return o;
        }
    }
}
