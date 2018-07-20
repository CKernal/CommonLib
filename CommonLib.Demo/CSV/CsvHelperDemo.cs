using CommonLib.CSV;
using System;
using System.Data;
using System.Linq;

namespace CommonLib.Demo.CSV
{
    public class CsvHelperDemo
    {
        public static void CsvHelperReadWriteTest()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Restart();
            DataTable dt = CsvHelper.OpenCSV(@"D:\barcode\Fixture1.csv");
            sw.Stop();
            Console.WriteLine(string.Format("OpenCSV: {0}", sw.Elapsed.TotalMilliseconds));

            if (dt != null)
            {
                sw.Restart();
                var barcode = dt.AsEnumerable()
                        .Select(r => r.Field<string>("Barcode"))
                        .ToArray() ;
                sw.Stop();
                Console.WriteLine(string.Format("Select: {0}", sw.Elapsed.TotalMilliseconds));

                sw.Restart();
                CsvHelper.SaveCSV(@"D:\barcode\Fixture11111111.csv", dt);
                sw.Stop();
                Console.WriteLine(string.Format("SaveCSV: {0}", sw.Elapsed.TotalMilliseconds));
            }
        }
    }
}
