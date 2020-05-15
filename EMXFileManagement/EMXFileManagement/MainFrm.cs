using FileManagementCore.Kernel.Structure;
using FileManagementCore.Kernel.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMXFileManagement
{
    //https://codeblitz.wordpress.com/2009/07/29/perfect-match-composite-and-visitor-pattern/
    public partial class MainFrm : Form
    {
        DiskManagement disk = new DiskManagement();

        FileManagement fileManagement;
        FolderModel root;
        public string current_location = "";
        public DataComponent current_selected;
        public MainFrm()
        {
            InitializeComponent();

            treeView1.AfterSelect += TreeView1_AfterSelect;
            listView1.MouseClick += listView1_MouseClick;

        }
        private void openDiskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!disk.IsOpened)
            {
                disk.OpenStream();
            }
            disk.ReadFatCache();
            fileManagement = new FileManagement(disk);
            root = new FolderModel();
            root._core_disk = disk;
            load();
        }
        void load()
        {
            List<DataComponent> dataComponents = root.GetAllInside();
            root.PrintPretty(" ", true);
            Console.WriteLine(" \n\n\n");
            TreeNode tre = root.GetTreeNode();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(tre);
            treeView1.ExpandAll();
            listView1.Items.Clear();

        }
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }
        //fix subfolder  size

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listView1.Items.Clear();
            DataComponent _current = root;
            TreeNode CurrentNode = e.Node;

            string fullpath = CurrentNode.FullPath;
            current_location = fullpath;  //full path to element

            if (fullpath == "")
                return;
            string[] parse_path = fullpath.Split('\\');
            if (parse_path.Length > 1)
            {
                for (int i = 1; i < parse_path.Length; i++)
                {
                    _current = _current.SearchComponent(parse_path[i]);
                }
            }

            current_selected = _current;


            if (_current is FolderModel)
            {

                List<DataComponent> list = ((FolderModel)_current).GetAllInside();
                foreach (DataComponent item in list)
                {
                    if (item.IsDeleted && !checkFlagDeletedShow.Checked)
                        continue; 
                    string[] row = { item.ToString(), item.Created_datetime.ToString(), item.Modified_date.ToString(), item.FileSize.ToString() };
                    var listViewItem = new ListViewItem(row);

                    listView1.Items.Add(listViewItem);
                }
            }
            //  MessageBox.Show(fullpath);
        }

      

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thông tin nhóm", " - Nhóm OS bài tập 21");
        }

        private void xoáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataComponent _current = root;
            var selected = listView1.SelectedItems[0];
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn xoá?", "Xoá", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //do something
                string full_path_file = current_location + "\\" + selected.SubItems[0].Text.ToString();

                string[] parse_path = full_path_file.Split('\\');
                if (parse_path.Length > 1)
                {
                    for (int i = 1; i < parse_path.Length; i++)
                    {
                        _current = _current.SearchComponent(parse_path[i]);
                    }
                }
                _current.Remove(disk);
                MessageBox.Show("Xoá file thành công");

            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void xuấtFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void đặtPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void phụcHồiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataComponent _current = root;
            var selected = listView1.SelectedItems[0];
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn phục hồi?", "Phục hồi", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //do something
                string full_path_file = current_location + "\\" + selected.SubItems[0].Text.ToString();

                string[] parse_path = full_path_file.Split('\\');
                if (parse_path.Length > 1)
                {
                    for (int i = 1; i < parse_path.Length; i++)
                    {
                        _current = _current.SearchComponent(parse_path[i]);
                    }
                }
                _current.Recover(disk);
                MessageBox.Show("Phục hồi file thành công");

            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }


        #region CREATE_VOLUMN_AND_ADD_FILE_SAMPLE


        #endregion

        private void createVolumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (disk.IsOpened)
            {
                disk.CloseStream();   
            }

            if (System.IO.File.Exists("disk.dat"))
            {
                System.IO.File.Delete("disk.dat");
            }

            disk.OpenStream();
            disk.CreateVolumn();
            disk.CloseStream();

            MessageBox.Show("Đã tạo volumn mới");

        }

        private void addSampleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!disk.IsOpened)
            {
                disk.OpenStream();
                disk.ReadFatCache();
            }
            


            FileManagement fm = new FileManagement(disk);


            FileModel rfile1 = new FileModel()
            {
                FileName = "rfile1",
                FileExt = "txt",
                Password = ""
            };
            List<byte> rdata1 = Enumerable.Repeat((byte)0x42, 999).ToList();
            rfile1._data = rdata1;

            FileModel rfile2 = new FileModel()
            {
                FileName = "rfile2",
                FileExt = "txt",
                Password = ""
            };
            List<byte> rdata2 = Enumerable.Repeat((byte)0x43, 4096).ToList();
            rfile2._data = rdata2;


            FolderModel root = new FolderModel();
            fm.AddNewFile(root, rfile1);
            fm.AddNewFile(root, rfile2);
            

            // Add folder con and cfile1.pdf

            FolderModel con =  fm.CreateFolder(root, "TMCon", "");
            FileModel cfile1 = new FileModel()
            {
                FileName = "cfile1",
                FileExt = "pdf",
                Password = "pass"
            };
            List<byte> cdata1 = Enumerable.Repeat((byte)0x45, 8270).ToList();
            cfile1._data = cdata1;
            fm.AddNewFile(con, cfile1);



           // List<DataComponent> root_inside = fm.GetAllInsideFolder(root);
           root.PrintPretty(" ", true);

            








        }

     
    }
}
