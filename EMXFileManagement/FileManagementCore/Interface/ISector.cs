using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementCore.Interface
{
    interface ISector
    {
        IList<byte> ReadSectors(int pos);    
        
        IList<byte> ReadSectors(int start_pos, int num_read_sector); 
        
        
    }
}
