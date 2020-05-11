using FileManagementCore.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManagementCore.Kernel.Structure
{
    public class FileModel : DataComponent
    {
       // int parent_folder_cluster = 3;

        //public int _rdet_idx { get; set;  }

        public List<byte> _data = new List<byte>();

        public FileModel()
        {
            
        }
        public FileModel(int parent_clus)
        {
            this.parent_cluster = parent_clus;
        }

        public FileModel(SRDETEntry entry, int parent_clus) :base (entry) 
        {
            this.parent_cluster = parent_clus;

        }
        public override int DataSize()
        {
            return this._data.Count;
        }

        public virtual SRDETEntry GetEntry()
        {
            return base.GetEntry();
        }

    }
}
