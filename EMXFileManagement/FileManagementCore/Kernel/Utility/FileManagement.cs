using FileManagementCore.Kernel.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _disk.WriteNewEntry(entry, parent.dir_cluster);//root cluster is 3  
        }

    }
}
