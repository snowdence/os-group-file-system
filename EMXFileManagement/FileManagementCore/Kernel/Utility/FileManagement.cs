using FileManagementCore.Kernel.Structure;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManagementCore.Kernel.Utility
{

    //khong can biet offset cua sector
    public class FileManagement
    {
        DiskManagement _disk;
        public FileManagement(DiskManagement disk)
        {
            this._disk = disk;

        }





        /// <summary>
        /// Create a file with model
        /// </summary>
        /// <param name="fileModel"></param>
        /// <returns></returns>
        /// 1. Tìm các cluster trống trong FAT ROOT
        /// 2. Lấy các danh sách cluster ghi
        /// 3. Ghi dữ liệu vào FAT các cluster đã dùng
        /// 4. Ghi entry
        /// 5. Ghi data
        ///
        public int CreateNewFileRoot(FileModel fileModel) {
            //1. Get all empty cluster and write data
            int file_size = (int) fileModel.DataSize();
            List<int>list_wrote =  _disk.WriteBlockData(fileModel._data.ToArray(), file_size);
            //2. Create entry and write to RDET
            SRDETEntry entry = fileModel.GetEntry();
            entry.FIRST_CLUSTER_LOW_WORD = BitConverter.GetBytes(list_wrote[0]);
            entry.FILE_SIZE = BitConverter.GetBytes(file_size);
            _disk.WriteNewEntry(entry, 2);//root cluster is 3
            //int get_empty =
            return 0;
        }
        public void AddNewFile(FolderModel parent, FileModel file)
        {
            int file_size = (int)file.DataSize();
            List<int> list_wrote = _disk.WriteBlockData(file._data.ToArray(), file_size);

            SRDETEntry entry = file.GetEntry();
            entry.FIRST_CLUSTER_LOW_WORD = BitConverter.GetBytes(list_wrote[0]);
            entry.FILE_SIZE = BitConverter.GetBytes(file_size);
            _disk.WriteNewEntry(entry, parent.dir_cluster);//root cluster is 3  

        }
        public void DeleteFile(FileModel file)
        {
            int parent_cluster = file.parent_cluster;
            if(parent_cluster == 0)
            {
                //root
                parent_cluster = 2;
            }

            file.Reversed = 0xE5;
            SRDETEntry entry = file.GetEntry();
            _disk.UpdateEntry(entry, parent_cluster);
        }

        public void RecoverFile(FileModel file)
        {
            int parent_cluster = file.parent_cluster;
            if (parent_cluster == 0)
            {
                //root
                parent_cluster = 2;
            }

            file.Reversed = 0x00;
            SRDETEntry entry = file.GetEntry();
            _disk.UpdateEntry(entry, parent_cluster);
        }


        byte[] getBytes(SRDETEntry str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
        public FolderModel CreateFolder(FolderModel parent, string folder_name, string password = "")
        {
            int dir_cluster_rdet = parent.dir_cluster;
            int current_dir_cluster_rdet = _disk.GetNextClusterEmpty();
            _disk.WriteBlockFileWrittenFat(current_dir_cluster_rdet);//mark


            //write buffer with entry

            FolderModel folderModel = new FolderModel()
            {
                FileName = folder_name,
                FileExt = "",
                Flag = 0x03,
                Attribute = (byte)EntryAttribute.SUB_DIRECTORY,
                Created_datetime = DateTime.Now,
                Modified_date = DateTime.Now,
                Last_access_date = DateTime.Now,
                First_cluster = current_dir_cluster_rdet,
                dir_cluster = current_dir_cluster_rdet,
                Password = password
            };
            SRDET cluster_sdet = new SRDET();
            cluster_sdet.entries = new SRDETEntry[64];
            cluster_sdet.entries[0] = folderModel.GetEntry();
            //_disk.WriteBlockData(getBytes(folderModel.GetEntry()), 4096, current_dir_cluster_rdet);
            _disk.WriteSDETCluster(cluster_sdet, current_dir_cluster_rdet);
            _disk.WriteNewEntry(folderModel.GetEntry(), dir_cluster_rdet);//root cluster is 3
            return folderModel;
        }
        public List<DataComponent> GetAllInsideFolder(FolderModel folder)
        {
            List<DataComponent> data = new List<DataComponent>();
            int folder_rdet_cluster = folder.dir_cluster;
            SRDET rdet_cache = _disk.ReadRDETCache(folder_rdet_cluster);
            for (int i = 0; i < rdet_cache.entries.Count(); i++)
            {
                SRDETEntry _e = rdet_cache.entries[i];
                if (_e.FLAG == 0x00)
                {
                    break;
                }
                if (_e.FLAG == 0x02)
                {
                    //file
                    data.Add(new FileModel(_e, folder.dir_cluster));
                }
                else if (_e.FLAG == 0x03)
                {
                    data.Add(new FolderModel(_e, folder.dir_cluster));
                }
            }
            return data;
        }
        public void DeleteFolder(FolderModel folder)
        {
            int parent_cluster = folder.parent_cluster;
            int dir_cluster_sdet = folder.dir_cluster;

        }
        public void ExportFile(FileModel file, string path)
        {
            uint eof = BitConverter.ToUInt32(new byte[] { 0xFF, 0xFF, 0xFF, 0x0F }, 0);

            List<byte> bytes = new List<byte>();
            int first_cluster = file.First_cluster;
            uint next_cluster = (uint)first_cluster;

            do {
                SCluster sCluster = _disk.ReadBlockData((int)next_cluster);
                bytes.AddRange(sCluster.data.ToList());
                
                next_cluster = _disk.ReadFatEntry((int)next_cluster);
            } while (next_cluster != eof);

            var fileFullName = file.FileName + "." + file.FileExt;

            File.WriteAllBytes(path + "\\"+fileFullName, bytes.ToArray()); 
            /* 
             * Not need to do this
            Console.Write("Where do you want to export \"" + fileFullName + "\"?");
            Console.WriteLine(" (Ex: D:, D:\\share, C:\\user\\me\\files)");
            Console.Write("Enter full path here => ");

            path = Console.ReadLine();
            path += "\\\\" + fileFullName;

            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            for (var i = 0; i < file.FileSize; ++i) {
              bw.Write(bytes[i]);
            }
            bw.Close();
            */
        }
    }
}
