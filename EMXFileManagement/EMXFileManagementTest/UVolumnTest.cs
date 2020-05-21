using System;
using FileManagementCore.Kernel.Structure;
using FileManagementCore.Kernel.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EMXFileManagementTest
{
    [TestClass]
    public class UVolumnTest
    {
        DiskManagement disk = new DiskManagement();
        [TestMethod]
        public void TEST_CREATE_VOLUMN_SECURE()
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
            disk.CreateVolumn("unique", "admin","pass", "hash" );
            SBootSystem boot_sys = disk.GetBootSystemData();
            Assert.IsTrue(disk.IsSecure());
            disk.CloseStream();
        }
    }
}
