using FileManagementCore.Kernel.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementCoreCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            BootSector boot = new BootSector();
            boot.InitBootSector();
        }
    }
}
