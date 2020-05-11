using FileManagementCore.Helper;
using FileManagementCore.Kernel.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManagementCore.Kernel.Structure
{
    public class DataComponent
    {
        public int parent_cluster = 2; //to direct to table  default is root

        protected byte _flag = 0x02; //0x02 is file
        protected string _name = "";
        protected string _ext = "";
        protected byte _attribute = (byte)EntryAttribute.Archieve;//~rofile~
        protected byte _reversed = 0x00;
        protected byte _dms_time = 0x00;
        protected DateTime _created_datetime = DateTime.Now;
        protected DateTime _last_access_date = DateTime.Now;
        protected DateTime _modified_date = DateTime.Now;
        protected int _first_cluster = 0;
        protected string _password = "";
        protected int _file_size = 0;
        SRDETEntry _entry;
        protected List<DataComponent> _list_component = new List<DataComponent>();
        TreeNode _tree_node;
        public DataComponent()
        {

        }
        public virtual TreeNode GetTreeNode()
        {
            if (_name == "")
            {
                _tree_node = new TreeNode("ROOT");
            }
            else{
                if(this is FolderModel)
                {
                    _tree_node = new TreeNode($"{ this.FileName }");
                }
                else
                {
                    _tree_node = new TreeNode($"{ this.FileName }.{ this.FileExt}");

                }
            }
            // _tree_node = new TreeNode($"{ this.FileName }.{ this.FileExt } - {this.First_cluster} - { (this is FileModel ? "file" : "folder") }" );
            _tree_node.Tag = $"{FileName}.{FileExt}";
            foreach(DataComponent dataComponent in _list_component)
            {
                _tree_node.Nodes.Add(dataComponent.GetTreeNode());
            }
            return _tree_node;
        }
      
        public virtual void PrintPretty(string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }
            if (this is FolderModel)
            {
                Console.WriteLine($"{this.FileName}  - [FCluster] {this.First_cluster} - Parent {this.parent_cluster} - Deleted: {this.IsDeleted}");
            }
            else
            {
                Console.WriteLine($"{this.FileName}.{this._ext} - Cluster {this.First_cluster} - Parent {this.parent_cluster} - Size: {this._file_size}  - Deleted: {this.IsDeleted}");

            }
            for (int i = 0; i < _list_component.Count; i++)
                _list_component[i].PrintPretty(indent, i == _list_component.Count - 1);

        }
        public DataComponent(SRDETEntry entry)
        {
            this._entry = entry;
            _flag = entry.FLAG;
            _name = entry.FILE_NAME;
            _ext = entry.FILE_EXT;
            _attribute = entry.FILE_ATTRIBUTE;
            _reversed = entry.REVERSED;
            _dms_time = entry.DMSTIME;
            try
            {
                _created_datetime = DateTimeHelper.GetDateTimeFromDosDateTime(BitConverter.ToInt32(entry.CREATED_DATETIME, 0));
            }
            catch
            {

                _created_datetime = DateTime.Now;
            }

            _last_access_date = DateTime.Now;
            try
            {

                _modified_date = DateTimeHelper.GetDateTimeFromDosDateTime(BitConverter.ToInt32(entry.MODIFIED_DATETIME, 0));
            }
            catch
            {
                _modified_date = DateTime.Now;
            }
            _file_size = BitConverter.ToInt32( entry.FILE_SIZE,0);
            _first_cluster = BitConverter.ToInt32(entry.FIRST_CLUSTER_LOW_WORD, 0);
            _password = entry.PASSWORD;
        }
        public virtual SRDETEntry GetEntry()
        {
            return new SRDETEntry()
            {
                FLAG = _flag,
                FILE_NAME = _name,
                FILE_EXT = _ext,
                FILE_ATTRIBUTE = _attribute,
                REVERSED = _reversed,
                DMSTIME = _dms_time,
                CREATED_DATETIME = BitConverter.GetBytes(DateTimeHelper.ToDOSDateTimeInt(_created_datetime)),
                LAST_ACCESS_DATE = BitConverter.GetBytes(DateTimeHelper.ToDOSDate(_created_datetime)),
                MODIFIED_DATETIME = BitConverter.GetBytes(DateTimeHelper.ToDOSDateTimeInt(_modified_date)),
                FIRST_CLUSTER_LOW_WORD = BitConverter.GetBytes(_first_cluster),
                FILE_SIZE = BitConverter.GetBytes(this.DataSize()),
                PASSWORD = _password
            };
        }
        public virtual int DataSize()
        {
            return this._file_size;
        }

        //public string Data { get => _data; set => _data = value; }

        public byte Flag { get => _flag; set => _flag = value; }
        public int FileSize { get => _file_size; set => _file_size = value; }
        public string FileName { get => _name; set => _name = value; }
        public string FileExt { get => _ext; set => _ext = value; }
        public byte Attribute { get => _attribute; set => _attribute = value; }

        public byte Reversed { get => _reversed; set => _reversed = value; }
        //public byte Dms_time { get => _dms_time; set => _dms_time = value; }
        public DateTime Created_datetime { get => _created_datetime; set => _created_datetime = value; }
        public DateTime Last_access_date { get => _last_access_date; set => _last_access_date = value; }
        public DateTime Modified_date { get => _modified_date; set => _modified_date = value; }
        public int First_cluster { get => _first_cluster; set => _first_cluster = value; }
        public string Password { get => _password; set => _password = value; }

        public bool IsDeleted
        {
            get => (this._reversed == 0xE5);
            set
            {
                if (value)
                {
                    _reversed = 0xE5;
                }
                else
                {
                    _reversed = 0x00;//file
                }
            }
        }
        public bool IsHidden
        {
            get => (this._attribute == (byte)EntryAttribute.HIDDEN);
            set
            {
                if (value)
                {
                    _flag = (byte)EntryAttribute.HIDDEN;
                }
            }
        }
        public bool IsReadOnly
        {
            get => (this._attribute == (byte)EntryAttribute.READ_ONLY);
            set
            {
                if (value)
                {
                    _flag = (byte)EntryAttribute.READ_ONLY;
                }
            }
        }

        public bool HasPassword()
        {
            return Password.Length > 0;
        }

        public virtual void Remove(DiskManagement disk)
        {
            int parent_cluster = this.parent_cluster;
            if (parent_cluster == 0)
            {
                //root
                parent_cluster = 2;
            }

            this.Reversed = 0xE5;
            SRDETEntry entry = this.GetEntry();
            disk.UpdateEntry(entry, parent_cluster);
        }


        public virtual void Recover(DiskManagement disk)
        {
            int parent_cluster = this.parent_cluster;
            if (parent_cluster == 0)
            {
                //root
                parent_cluster = 2;
            }

            this.Reversed = 0x00;
            SRDETEntry entry = this.GetEntry();
            disk.UpdateEntry(entry, parent_cluster);
        }
        
        public virtual DataComponent SearchComponent(string str)
        {
            for(int i =0; i < _list_component.Count; i++)
            {
                if(_list_component[i].ToString() == str)
                {
                    return _list_component[i];
                }
            }
            return null;
        }

        public override string ToString()
        {
            if (this is FolderModel)
            {
                return $"{ this.FileName }";
            }
            else
            {
                return $"{ this.FileName }.{ this.FileExt}";

            }
        }
    }
}
