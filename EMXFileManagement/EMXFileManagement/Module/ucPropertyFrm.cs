using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileManagementCore.Kernel.Structure;
using System.Runtime.CompilerServices;

namespace EMXFileManagement.Module
{
    public partial class ucPropertyFrm : UserControl
    {
        public DataComponent Model { get; set;  }
        public DataComponent Root { get; set; }

        public string LocationPath { get; set;  }

        public string FileName { 
            get { return txtName.Text; }
        }
        
        public ucPropertyFrm()
        {
            InitializeComponent();
        }
        public void Init()
        {
            txtName.Text = Model.FileName + Model.FileExt;
            lblType.Text = (Model is FolderModel) ? "Folder" : "File";
            lblPath.Text = LocationPath;
            lblSize.Text = Model.DataSize().ToString() + " bytes";
            lblFirstCluster.Text = Model.First_cluster.ToString();
            lblCreatedTime.Text = Model.Created_datetime.ToString();
            checkHidden.Checked = Model.IsHidden;
        }

        

    }
}
