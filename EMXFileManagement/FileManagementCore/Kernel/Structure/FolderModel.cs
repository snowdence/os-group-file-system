using FileManagementCore.Kernel.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManagementCore.Kernel.Structure
{
    public class FolderModel : DataComponent
    {
        public int dir_cluster = 2; //to direct to table  default is root
        bool recursive = false; 
     
        public DiskManagement _core_disk;
        public FolderModel(SRDETEntry entry) : base(entry)
        {
            dir_cluster = BitConverter.ToInt32(entry.FIRST_CLUSTER_LOW_WORD, 0);
        }
        public FolderModel(DiskManagement disk, SRDETEntry entry, bool recursive_query) : base(entry)
        {
            this._core_disk = disk;
           
            dir_cluster = BitConverter.ToInt32(entry.FIRST_CLUSTER_LOW_WORD, 0);
            if (recursive_query) { 
                this.GetAllInside();
            }
        }
        public FolderModel(DiskManagement disk,  bool recursive_query) : base()
        {

            this._core_disk = disk;
            
            //dir_cluster = BitConverter.ToInt32(entry.FIRST_CLUSTER_LOW_WORD, 0);
            if (recursive_query)
            {
                this.GetAllInside();
            }
        }
        public FolderModel()
        {
            
        }
   



        public List<DataComponent> GetAllInside()
        {
            int folder_rdet_cluster = this.dir_cluster;
            SRDET rdet_cache = _core_disk.ReadRDETCache(folder_rdet_cluster);
            for (int i = 1; i < rdet_cache.entries.Count(); i++)
            {
                SRDETEntry _e = rdet_cache.entries[i];
                if (_e.FLAG == 0x00)
                {
                    break;
                }
                if (_e.FLAG == 0x02)
                {
                    //file
                    _list_component.Add(new FileModel( _e));
                }
                else if (_e.FLAG == 0x03)
                {
                    _list_component.Add(new FolderModel(_core_disk,  _e, true));
                }
            }
            return _list_component;
        }
      

        public override int DataSize()
        {
            return base.DataSize();
        }
        public override void PrintPretty(string indent, bool last)
        {

            base.PrintPretty(indent, last);

        }
        public override SRDETEntry GetEntry()
        {
            return base.GetEntry();
        }
    }
}
