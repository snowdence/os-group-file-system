          aa a = new aa();
            a.FAT_ENTRY = new byte[12] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12 };

            int size = Marshal.SizeOf(a);

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(a, ptr, true);
            byte[] arr = new byte[8];
            int n = 1;
            Marshal.Copy(ptr + n *4, arr,  0, 4);
            Marshal.FreeHGlobal(ptr);

             aa a = new aa();
            a.FAT_ENTRY = new byte[12] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12 };

            int size = Marshal.SizeOf(a);

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(a, ptr, true);
            byte[] arr = new byte[4] { 0x1A, 0x1B,0x1C, 0x1D};
            int n = 1;
            Array.Copy(arr, 0, a.FAT_ENTRY, 4, 4);
            //Marshal.WriteIntPtr(ptr, 0 * 4,     ((IntPtr)(i + 1)));
            Marshal.FreeHGlobal(ptr);
