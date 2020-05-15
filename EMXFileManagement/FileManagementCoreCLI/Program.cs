using FileManagementCore.Kernel.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FileManagementCore.Helper;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.VisualStyles;
using FileManagementCore.Kernel.Utility;
using System.Runtime.CompilerServices;

namespace FileManagementCoreCLI
{
    class Program
    {
        struct Test : ICloneable
        {
            public int[] arr;

            public Test(int cons)
            {
                arr = new int[] { 8, 9, 10 };
            }
            public object Clone()
            {
                Test other = (Test)this.MemberwiseClone();
                other.arr = arr.ToArray();
                return other;
            }
        }

        static void test_createfile(DiskManagement disk, string name, string ext, string pass, int size, int byte_rand)
        {
            FileManagementCore.Kernel.Utility.FileManagement file_management = new FileManagementCore.Kernel.Utility.FileManagement(disk);

            FileModel file = new FileModel()
            {
                FileName = name,
                FileExt = ext,
                Created_datetime = DateTime.Now,
                Last_access_date = DateTime.Now,
                Modified_date = DateTime.Now,
                Password = pass,
                
            };
            //0x46
            List<byte> data = Enumerable.Repeat((byte) byte_rand, size).ToList();
            file._data = data;
            

            file_management.CreateNewFileRoot(file);
        }

        static void test_add_folder_and_file()
        {

            if (System.IO.File.Exists("disk.dat"))
            {
                System.IO.File.Delete("disk.dat");
            }
            DiskManagement disk = new DiskManagement();
            disk.CreateVolumn();
            disk.ReadFatCache();
            test_createfile(disk, "file1", "txt", "", 4096, 0x42);
            test_createfile(disk, "file2", "exe", "bimat", 8888, 0x43);

            FolderModel folder = new FolderModel()
            {
                dir_cluster = 2
            };

            FileManagement fileManagement = new FileManagement(disk);

            fileManagement.CreateFolder(folder, "thumuccon", "");
            List<DataComponent> obj = fileManagement.GetAllInsideFolder(folder);
            FolderModel con = (FolderModel)obj[2];


            FileModel file = new FileModel()
            {
                FileName = "Ten",
                FileExt = "pdf",
                Created_datetime = DateTime.Now,
                Last_access_date = DateTime.Now,
                Modified_date = DateTime.Now,
                Password = "matkhau",

            };
            //0x46
            List<byte> data = Enumerable.Repeat((byte)0x42, 999).ToList();
            file._data = data;
            
            //size
            fileManagement.AddNewFile(con, file);
            fileManagement.CreateFolder(con, "dirindir", "pass");

            List<DataComponent> _con_detail = fileManagement.GetAllInsideFolder(con);
            file.FileName = "hihi";
            fileManagement.AddNewFile((FolderModel)_con_detail[1], file);
            var sd = fileManagement.GetAllInsideFolder((FolderModel)_con_detail[1]);



        }
        static void test_print_delete_file()
        {

            DiskManagement disk = new DiskManagement();
            disk.ReadFatCache();

            FileManagement fileManagement = new FileManagement(disk);

            FolderModel root = new FolderModel();
            root._core_disk = disk;
            //var objk = root.GetAllInside();
            //objk = ((FolderModel)objk[2]).GetAllInside();
            List<DataComponent> dataComponents  =  root.GetAllInside();

            root.PrintPretty(" ", true);
            Console.WriteLine(" \n\n\n");

            //dataComponents[2].Remove(disk);
            
            root.PrintPretty(" ", true);
            Console.WriteLine(" \n\n\n");

            FolderModel thumuccon = (FolderModel)dataComponents[2];
            List<DataComponent> trong_thu_muc_con = thumuccon.GetAllInside();

            FileModel tep_pdf = (FileModel)dataComponents[1];

            fileManagement.ExportFile(tep_pdf);

            Console.WriteLine(" \n\n\n");

            //dataComponents[2].Remove(disk);

            /*
            fileManagement.DeleteFile((FileModel)dataComponents[0]~)~;
            dataComponents = root.GetAllInside();
            root.PrintPretty(" ", true);
            Console.WriteLine(" \n\n\n");

            fileManagement.RecoverFile((FileModel)dataComponents[0]);
            dataComponents = root.GetAllInside();
            root.PrintPretty(" ", true);
            Console.WriteLine(" \n\n\n");
            */


        }
        static void Main(string[] args)
        {
            // test_add_folder_and_file();
            test_print_delete_file();

            Console.Read();
        }
    }
}
