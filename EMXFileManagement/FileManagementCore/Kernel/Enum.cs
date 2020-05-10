using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementCore.Kernel
{
    [Flags]
    public enum EntryAttribute : byte
    {
        READ_ONLY = 0x01,
        HIDDEN = 0x02,
        SYSTEM = 0x04,
        VOLUMN_LABEL = 0x08,
        SUB_DIRECTORY= 0x10,
        Archieve = 0x20,
        Device = 0x40,
        Reserved =0x80
    }
}
