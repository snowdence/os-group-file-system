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
using FileManagementCore.Helper;

namespace EMXFileManagement.Module
{
    public partial class ucPropertyFrm : UserControl
    {
        public DataComponent Model { get; set; }
        public DataComponent Root { get; set; }


        public string LocationPath { get; set; }

        public string FileName
        {
            get { return txtName.Text ?? "error"; }
        }
        public bool CheckHidden
        {
            get { return this.checkHidden.Checked; }
        }



        public ucPropertyFrm()
        {
            InitializeComponent();
        }
        public void Init()
        {

            txtName.Text = Model.ToString();

            lblType.Text = (Model is FolderModel) ? "Folder" : "File";
            lblPath.Text = LocationPath;
            lblSize.Text = Model.DataSize().ToString() + " bytes";
            lblFirstCluster.Text = Model.First_cluster.ToString();
            lblCreatedTime.Text = Model.Created_datetime.ToString();
            checkHidden.Checked = Model.IsHidden;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string oldpass = txtOldPass.Text;
            string newpass = txtNewPass.Text;
            string repass = txtNewRepass.Text;
            if (Model.HasPassword())
            {
                if (OOHashHelper.getString(oldpass) != Model.Password)
                {
                    MessageBox.Show("Mật khẩu cũ sai");
                    return;
                }
            }
            if (newpass != repass)
            {
                MessageBox.Show("Vui lòng nhập mật khẩu xác nhận lại chính xác");
                return;
            }
            Model.Password = OOHashHelper.getString(newpass);
            MessageBox.Show("Thay đổi pass thành công");
        }
    }
}
