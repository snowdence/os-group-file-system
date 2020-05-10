          aa a = new aa();
            a.FAT_ENTRY = new byte[12] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12 };

            int size = Marshal.SizeOf(a);

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(a, ptr, true);
            byte[] arr = new byte[8];
            int n = 1;
            Marshal.Copy(ptr + n *4, arr,  0, 4);
            Marshal.FreeHGlobal(ptr);

             aa a = new aa();
            a.FAT_ENTRY = new byte[12] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12 };

            int size = Marshal.SizeOf(a);

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(a, ptr, true);
            byte[] arr = new byte[4] { 0x1A, 0x1B,0x1C, 0x1D};
            int n = 1;
            Array.Copy(arr, 0, a.FAT_ENTRY, 4, 4);
            //Marshal.WriteIntPtr(ptr, 0 * 4,     ((IntPtr)(i + 1)));
            Marshal.FreeHGlobal(ptr);
\
\
 byte[] bTimeDate = { 0x36, 0x47, 0xA9, 0x50 };

            byte[] bTime = { 0x36, 0x47 };

            byte[] bDate = { 0xA9, 0x50 };
            DateTimeHelper helper = new DateTimeHelper();
            MessageBox.Show(DateTimeHelper.GetDateTimeFromDosDateTime(DateTimeHelper.ByteToInt16(bTime), DateTimeHelper.ByteToInt16(bDate)).ToString());

            MessageBox.Show(DateTimeHelper.GetDateTimeFromDosDateTime(DateTimeHelper.ByteToInt32(bTimeDate)).ToString());


##Deepcopy
            struct Test : ICloneable
        {
            public int[] arr;


            public object Clone()
            {
                Test other = (Test)this.MemberwiseClone();
                other.arr = arr.ToArray();
                return other;
            }
        }

   
    SRDET rDET = new SRDET();
            rDET.entries = new SRDETEntry[3];
            Test a1 = new Test();
            a1.arr = new int[] { 1, 2, 3, 4 };
            Test a2 = (Test)a1.Clone();
            a2.arr[0] = 0;



#Thai revert

using System;
					
public class Program
{
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

public static int ToDOSDate(DateTime dateTime)
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
	public static void Main()
	{
		DateTime re1 = GetDateTimeFromDosDateTime(BitConverter.ToInt16( new byte[] { 0x11, 0x38},0), BitConverter.ToInt16( new byte[] { 0x1F, 0x3D},0));
		Console.WriteLine(re1.ToString());
		
		int DosDate1 = ToDOSDate(re1);
		var date1 = (DosDate1 & 0xFFFF0000) >> 16;
    	var time1 = (DosDate1 & 0x0000FFFF);
		DateTime re2 = GetDateTimeFromDosDateTime(Convert.ToInt16(time1), Convert.ToInt16(date1));
		Console.WriteLine(re2.ToString());
		
		DateTime res1 = GetDateTimeFromDosDateTime(BitConverter.ToInt16( new byte[] { 0x36, 0x11},0), BitConverter.ToInt16( new byte[] { 0x27, 0x4C},0));
		Console.WriteLine(res1.ToString());
		
		int DosDate2 = ToDOSDate(res1);
		var date2 = (DosDate2 & 0xFFFF0000) >> 16;
    	var time2 = (DosDate2 & 0x0000FFFF);
		DateTime res2 = GetDateTimeFromDosDateTime(Convert.ToInt16(time2), Convert.ToInt16(date2));
		Console.WriteLine(res2.ToString());
		
		DateTime r1 = GetDateTimeFromDosDateTime(BitConverter.ToInt16( new byte[] { 0xAB, 0x23},0), BitConverter.ToInt16( new byte[] { 0xED, 0x10},0));
		Console.WriteLine(r1.ToString());
		
		int DosDate3 = ToDOSDate(r1);
		var date3 = (DosDate3 & 0xFFFF0000) >> 16;
    	var time3 = (DosDate3 & 0x0000FFFF);
		DateTime r2 = GetDateTimeFromDosDateTime(Convert.ToInt16(time3), Convert.ToInt16(date3));
		Console.WriteLine(r2.ToString());
	}
}