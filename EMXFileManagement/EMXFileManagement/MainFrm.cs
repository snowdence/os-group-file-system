using FileManagementCore.Kernel.Structure;
using FileManagementCore.Kernel.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMXFileManagement
{
    //https://codeblitz.wordpress.com/2009/07/29/perfect-match-composite-and-visitor-pattern/
    public partial class MainFrm : Form
    {
        DiskManagement disk;
        
        FileManagement fileManagement;
        FolderModel root;
        public MainFrm()
        {
            InitializeComponent();
           
            disk = new DiskManagement();
            disk.ReadFatCache();
            fileManagement = new FileManagement(disk);

            root = new FolderModel();
           
            root._core_disk = disk;
            load();
            treeView1.AfterSelect += TreeView1_AfterSelect;

        }

        //fix subfolder  size
        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listView1.Items.Clear();
            DataComponent _current = root;
            TreeNode CurrentNode = e.Node;
        
            string fullpath = CurrentNode.FullPath;
            if (fullpath == "")
                return;
            string[] parse_path = fullpath.Split('\\');
            if (parse_path.Length > 1)
            {
                for(int i =1; i<  parse_path.Length ; i++)
                {
                    _current = _current.SearchComponent(parse_path[i]); 
                }
            }

            if (_current is FolderModel)
            {
                List<DataComponent> list = ((FolderModel)_current).GetAllInside();
                foreach (DataComponent item in list)
                {
                    string[] row = { item.ToString(), item.Created_datetime.ToString(), item.Modified_date.ToString(), item.FileSize.ToString() };
                    var listViewItem = new ListViewItem(row);
                    listView1.Items.Add(listViewItem);
                }
            }
          //  MessageBox.Show(fullpath);
        }

        void load()
        {
            List<DataComponent> dataComponents = root.GetAllInside();
            root.PrintPretty(" ", true);
            Console.WriteLine(" \n\n\n");
            TreeNode tre = root.GetTreeNode();
            treeView1.Nodes.Add(tre);
            treeView1.ExpandAll();

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thông tin nhóm", " - Nhóm OS bài tập 21");
        }
    }
}
