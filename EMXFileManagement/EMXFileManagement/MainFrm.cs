using EMXFileManagement.Module;
using FileManagementCore.Helper;
using FileManagementCore.Kernel.Structure;
using FileManagementCore.Kernel.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        bool recycle_bin = false;


        public MainFrm()
        {
            InitializeComponent();

            treeView1.AfterSelect += TreeView1_AfterSelect;
            listView1.MouseClick += listView1_MouseClick;
            listView1.MouseDown += ListView1_MouseDown;
        }

        private void ListView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.GetItemAt(e.X, e.Y) == null)
                {
                    contextMenuStrip2.Show(Cursor.Position);
                }
            }
        }

        private void openDiskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Nếu disk đang mở thì đóng lại trước tránh 2 class đọc 1 file sẽ crash
            if (!disk.IsOpened)
            {
                disk.OpenStream();//mở file disk
            }

            //đọc fat vào cache (biến trong disk management)
            disk.ReadFatCache();

            //quản lý file
            fileManagement = new FileManagement(disk);
            //Tạo folder root mặc định cluster 2 
            root = new FolderModel();
            //gán disk vào để root có thể sử dụng disk. Vì chỉ được 1 DiskManagement được truy xuất volumn nên mới cần ~~ph~~ải gán vậy
            root._core_disk = disk;
            //lấy cây thư mục từ root, gán vào nodes treeview. 
            load();
        }
        /// <summary>
        /// Gọi getallinside sẽ lấy toàn bộ cây thư mục. Reload lại treeview và listview
        /// </summary>
        void load()
        {
            recycle_bin = false;
            List<DataComponent> dataComponents = root.GetAllInside();
            root.PrintPretty(" ", true);
            Console.WriteLine(" \n\n\n");
            //Xây dựng cây thư mục từ root


            TreeNode tre = root.GetTreeNode(checkFlagHiddenShow.Checked);

            //xoá tạm các node cũ  (trường hợp f5 lại thì xoá dữ liệu cũ)
            listView1.Items.Clear();

            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(tre);
            treeView1.SelectedNode = tre;
            treeView1.ExpandAll();


        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    //Hiển thị menu chuột phải khi chọn 1 phần tử listview
                    contextMenuStrip1.Show(Cursor.Position);
                }
                else
                {
                    contextMenuStrip2.Show(Cursor.Position);
                }
            }
        }
        //fix subfolder  size

        //Sự kiện khi chọn 1 phần tử trong treeview
        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            current_selected = root;
            listView1.Items.Clear();
            DataComponent _current = root;
            TreeNode CurrentNode = e.Node;


            string fullpath = CurrentNode.FullPath; // Ví dụ : Root\thumucon\cfil1.txt
            current_location = fullpath;  //full path to element


            //Từ 1 phần tử chọn treeview  chọn truy xuất các phần tử con trong Root để tìm phần tử được chọn 
            //Cấu trúc fullpath 
            if (fullpath == "")
                return;
            string[] parse_path = fullpath.Split('\\');
            if (parse_path.Length > 1)
            {
                //i =1  vì bỏ qua ROOT. Tìm trong root là đủ
                for (int i = 1; i < parse_path.Length; i++)
                {
                    _current = _current.SearchComponent(parse_path[i]);
                }
            }


            //Đã tìm được thư mục hoặc file vừa chọn trong treeview
            current_selected = _current;


            //nếu current là FolderModel. Do sử dụng composite pattern nên cần check
            if (_current is FolderModel)
            {
                if (_current.HasPassword())
                {
                    string promptValue = ShowDialog("Nhập mật khẩu của file hoặc thư mục", "Mật khẩu");

                    if (OOHashHelper.getString(promptValue) != _current.Password)
                    {
                        MessageBox.Show("Mật khẩu sai");
                        return;
                    }
                }
                List<DataComponent> list = new List<DataComponent>();

                if (recycle_bin)
                {
                    list = ((FolderModel)_current).GetAllInsideRecycleBin();
                }
                else
                {
                    list = ((FolderModel)_current).GetAllInside();
                }


                foreach (DataComponent item in list)
                {
                    if (item.IsHidden && checkFlagHiddenShow.Checked == false)
                    {
                        continue;
                    }
                    string status = "";
                    if (item.IsDeleted)
                    {
                        status = "Đã xoá";
                    }
                    else if (item.IsHidden)
                    {
                        status = "Đã ẩn";
                    }
                    else if (item.IsReadOnly)
                    {
                        status = "Chỉ đọc";
                    }
                    if (item.HasPassword())
                    {
                        status += "Đã bảo mật bằng pass";
                    }
                    string[] row = { item.ToString(), item.Created_datetime.ToString(), item.Modified_date.ToString(), item.DataSize().ToString()
                        ,status, item.First_cluster.ToString()
                    };
                    var listViewItem = new ListViewItem(row);
                    listViewItem.ImageIndex = (item is FolderModel) ? 0 : 1;

                    //thêm row vào listview
                    listView1.Items.Add(listViewItem);
                }
            }
            //  MessageBox.Show(fullpath);
        }



        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thông tin nhóm", " - Nhóm OS bài tập 21");
        }


        public string ShowDialog(string text, string caption, bool pass_char = true)
        {

            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            textBox.UseSystemPasswordChar = pass_char;

            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
        private void xoáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataComponent _current = root;

            DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn xoá?", "Xoá", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //do something
                _current = GetCurrentSelectedListView();

                if (_current.HasPassword())
                {
                    string promptValue = ShowDialog("Nhập mật khẩu của file hoặc thư mục", "Mật khẩu");
                    if (OOHashHelper.getString(promptValue) != _current.Password)
                    {
                        MessageBox.Show("Mật khẩu sai");
                        return;
                    }

                }

                if (_current.IsDeleted)
                {
                    _current.RemovePermanently(disk);
                }
                else
                {
                    _current.Remove(disk);
                }


                MessageBox.Show("Xoá file thành công");


                if (recycle_bin)
                {
                    this.recyle_bin_load();
                }
                else
                {
                    this.load();
                }

            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }

        }

        private void xuấtFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataComponent _current = root;
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xuất file", "Xuất file", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                _current = GetCurrentSelectedListView();


                if (_current is FileModel)
                {
                    if (_current.HasPassword())
                    {
                        string promptValue = ShowDialog("Nhập mật khẩu của file hoặc thư mục", "Mật khẩu");
                        if (OOHashHelper.getString(promptValue) != _current.Password)
                        {
                            MessageBox.Show("Mật khẩu sai");
                            return;
                        }

                    }
                    using (var fbd = new FolderBrowserDialog())
                    {
                        DialogResult result = fbd.ShowDialog();

                        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                        {
                            string path = fbd.SelectedPath;
                            fileManagement.ExportFile((FileModel)_current, path);
                        }
                    }
                    _current.Recover(disk);
                    MessageBox.Show("Xuất file thành công" + _current.ToString());
                    this.load();
                }
                else
                {
                    MessageBox.Show("Chưa hỗ trợ xuất folder");

                }

            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void đặtPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataComponent _current = root;
            _current = GetCurrentSelectedListView();
            if (_current.Password != "")
            {
                //khong co pass
                string oldPass = ShowDialog("Nhập pass cũ", "Mật khẩu");
                if (OOHashHelper.getString(oldPass) != _current.Password)
                {
                    MessageBox.Show("Bạn đã nhập sai pass cũ");
                    return;
                }

            }

            string newPass = ShowDialog("Nhập pass mới", "Mật khẩu");
            string reNewPass = ShowDialog("Nhập lại pass mới", "Mật khẩu");
            if (newPass != reNewPass)
            {
                MessageBox.Show("Bạn đã nhập xác nhận pass mới sai");
                return;
            }

            _current.SetPassword(disk, newPass);
            MessageBox.Show("Đặt mật khẩu thành công");
        }
        private void ẩnFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataComponent _current = root;

            DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn ẩn file?", "Ẩn file", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //do something
                _current = GetCurrentSelectedListView();

                if (_current.HasPassword())
                {
                    string promptValue = ShowDialog("Nhập mật khẩu của file hoặc thư mục", "Mật khẩu");
                    if (OOHashHelper.getString(promptValue) != _current.Password)
                    {
                        MessageBox.Show("Mật khẩu sai");
                        return;
                    }

                }
                _current.Hide(disk);

                MessageBox.Show("Ẩn file thành công");
                this.load();

            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }

        }

        /// <summary>
        /// Chọn file hoặc folder dựa vào mục đã chọn trong treeview
        /// </summary>
        /// <returns>Trả về đối tượng tới file hoặc folder đã chọn </returns>
        public DataComponent GetCurrentSelectedListView()
        {
            DataComponent _current = root;
            var selected = listView1.SelectedItems[0];
            string full_path_file = current_location + "\\" + selected.SubItems[0].Text.ToString();

            string[] parse_path = full_path_file.Split('\\');
            if (parse_path.Length > 1)
            {
                for (int i = 1; i < parse_path.Length; i++)
                {
                    _current = _current.SearchComponent(parse_path[i]);
                }
            }
            return _current;

        }
        private void phụcHồiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataComponent _current = root;

            DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn phục hồi?", "Phục hồi", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //do something
                _current = GetCurrentSelectedListView();
                if (_current.HasPassword())
                {
                    string promptValue = ShowDialog("Nhập mật khẩu của file hoặc thư mục", "Mật khẩu");
                    if (OOHashHelper.getString(promptValue) != _current.Password)
                    {
                        MessageBox.Show("Mật khẩu sai");
                        return;
                    }

                }
                _current.Recover(disk);
                MessageBox.Show("Phục hồi file thành công");
                this.load();

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
            List<byte> rdata1 = Enumerable.Repeat((byte)0x61, 4097).ToList();
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
            //fm.AddNewFile(root, rfile2);


            // Add folder con and cfile1.pdf
            FolderModel con = fm.CreateFolder(root, "TMCon", "");



            FileModel cfile1 = new FileModel()
            {
                FileName = "cfile1",
                FileExt = "pdf",
                Password = OOHashHelper.getString("pass")
            };
            List<byte> cdata1 = Enumerable.Repeat((byte)0x45, 8270).ToList();
            cfile1._data = cdata1;
            fm.AddNewFile(con, cfile1);

            FolderModel con_cua_con = fm.CreateFolder(con, "ConcuaCon", "");

            FileModel cFileCon1 = new FileModel()
            {
                FileName = "cFileCon1",
                FileExt = "exe",
                Password = OOHashHelper.getString("")
            };
            List<byte> cdatacon1 = Enumerable.Repeat((byte)0x49, 9999).ToList();
            cFileCon1._data = cdatacon1;
            fm.AddNewFile(con_cua_con, cFileCon1);

            root.PrintPretty(" ", true);
        }



        private void btnRefresh_Click(object sender, EventArgs e)
        {

            if (!disk.IsOpened)
            {
                if (File.Exists("disk.dat"))
                {
                    disk.OpenStream();
                }
                else
                {
                    MessageBox.Show("Vui lòng tạo ổ cứng mẫu hoặc import từ file ổ cứng có sẵn");
                }
            }
            disk.ReadFatCache();
            fileManagement = new FileManagement(disk);
            root = new FolderModel();
            root._core_disk = disk;
            if (recycle_bin)
            {
                this.recyle_bin_load();
            }
            else
            {
                this.load();
            }
        }


        /// <summary>
        /// Nhập file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nhậpFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (current_selected != null && !(current_selected is FolderModel))
            {
                MessageBox.Show("Vui lòng chọn 1 thư mục từ treeview");
                return;
            }

            //DataComponent _current = GetCurrentSelectedListView();
            string filePath = "";
            string fileContent = "";
            string fileName = "";

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    fileName = Path.GetFileNameWithoutExtension(filePath);
                    fileContent = Path.GetExtension(filePath);
                    if (string.IsNullOrEmpty(fileContent))
                    {
                        fileContent = "known";
                    }
                    else
                    {
                        fileContent = fileContent.Substring(1);
                    }
                    FileModel file = new FileModel()
                    {
                        FileName = fileName,
                        FileExt = fileContent,
                        Created_datetime = DateTime.Now,
                        Last_access_date = DateTime.Now,
                        Modified_date = DateTime.Now,
                        Password = "",
                    };

                    byte[] fileOrigin = File.ReadAllBytes(filePath);
                    List<byte> data = fileOrigin.ToList();
                    file._data = data;

                    fileManagement.AddNewFile((FolderModel)current_selected, file);
                    // MessageBox.Show($"Thêm file thành công{fileName}.{fileContent}");

                    load();
                }
            }

        }
        void recyle_bin_load()
        {
            recycle_bin = true;
            Console.WriteLine("Mở thùng rác");

            if (!disk.IsOpened)
            {
                disk.OpenStream();
            }
            disk.ReadFatCache();
            fileManagement = new FileManagement(disk);
            root = new FolderModel();
            root._core_disk = disk;

            List<DataComponent> dataComponents = root.GetAllInsideRecycleBin();
            root.PrintPretty(" ", true);
            Console.WriteLine(" \n\n\n");
            //Xây dựng cây thư mục từ root


            TreeNode tre = root.GetRecycleBinNode();

            //xoá tạm các node cũ  (trường hợp f5 lại thì xoá dữ liệu cũ)
            listView1.Items.Clear();

            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(tre);
            treeView1.SelectedNode = tre;
            treeView1.ExpandAll();


        }


        /// <summary>
        /// toolstrip mở thùng rác
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            recyle_bin_load();
        }


        //Dang lam
        public string ShowProperty()
        {
            DataComponent model = GetCurrentSelectedListView();
            ucPropertyFrm ucPropertyFrm = new ucPropertyFrm();
            ucPropertyFrm.Root = root;
            ucPropertyFrm.Model = model;
            ucPropertyFrm.LocationPath = current_location;
            ucPropertyFrm.Dock = DockStyle.Top;
            ucPropertyFrm.Init();
            Form prompt = new Form()
            {
                Width = ucPropertyFrm.Width + 10,
                Height = ucPropertyFrm.Height + 100,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Property",
                StartPosition = FormStartPosition.CenterScreen
            };

            Button confirmation = new Button() { Text = "Ok", Left = ucPropertyFrm.Width - 220, Width = 100, Top = ucPropertyFrm.Height, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Cancel", Left = ucPropertyFrm.Width - 110, Width = 100, Top = ucPropertyFrm.Height, DialogResult = DialogResult.Cancel };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) =>
            {
                prompt.Close();
            };
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(ucPropertyFrm);
            DialogResult rsltDlg = prompt.ShowDialog();
            if (rsltDlg == DialogResult.Cancel)
            {
                return "";
                //handle Cancel
            }
            else if (rsltDlg == DialogResult.OK)
            {
                bool isHidden = ucPropertyFrm.CheckHidden; // true hide
                string FileName = ucPropertyFrm.FileName;
                var ext = Path.GetExtension(FileName);
                var name = Path.GetFileNameWithoutExtension(FileName);
                if (!string.IsNullOrEmpty(ext))
                {
                    ext = ext.Substring(1);
                }
                model.FileName = name;
                model.FileExt = ext ?? "";
                model.IsHidden = isHidden;

                disk.UpdateEntry(model.GetEntry(), model.parent_cluster);
                this.load();

            }
            return "";
        }

        private void thuộcTínhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataComponent listview_selected_item = GetCurrentSelectedListView();
            ShowProperty();
        }

        private void detailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.View = View.Details;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.View = View.List;

        }

        private void smallIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.View = View.SmallIcon;

        }

        private void largeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.View = View.LargeIcon;

        }

        private void titleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.View = View.Tile;

        }


        #region SEARCH_NODE_IN_TREEVIEW_STACKOVERFLOW Source

        private void btnSearch_Click(object sender, EventArgs e)
        {

            string searchText = this.txtSearchFileName.Text;
            if (String.IsNullOrEmpty(searchText))
            {
                return;
            };


            if (LastSearchText != searchText)
            {
                //It's a new Search
                CurrentNodeMatches.Clear();
                LastSearchText = searchText;
                LastNodeIndex = 0;
                SearchNodes(searchText, treeView1.Nodes[0]);
            }

            if (LastNodeIndex >= 0 && CurrentNodeMatches.Count > 0 && LastNodeIndex < CurrentNodeMatches.Count)
            {
                TreeNode selectedNode = CurrentNodeMatches[LastNodeIndex];
                LastNodeIndex++;
                this.treeView1.SelectedNode = selectedNode;
                this.treeView1.SelectedNode.Expand();
                this.treeView1.Select();
            }
        }
        private List<TreeNode> CurrentNodeMatches = new List<TreeNode>();

        private int LastNodeIndex = 0;

        private string LastSearchText;



        private void SearchNodes(string SearchText, TreeNode StartNode)
        {
            TreeNode node = null;
            while (StartNode != null)
            {
                if (StartNode.Text.ToLower().Contains(SearchText.ToLower()))
                {
                    CurrentNodeMatches.Add(StartNode);
                };
                if (StartNode.Nodes.Count != 0)
                {
                    SearchNodes(SearchText, StartNode.Nodes[0]);//Recursive Search 
                };
                StartNode = StartNode.NextNode;
            };

        }
        #endregion
        void CreateSampleDisk()
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


        }
        void AddSampleFileAndFolder()
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
            List<byte> rdata1 = Enumerable.Repeat((byte)0x61, 4097).ToList();
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
            //fm.AddNewFile(root, rfile2);


            // Add folder con and cfile1.pdf
            FolderModel con = fm.CreateFolder(root, "TMCon", "");



            FileModel cfile1 = new FileModel()
            {
                FileName = "cfile1",
                FileExt = "pdf",
                Password = OOHashHelper.getString("pass")
            };
            List<byte> cdata1 = Enumerable.Repeat((byte)0x45, 8270).ToList();
            cfile1._data = cdata1;
            fm.AddNewFile(con, cfile1);

            FolderModel con_cua_con = fm.CreateFolder(con, "ConcuaCon", "");

            FileModel cFileCon1 = new FileModel()
            {
                FileName = "cFileCon1",
                FileExt = "exe",
                Password = OOHashHelper.getString("")
            };
            List<byte> cdatacon1 = Enumerable.Repeat((byte)0x49, 9999).ToList();
            cFileCon1._data = cdatacon1;
            fm.AddNewFile(con_cua_con, cFileCon1);

            root.PrintPretty(" ", true);
        }

        /// <summary>
        /// Toolstrip create and add sample volumn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
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

            AddSampleFileAndFolder();
            MessageBox.Show("Đã tạo volumn mới");


        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!disk.IsOpened)
            {
                disk.OpenStream();
            }
            disk.ReadFatCache();
            fileManagement = new FileManagement(disk);
            root = new FolderModel();
            root._core_disk = disk;


            this.load();

        }

        private void thêmThưMụcMớiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (recycle_bin)
            {
                MessageBox.Show("Không thể tạo từ thùng rác");
                return;
            }
            if (current_selected != null && !(current_selected is FolderModel))
            {

                MessageBox.Show("Vui lòng chọn 1 thư mục từ treeview");
                return;
            }
            string folderName = ShowDialog("Nhập tên thư mục", "Tên thư mục", false);


            fileManagement.CreateFolder((FolderModel)current_selected, folderName);
            MessageBox.Show("Tạo folder mới thành công");
            this.load();
        }

        private void hiệnFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataComponent _current = root;

            DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn hiện file?", "Hiện file", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //do something
                _current = GetCurrentSelectedListView();

                if (_current.HasPassword())
                {
                    string promptValue = ShowDialog("Nhập mật khẩu của file hoặc thư mục", "Mật khẩu");
                    if (OOHashHelper.getString(promptValue) != _current.Password)
                    {
                        MessageBox.Show("Mật khẩu sai");
                        return;
                    }

                }
                _current.Show(disk);

                MessageBox.Show("Hiện file thành công");
                this.load();

            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void xoáPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataComponent _current = root;

            DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn xoá pass ?", "Xoá pass", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //do something
                _current = GetCurrentSelectedListView();

                if (_current.HasPassword())
                {
                    string promptValue = ShowDialog("Nhập mật khẩu của file hoặc thư mục", "Mật khẩu");
                    if (OOHashHelper.getString(promptValue) != _current.Password)
                    {
                        MessageBox.Show("Mật khẩu sai");
                        return;
                    }

                }
                _current.RemovePassword(disk);

                MessageBox.Show("Xoá pass thành công");
                this.load();

            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        /// <summary>
        /// Huỷ ổ cứng, đảo trộn dữ liệu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (disk.IsOpened)
            {
                disk.CloseStream();
            }
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn xoá pass ?", "Xoá pass", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                File.Delete("disk.dat");
                MessageBox.Show("Xoá thành công disk.dat");
            }
            else
            {

            }

        }

        private void importDiskToolStripMenuItem_Click(object sender, EventArgs e)
        {


            var fileContent = string.Empty;
            var filePath = string.Empty;


            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                //openFileDialog.Filter = "";
                //openFileDialog.FilterIndex = 2;
                //openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    DiskManagement oDisk = new DiskManagement();
                    oDisk.OpenStream(filePath);

                    SBootSystem parseBootSystem = oDisk.GetBootSystemData();

                    byte flag_secure = parseBootSystem.FLAG_SECURE_UNIQUE;
                    string unique_key = parseBootSystem.UNIQUE_ID;
                    string user = parseBootSystem.VOL_USER;
                    string pass = parseBootSystem.VOL_PASS;
                    oDisk.CloseStream();

                    if (disk.IsOpened)
                    {
                        disk.CloseStream();
                    }

                    if (System.IO.File.Exists("disk.dat"))
                    {
                        System.IO.File.Delete("disk.dat");
                    }

                    File.Copy(filePath, "disk.dat");

                    if (parseBootSystem.FLAG_SECURE_UNIQUE == 0x01)
                    {

                        using (FileStream diskImportStream  = new FileStream(filePath, FileMode.OpenOrCreate))
                        {
                            using (FileStream _raw_disk = new FileStream("disk.dat", FileMode.OpenOrCreate))
                            {
                                _raw_disk.Seek(32768 * 512, SeekOrigin.Begin);
                                
                                diskImportStream.Seek(32768 * 512, SeekOrigin.Begin);
                                XCryptHelper.CryptStream("6EAFEBFE8CA7AF7FBFF", diskImportStream, _raw_disk, false);//ma hoa
                                MessageBox.Show("Decrypt thành công");

                            }
                        }
                    }
                    else
                    {
                        //copy là đủ rồi
                        MessageBox.Show("Import thành công, không decrypt");

                    }


                }
            }
        }

        private void mãHoáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            string out_name = "out.dat";
            
            disk.OpenStream();
            SBootSystem ssecure = new SBootSystem();
            
            ssecure.UNIQUE_ID = XSecureCore.getUniqueID("C") ?? "";
            if (string.IsNullOrEmpty(ssecure.UNIQUE_ID))
            {
                ssecure.FLAG_SECURE_UNIQUE = 0x00;
            }
            else
            {
                ssecure.FLAG_SECURE_UNIQUE = 0x01;
                ssecure.VOL_USER = "admin";
                ssecure.VOL_PASS = "admin";

            }
            disk.WriteBootSystemData(ssecure);
            disk.CloseStream();

            if (System.IO.File.Exists(out_name))
            {
                System.IO.File.Delete(out_name);
                File.Copy("disk.dat", out_name);
            }


            FileStream in_disk = new FileStream("disk.dat", FileMode.OpenOrCreate);
            in_disk.Seek(32768 * 512, SeekOrigin.Begin);

            FileStream out_disk = new FileStream(out_name, FileMode.OpenOrCreate);
            out_disk.Seek(32768 * 512, SeekOrigin.Begin);


            
            
            XCryptHelper.CryptStream(ssecure.UNIQUE_ID, in_disk, out_disk, true);//ma hoa
            Console.WriteLine(ssecure.UNIQUE_ID);

            in_disk.Close();
            out_disk.Close();
            MessageBox.Show("Output thành công");

        }
    }
}
