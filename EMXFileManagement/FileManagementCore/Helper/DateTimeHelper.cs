using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementCore.Helper
{
    public static class DateTimeHelper
    {
        // WORD  ushort 16 bit
        //LPLIFETIME uint64
        [DllImport("kernel32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int DosDateTimeToFileTime(ushort dateValue, ushort timeValue, out UInt64 fileTime);



        //https://bytes.com/topic/net/answers/661377-c-program-convert-2-byte-time-2-byte-date-datetime

        //Thai 
        public static int ToDOSDateTimeInt16(DateTime dateTime, ref Int16 dateVal, ref Int16 timeVal)
        {
            int DosDate1 = ToDOSDateTimeInt(dateTime);

            var date = (DosDate1 & 0xFFFF0000) >> 16;
            var time = (DosDate1 & 0x0000FFFF);
            dateVal = Convert.ToInt16(time);
            timeVal = Convert.ToInt16(date);
            return 0; 
        }
        public static int ToDOSDateTimeInt(DateTime dateTime)
        {
            var years = dateTime.Year - 1980;
            var months = dateTime.Month;
            var days = dateTime.Day;
            var hours = dateTime.Hour;
            var minutes = dateTime.Minute;
            var seconds = dateTime.Second;

            var date = (years << 9) | (months << 5) | days;
            var time = (hours << 11) | (minutes << 5) | (seconds >> 1);

            return (date << 16) | time;
        }

        public static DateTime GetDateTimeFromDosDateTime(Int32 i32TimeDate)

        {

            Int16 i16Time = (Int16)(i32TimeDate & 0xFFFF);

            Int16 i16Date = (Int16)((i32TimeDate & 0xFFFF0000) >> 16);

            return GetDateTimeFromDosDateTime(i16Time, i16Date);

        }

        //https://stackoverflow.com/questions/15744647/converting-date-to-dos-date
        public static ushort ToDOSDate(this DateTime dateTime)
        {
            uint day = (uint)dateTime.Day;              // Between 1 and 31
            uint month = (uint)dateTime.Month;          // Between 1 and 12
            uint years = (uint)(dateTime.Year - 1980);  // From 1980

            if (years > 127)
                throw new ArgumentOutOfRangeException("Cannot represent the year.");

            uint dosDateTime = 0;
            dosDateTime |= day << (16 - 16);
            dosDateTime |= month << (21 - 16);
            dosDateTime |= years << (25 - 16);

            return unchecked((ushort)dosDateTime);
        }

        public static DateTime GetDateTimeFromDosDateTime(Int16 i16Time, Int16 i16Date)

        {

            //date : 10101001
            //       11111111111111

            int iYear = 0;

            int iMonth = 1;

            int iDay = 1;

            int iHour = 0;

            int iMinute = 0;

            int iSecond = 0;

            iDay = (i16Date & 0x1F);

            iMonth = ((i16Date & 0x01E0) >> 5);

            iYear = 1980 + ((i16Date & 0xFE00) >> 9);

            iSecond = (i16Time & 0x1F) * 2;

            iMinute = ((i16Time & 0x07E0) >> 5);

            iHour = ((i16Time & 0x0F800) >> 11);

            return new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond);

        }

        public static Int32 ByteToInt32(byte byte0, byte byte1, byte byte2, byte byte3)

        {

            return byte0 + byte1 * 256 + byte2 * 256 * 256 + byte3 * 256 * 256 * 256;

        }

        public static Int32 ByteToInt32(byte[] convertMe)

        {

            return convertMe[0] + convertMe[1] * 256 + convertMe[2] * 256 * 256 + convertMe[3] * 256 * 256 * 256;

        }

        public static Int16 ByteToInt16(byte[] convertMe)

        {

            return (short)(convertMe[0] + convertMe[1] * 256);

        }

        public static Int16 ByteToInt16(byte byte0, byte byte1)

        {

            return (short)(byte0 + byte1 * 256);

        }
    }
}
