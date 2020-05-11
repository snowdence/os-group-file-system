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
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
            load();
        }
        void load()
        {
            DiskManagement disk = new DiskManagement();
            disk.ReadFatCache();

            FolderManagement folderManagement = new FolderManagement(disk);
            FileManagement fileManagement = new FileManagement(disk);

            FolderModel root = new FolderModel();
            root._core_disk = disk;
            //var objk = root.GetAllInside();
            //objk = ((FolderModel)objk[2]).GetAllInside();
            List<DataComponent> dataComponents  =  root.GetAllInside();
            root.PrintPretty(" ", true);
            Console.WriteLine(" \n\n\n");
            TreeNode tre = root.GetTreeNode();
            treeView1.Nodes.Add(tre);
            
        }
    }
}
