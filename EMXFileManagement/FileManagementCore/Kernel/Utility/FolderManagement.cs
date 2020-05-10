using FileManagementCore.Kernel.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManagementCore.Kernel.Utility
{
    public class FolderManagement
    {
        DiskManagement _disk;
        public FolderManagement(DiskManagement disk)
        {
            this._disk = disk;
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
        public void CreateFolder(FolderModel parent, string folder_name, string password ="")
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


        }
        public List<DataComponent>  GetAllInsideFolder(FolderModel folder)
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
                    data.Add(new FileModel(_e));
                }
                else if (_e.FLAG == 0x03)
                {
                    data.Add(new FolderModel(_e));
                }
            }
            return data;
        }

        
    }
}
