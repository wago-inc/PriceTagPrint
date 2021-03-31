using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace PriceTagPrint.Common
{
    public class CsvUtility
    {
        /// <summary>
        /// CSVを読み込みDataTableとして返す
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="isHeader"></param>
        /// <param name="limit"></param>
        /// <param name="enccodeName"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public DataTable Read(string filename, bool isHeader, long limit = long.MaxValue, string encodeName = "shift-jis", string delimiter = "")
        {


            limit += (isHeader == true && limit < long.MaxValue) ? 1 : 0;
            long count = 0;


            var enc = Encoding.GetEncoding(encodeName);
            delimiter = (delimiter == "") ? GetDelimiter(filename, enc) : delimiter;

            DataTable dt = new DataTable();

            using (TextFieldParser parser = new TextFieldParser(filename, enc) { TextFieldType = FieldType.Delimited })
            {
                parser.Delimiters = new string[] { delimiter };

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();

                    if (count++ == 0)
                    {
                        //ヘッダがある場合、1行目のデータで列を追加
                        dt.Columns.AddRange(fields.Select(i => (isHeader) ? new DataColumn(i) : new DataColumn()).ToArray());
                        if (isHeader)
                        {
                            continue;
                        }
                    }

                    if (fields.Length > dt.Columns.Count)
                    {
                        dt.Columns.AddRange(Enumerable.Range(0, fields.Length - dt.Columns.Count).Select(i => new DataColumn()).ToArray());
                    }

                    if (count > limit)
                    {
                        break;
                    }

                    DataRow dr = dt.NewRow();
                    Enumerable.Range(0, fields.Length).Select(i => dr[i] = fields[i]).ToArray();
                    dt.Rows.Add(dr);
                }
            }

            return dt;

        }

        /// <summary>
        /// DataTableの内容をCSV形式でファイルに出力する
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="filename"></param>
        /// <param name="writeHeader"></param>
        /// <param name="delimiter"></param>
        /// <param name="encodeName"></param>
        /// <param name="isAppend"></param>
        public void Write(DataTable dt, string filename, bool writeHeader, string delimiter = ",", string encodeName = "shift-jis", bool isAppend = false)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            bool header = (writeHeader && (isAppend == false || (isAppend == true && File.Exists(filename) == false)));
            //書き込むファイルを開く
            using (StreamWriter sw = new StreamWriter(filename, isAppend, Encoding.GetEncoding(encodeName)))
            {
                //ヘッダを書き込む
                if (header)
                {
                    string[] headers = dt.Columns.Cast<DataColumn>().Select(i => enclose_ifneed(i.ColumnName)).ToArray();
                    sw.WriteLine(String.Join(delimiter, headers));
                }

                //レコードを書き込む
                foreach (DataRow dr in dt.Rows)
                {
                    string[] fields = Enumerable.Range(0, dt.Columns.Count).Select(i => enclose_ifneed(dr[i].ToString())).ToArray();
                    sw.WriteLine(String.Join(delimiter, fields));
                }
            }

            return;

            /// 必要ならば、文字列をダブルクォートで囲む
            string enclose_ifneed(string p_field)
            {
                //ダブルクォートで括る必要があるかを確認
                if (p_field.Contains('"') || p_field.Contains(',') || p_field.Contains('\r') || p_field.Contains('\n') ||
                     p_field.StartsWith(" ") || p_field.StartsWith("\t") || p_field.EndsWith(" ") || p_field.EndsWith("\t"))
                {
                    //ダブルクォートが含まれていたら２つ重ねて、前後にダブルクォートを付加
                    return (p_field.Contains('"')) ? ("\"" + p_field.Replace("\"", "\"\"") + "\"") : ("\"" + p_field + "\"");
                }
                else
                {
                    //何もせずそのまま返す
                    return p_field;
                }
            }

        }

        /// <summary>
        /// 区切り文字の自動判定
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="encodeName"></param>
        /// <returns></returns>
        public string GetDelimiter(string filename, Encoding encodeName)
        {
            using (StreamReader sr = new StreamReader(filename, encodeName))
            {
                string line = sr.ReadLine();
                return (line.Split(',').Length > line.Split('\t').Length) ? "," : "\t";
            }
        }
    }
}
