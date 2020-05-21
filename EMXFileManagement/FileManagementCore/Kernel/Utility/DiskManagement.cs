using FileManagementCore.Helper;
using FileManagementCore.Kernel.Structure;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace FileManagementCore.Kernel.Utility
{
    public class DiskManagement
    {
        int cluster_size_in_bytes = 4096;  //8 sector, 512 per sector
        int boot_system_offset = 512;
        int boot_sector_size = 3282 * 512;
        int fat1_pos = 3282 * 512; //1680384
        int fat2_pos = 18025 * 512; //9228800   
        int rdet_pos = 32768 * 512; // 16777216
        int data_pos = (32768 + 8) * 512; //16781312

        int EOF = 268435455;
        private FileStream _file_stream;
        private SFileAllocationTable _fat_cache;
        private FileAllocationTable _fileAllocationTable;

        private SRDET _rdet_cache;
        private RDET _rdetTable;
        private bool _volumn_opened = false;
        public bool IsOpened
        {
            get { return _volumn_opened; }
        }

        /// <summary>
        /// Mở file chứa volumn
        /// </summary>
        /// <param name="path_disk">Tên file dữ liệu volumn</param>
        public void OpenStream(string path_disk = "disk.dat")
        {
            _file_stream = new FileStream(path_disk, FileMode.OpenOrCreate);
            // this.ReadFatCache();
            this._volumn_opened = true;
        }
        /// <summary>
        /// Đóng file chứa volumn, tránh việc crash 
        /// </summary>
        public void CloseStream()
        {
            //_file_stream.Dispose();
            this._volumn_opened = false;

            _file_stream.Close();
        }
        public DiskManagement()
        {


        }
        ~DiskManagement()
        {
            if (this.IsOpened)
            {
                _file_stream.Close();
            }
        }


        public SBootSystem GetBootSystemData()
        {
            _file_stream.Seek(boot_system_offset, SeekOrigin.Begin);
            SBootSystem boot_system = (SBootSystem)FileIOHelper.Read(_file_stream, typeof(SBootSystem));
            return boot_system;
        }
        public bool IsSecure()
        {
            SBootSystem boot_system_data = this.GetBootSystemData();
            return boot_system_data.FLAG_SECURE_UNIQUE == 0x01;
        }
        

        #region CREATE_VOLUMN
        /// <summary>
        /// Tạo vùng boot sector
        /// </summary>
        /// <param name="total_sector"></param>
        /// <param name="sector_per_fat"></param>
        public void CreateBootSector(string unique = "", string user = "", string pass = "", string hash = "", int total_sector = 15128576, int sector_per_fat = 14743)
        {
            SBootSector sb = new SBootSector();
            sb.jmp = new byte[] { 0x00, 0, 0 };
            sb.OEM_ID = "EMX1.0";

            sb.BIOS_PARAM = new SBiosParamBootSector()
            {
                BYTE_PER_SECTOR = new byte[] { 0x00, 0x02 },
                SECTORS_PER_CLUSTER = 0x08,
                REVERSE_SECTORS = new byte[] { 0xD2, 0x0C },
                NUM_OF_FAT = 0x02,
                NO_USE = new byte[9],
                NUMBER_OF_HEAD = new byte[2] { 0xff, 0x00 },
                HIDDEN_SECTOR = new byte[4] { 0x00, 0x08, 0x00, 0x00 },
                TOTAL_SECTOR = BitConverter.GetBytes(total_sector), //
                SECTOR_PER_FAT = BitConverter.GetBytes(sector_per_fat),  //new byte[4] { 0x97, 0x39, 0x00, 0x00 }, //14743
                VERSION = new byte[4] { 0x00, 0x12, 0x01, 0x22 },
                ROOT_CLUSTER = new byte[4] { 0x02, 0x00, 0x00, 0x00 },
                SYS_INFO = new byte[2] { 0x01, 0x00 },
                BACKUP_BOOT_SECTOR = new byte[2] { 0x06, 0x00 },
                UNUSED_2 = new byte[12]
            };

            sb.EXTENDED_BIOS_PARAM = new SExtendBiosBootSector()
            {
                PHYSIC_DRIVE = 128,
                REVERSE = 0,
                SIGNATURE = 0x29,
                SERIAL_NUM = new byte[4] { 0x23, 0x24, 0x25, 0x26 },
                VOLUMN_NAME = "OCUN0G1",
                FILE_SYSTEM = "EMXFAT"
            };


            sb.BOOTSTRAP_CODE = new byte[420];
            sb.SIGNATURE = new byte[2] { 0x55, 0xAA };
            //short num = BitConverter.ToInt16(sb.BIOS_PARAM.BYTE_PER_SECTOR, 0);
            SBootSystem bootSystem = new SBootSystem();
            if (string.IsNullOrWhiteSpace(unique))
            {
                bootSystem.FLAG_SECURE_UNIQUE = 0x00;
            }
            else
            {
                bootSystem.FLAG_SECURE_UNIQUE = 0x01; //secure
                bootSystem.UNIQUE_ID = unique;
                bootSystem.VOL_USER = user;
                bootSystem.VOL_PASS = pass;
                bootSystem.VOL_HASH_REVERSE = hash;
            }


            _file_stream.Seek(0, SeekOrigin.Begin);
            FileIOHelper.Write(_file_stream, sb);
            FileIOHelper.Write(_file_stream, bootSystem);
            //total is 1 + 3281= 3282 sector in head 
            Console.WriteLine("Write Bootsector .... 3282 sector {0} bytes", 3282 * 512);

        }



        /// <summary>
        /// TẠo vùng FAT
        /// </summary>
        /// <param name="num_cluster"></param>
        public void CreateFAT(int num_cluster = 1887104)
        {
            SFileAllocationTable fat1 = new SFileAllocationTable();
            fat1.FAT_ENTRY = new uint[num_cluster];
            fat1.FAT_ENTRY[0] = BitConverter.ToUInt32(new byte[] { 0xF0, 0xFF, 0xFF, 0x0F }, 0);
            fat1.FAT_ENTRY[1] = BitConverter.ToUInt32(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, 0);
            fat1.FAT_ENTRY[2] = BitConverter.ToUInt32(new byte[] { 0xFF, 0xFF, 0xFF, 0x0F }, 0); //RDET
            FileIOHelper.Write(_file_stream, fat1);
            FileIOHelper.Write(_file_stream, fat1);
            Console.WriteLine("Written 2 FAT : 1887104 *4 *2 bytes");
        }
        /// <summary>
        /// Tạo vùng RDET 
        /// </summary>
        public void CreateRDET()
        {
            //cluster 2
            SRDET srdet_first_default_block = new SRDET();
            srdet_first_default_block.entries = new SRDETEntry[128];
            srdet_first_default_block.entries[0] = new SRDETEntry()
            {
                FLAG = 0x01,
                FILE_NAME = "VOLUMN_NAME",
                FILE_EXT = "noop",
                FILE_ATTRIBUTE = (byte)EntryAttribute.VOLUMN_LABEL,
                REVERSED = 0x02,
                DMSTIME = 0x02,
                CREATED_DATETIME = BitConverter.GetBytes(DateTimeHelper.ToDOSDateTimeInt(DateTime.Now)), //00
                LAST_ACCESS_DATE = BitConverter.GetBytes(DateTimeHelper.ToDOSDateTimeInt(DateTime.Now)),

                MODIFIED_DATETIME = BitConverter.GetBytes(DateTimeHelper.ToDOSDateTimeInt(DateTime.Now)),
                FIRST_CLUSTER_LOW_WORD = new byte[4],
                FILE_SIZE = new byte[4],
                PASSWORD = ""
            };
            FileIOHelper.Write(_file_stream, srdet_first_default_block);
        }
        /// <summary>
        /// Tạo volumn sẽ tạo  3 vùng Bootsector , FAT, RDET
        /// </summary>
        public void CreateVolumn(string unique = "", string user = "", string pass = "", string hash = "")
        {
            int size_mb = 7387; //megabytes

            CreateBootSector(unique, user, pass, hash); //3282

            CreateFAT(); //32768

            CreateRDET(); // cluster 8 


            //create boot

            //create another system part

            //create FaT1

            //create fat2

            //create RDET
            //FF FF FF 0F FF FF FF 0F FF FF FF 0F 

        }

        #endregion

        #region CALC_OFFSET
        /// <summary>
        /// Tính toán cluster n là sector bao nhiêu
        /// </summary>
        /// <param name="cluster_n">Cluster n</param>
        /// <returns>Sector</returns>
        public int GetSectorOffsetFromCluster(int cluster_n)
        {
            int rslt = 32768 + (cluster_n - 2) * 8;
            return rslt;
        }
        /// <summary>
        /// Tính toán offset từ sector. 1 sector có 512 bytes
        /// </summary>
        /// <param name="sector">sector thứ n</param>
        /// <returns>Vị trí OFFSET</returns>
        public long CalcMoveOffsetPointerStream(int sector)
        {
            return sector * 512L;
        }
        /// <summary>
        /// Tính toán Offset từ cluster n
        /// </summary>
        /// <param name="cluster_n">Cluster n</param>
        /// <returns>Offset đầu của cluster n </returns>
        public long CalcMoveOffsetClusterPointerStream(int cluster_n)
        {
            return GetSectorOffsetFromCluster(cluster_n) * 512L;
        }

        #endregion


        /// <summary>
        /// Cập nhật entry RDET hoặc SDET
        /// </summary>
        /// <param name="entry">vùng RDET hoặc SDET</param>
        /// <param name="cluster">Cluster chứa RDET hoặc SDET</param>
        public void UpdateEntry(SRDETEntry entry, int cluster = 2)
        {
            this.ReadRDETCache(cluster);
            for (int i = 0; i < _rdet_cache.entries.Count(); i++)
            {
                if (_rdet_cache.entries[i].FILE_NAME == entry.FILE_NAME && _rdet_cache.entries[i].FILE_EXT == entry.FILE_EXT)
                {
                    _rdet_cache.entries[i] = entry;
                }

            }


            _file_stream.Seek(CalcMoveOffsetClusterPointerStream(cluster), SeekOrigin.Begin);
            FileIOHelper.Write(_file_stream, _rdet_cache);
        }
        public void RemoveEntry(SRDETEntry entry, int cluster = 2)
        {
            this.ReadRDETCache(cluster);
            for (int i = 0; i < _rdet_cache.entries.Count(); i++)
            {
                if (_rdet_cache.entries[i].FILE_NAME == entry.FILE_NAME && _rdet_cache.entries[i].FILE_EXT == entry.FILE_EXT)
                {
                    _rdet_cache.entries[i] = new SRDETEntry();
                }

            }


            _file_stream.Seek(CalcMoveOffsetClusterPointerStream(cluster), SeekOrigin.Begin);
            FileIOHelper.Write(_file_stream, _rdet_cache);
        }
        /// <summary>
        /// Thêm mới entry vào vị trí trống
        /// </summary>
        /// <param name="entry">vùng RDET hoặc SDET</param>
        /// <param name="cluster">Cluster chứa RDET hoặc SDET</param>
        public void WriteNewEntry(SRDETEntry entry, int cluster = 2)
        {
            this.ReadRDETCache(cluster);
            int _first_idx = _rdetTable.GetEmptyEntry();
            _rdet_cache.entries[_first_idx] = entry;
            _file_stream.Seek(CalcMoveOffsetClusterPointerStream(cluster), SeekOrigin.Begin);
            FileIOHelper.Write(_file_stream, _rdet_cache);
        }

        /// <summary>
        /// Viết toàn bộ vùng RDET bằng struct
        /// </summary>
        /// <param name="entry">vùng RDET hoặc SDET</param>
        /// <param name="cluster">Cluster chứa RDET hoặc SDET</param>
        public void WriteSDETCluster(SRDET sdet, int cluster)
        {
            _file_stream.Seek(CalcMoveOffsetClusterPointerStream(cluster), SeekOrigin.Begin);
            FileIOHelper.Write(_file_stream, sdet);
        }


        /// <summary>
        /// Đọc RDET hoặc SDET và lưu vào _rdet_cache và _rdetTable để quản lý cập nhật dễ dàng
        /// </summary>
        /// <param name="cluster">Cluster chứa RDET hoặc SDET</param>
        /// <returns>SRDET struct</returns>
        public SRDET ReadRDETCache(int cluster = 2)
        {
            if (IsOpened)
            {
                _file_stream.Seek(CalcMoveOffsetClusterPointerStream(cluster), SeekOrigin.Begin);
                _rdet_cache = (SRDET)FileIOHelper.Read(_file_stream, typeof(SRDET));
                _rdetTable = new RDET(_rdet_cache);
                return this._rdet_cache;
            }
            return default(SRDET);
        }
        /// <summary>
        /// Đọc FAT và lưu vào biến (cache) để quản lý.
        /// </summary>
        public void ReadFatCache()
        {
            _file_stream.Seek(fat1_pos, SeekOrigin.Begin);
            //read ~~fa~~t
            _fat_cache = (SFileAllocationTable)FileIOHelper.Read(_file_stream, typeof(SFileAllocationTable));
            _fileAllocationTable = new FileAllocationTable(_fat_cache);

        }
        /// <summary>
        /// Lấy value từ cache đã đọc
        /// </summary>
        /// <param name="n">entry thứ n (cluster n)</param>
        /// <returns>Giá trị chứa trong entry đó (uint)</returns>
        public uint ReadFatEntry(int n)
        {
//           this.ReadFatCache();// refresh fat
            return _fat_cache.FAT_ENTRY[n];
        }

        /// <summary>
        /// Lấy cluster trống tiếp theo của FAT, đồng thời đánh dấu đã chứa dữ liệu để tránh file sau ghi đè
        /// </summary>
        /// <returns>Cluster còn trống dựa trên bảng FAT</returns>
        public int GetNextClusterEmpty()
        {
            return this._fileAllocationTable.GetNextClusterEmpty();
        }

        /// <summary>
        /// Ghi EOF  vào entry FAT thứ n (cluster n)
        /// </summary>
        /// <param name="cluster">cluster cần đánh dấu trong fat</param>
        public void WriteBlockFileWrittenFat(int cluster)
        {
            _file_stream.Seek(fat1_pos, SeekOrigin.Begin);
            _fileAllocationTable.SetFatEntry(BitConverter.ToUInt32(new byte[4] { 0xFF, 0xFF, 0xFF, 0x0F }, 0), cluster);
            FileIOHelper.Write(_file_stream, _fat_cache);
        }

        public void WriteNULLfat(List<uint> list_cluster )
        {
            _file_stream.Seek(fat1_pos, SeekOrigin.Begin);

            foreach (int item in list_cluster)
            {
                _fileAllocationTable.SetFatEntry(BitConverter.ToUInt32(new byte[4] { 0x00, 0x00, 0x00, 0x00 }, 0), item);
            }
            FileIOHelper.Write(_file_stream, _fat_cache);

        }
        /// <summary>
        /// Đánh dấu danh sách cluster đã chiếm chỗ 
        /// </summary>
        /// <param name="list_cluster">Danh sách cluster đã chiếm theo thứ tự theo link list. Cuối cùng đánh dấu EOF</param>
        public void WriteBlockFileWrittenFat(List<int> list_cluster)
        {
            for (int i = 0; i < list_cluster.Count; i++)
            {
                if (i == list_cluster.Count - 1)
                {
                    _fileAllocationTable.SetFatEntry(BitConverter.ToUInt32(new byte[4] { 0xFF, 0xFF, 0xFF, 0x0F }, 0), list_cluster[i]);
                }
                else
                {
                    _fileAllocationTable.SetFatEntry(Convert.ToUInt32(list_cluster[i + 1]), list_cluster[i]);
                }
            }

            _file_stream.Seek(fat1_pos, SeekOrigin.Begin);

            FileIOHelper.Write(_file_stream, _fat_cache);
            FileIOHelper.Write(_file_stream, _fat_cache);//fat 2
        }

        /// <summary>
        /// Ghi dữ liệu vào cluster n
        /// </summary>
        /// <param name="buffer">Buffer </param>
        /// <param name="length">kích thước dữ liệu</param>
        /// <param name="cluster_n">cluster n</param>
        public void WriteBlockData(byte[] buffer, int length, int cluster_n)
        {
            SCluster cluster = new SCluster();
            _file_stream.Seek(CalcMoveOffsetClusterPointerStream(cluster_n), SeekOrigin.Begin);
            cluster.data = buffer;
            FileIOHelper.Write(_file_stream, cluster);
        }

        /// <summary>
        /// Đọc đữ liệu ở cluster n
        /// </summary>
        /// <param name="n">Cluster n</param>
        /// <returns>SCluster là struct chứa dữ liệu 4096 bytes</returns>
        public SCluster ReadBlockData(int n)
        {
            SCluster cluster = new SCluster();
            _file_stream.Seek(CalcMoveOffsetClusterPointerStream(n), SeekOrigin.Begin);

            cluster = (SCluster)FileIOHelper.Read(_file_stream, typeof(SCluster));
            return cluster;
        }

        /// <summary>
        /// Ghi dữ liệu vào disk tự động tìm các cluster trống và đánh dấu
        /// </summary>
        /// <param name="buffer">buffer dữ liệu</param>
        /// <param name="length">kích thước dữ liệu</param>
        /// <returns>Danh sách cluster đã lưu theo thứ tự</returns>
        public List<int> WriteBlockData(byte[] buffer, int length)
        {
            //length 8888  
            int remain_read = length;
            byte[] block_fixed = new byte[4096];
            int byte_read_epoch = 4096;
            int epoch = 0;
            int pos = 0;

            int num_need_cluster = (int)Math.Ceiling(length / 4096.00);
            if (num_need_cluster == 0)
            {
                num_need_cluster += 1;
            }
            //8888 ~ 2 cluster 
            List<int> list_cluster_need = _fileAllocationTable.GetListNextClusterEmpty(num_need_cluster); // lay n cluster

            while (remain_read > 0)
            {
                // 0: 0 * 4096;
                // 1: 1 * 4096;
                // 2: 2 * 4096;
                pos = epoch * byte_read_epoch;
                if (remain_read < byte_read_epoch)
                {
                    block_fixed = new byte[4096];
                    byte_read_epoch = remain_read;
                }
                Array.Copy(buffer, pos, block_fixed, 0, byte_read_epoch);
                WriteBlockData(block_fixed, 4096, list_cluster_need[epoch]);

                remain_read -= byte_read_epoch;
                epoch += 1;
            }
            WriteBlockFileWrittenFat(list_cluster_need);
            return list_cluster_need;
        }



        //RDET import
        /// <summary>
        /// Nhập file test
        /// </summary>
        public void ImportFile()
        {
            _file_stream.Seek(fat1_pos, SeekOrigin.Begin);
            //read ~~fa~~t
            SFileAllocationTable fat_cache = (SFileAllocationTable)FileIOHelper.Read(_file_stream, typeof(SFileAllocationTable));

            FileAllocationTable fileAllocationTable = new FileAllocationTable(fat_cache);

            int free_cluster = -1;
            long size = FileIOHelper.SizeofFile(@"teptin.txt"); // byte

            int need_clusters = (int)Math.Ceiling(size / 4096.00);

            SCluster cluster = new SCluster();
            List<int> list_write = new List<int>();
            using (FileStream fileStream = new FileStream(@"teptin.txt", FileMode.OpenOrCreate))
            {
                //Cluster temp = (Cluster)FileIOHelper.Read(fileStream, typeof(Cluster));
                byte[] block = new byte[4096];
                while (fileStream.Read(block, 0, 4096) > 0)
                {  //as long as this does not return 0, the data in the file hasn't been completely read          
                   //Print/do anything you want with [block], your 16 bytes data are there

                    free_cluster = fileAllocationTable.GetNextClusterEmpty();
                    _file_stream.Seek(CalcMoveOffsetClusterPointerStream(free_cluster), SeekOrigin.Begin);

                    cluster.data = block;
                    FileIOHelper.Write(_file_stream, cluster);
                    list_write.Add(free_cluster);
                    block = new byte[4096];
                    //  MessageBox.Show("Add " + free_cluster.ToString()); 
                }
            }
            //List write 3,4,5
            // Edit FAT
            for (int i = 0; i < list_write.Count; i++)
            {
                if (i == list_write.Count - 1)
                {
                    fileAllocationTable.SetFatEntry(BitConverter.ToUInt32(new byte[4] { 0xFF, 0xFF, 0xFF, 0x0F }, 0), list_write[i]);
                }
                else
                {
                    fileAllocationTable.SetFatEntry(Convert.ToUInt32(list_write[i + 1]), list_write[i]);
                }
            }

            _file_stream.Seek(fat1_pos, SeekOrigin.Begin);

            FileIOHelper.Write(_file_stream, fat_cache);
            FileIOHelper.Write(_file_stream, fat_cache);//fat 2





            _file_stream.Seek(rdet_pos, SeekOrigin.Begin);
            SRDET rdet_cache = (SRDET)FileIOHelper.Read(_file_stream, typeof(SRDET));
            RDET rdet = new RDET(rdet_cache);
            int index_empty_entry = rdet.GetEmptyEntry();


            uint file_size = Convert.ToUInt32(size);
            //file 1 cluster abcdef

            SRDETEntry entry = new SRDETEntry()
            {
                FLAG = 0x02, //file
                FILE_NAME = "file",
                FILE_EXT = "txt",
                FILE_ATTRIBUTE = (byte)EntryAttribute.Archieve,
                REVERSED = 0x02,
                DMSTIME = 0x02,
                CREATED_DATETIME = new byte[4], //00
                LAST_ACCESS_DATE = BitConverter.GetBytes(DateTimeHelper.ToDOSDate(DateTime.Now)),
                MODIFIED_DATETIME = new byte[4],
                FIRST_CLUSTER_LOW_WORD = BitConverter.GetBytes(list_write[0]),
                FILE_SIZE = BitConverter.GetBytes(file_size),
                PASSWORD = ""
            };

            rdet_cache.entries[index_empty_entry] = entry;
            _file_stream.Seek(rdet_pos + 64 * index_empty_entry, SeekOrigin.Begin);

            //update entry

            FileIOHelper.Write(_file_stream, entry);

        }





    }
}
