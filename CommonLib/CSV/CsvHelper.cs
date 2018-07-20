using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonLib.CSV
{
    public class CsvHelper
    {
        public static DataTable OpenCSV(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return null;
            }

            using (StreamReader sr = new StreamReader(fileInfo.FullName, Encoding.Default))
            {
                DataTable dt = new DataTable();

                string textAll = sr.ReadToEnd();
                string[] textLine = textAll.Split(Environment.NewLine.ToCharArray());
                textLine = textLine
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                bool IsFirst = true;
                foreach (var line in textLine)
                {
                    string[] lineSplit = line.Split(',');

                    if (IsFirst)
                    {
                        IsFirst = false;
                        for (int i = 0; i < lineSplit.Length; i++)
                        {
                            dt.Columns.Add(lineSplit[i]);
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < lineSplit.Length; i++)
                        {
                            dr[i] = lineSplit[i];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                return dt;
            }
        }

        public static void SaveCSV(string filePath, DataTable dt)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            using (StreamWriter sw = new StreamWriter(fi.FullName, false, Encoding.Default))
            {
                StringBuilder strBuilder = new StringBuilder();

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    strBuilder.Append(dt.Columns[i].ColumnName);

                    if (i < dt.Columns.Count - 1)
                    {
                        strBuilder.Append(",");
                    }
                }
                sw.WriteLine(strBuilder.ToString());

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strBuilder.Clear();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        strBuilder.Append(dt.Rows[i][j].ToString());
                        if (j < dt.Columns.Count - 1)
                        {
                            strBuilder.Append(",");
                        }
                    }
                    sw.WriteLine(strBuilder.ToString());
                }
            }
        }
    }
}
