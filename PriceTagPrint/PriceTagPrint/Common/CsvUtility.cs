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
        //dt:データを入れるDataTable
        //hasHeader:CSVの一行目がカラム名かどうか
        //fileName:ファイル名
        //separator:カラムを分けている文字(,など)
        //quote:カラムを囲んでいる文字("など)
        public DataTable ReadCSV(bool hasHeader, string fileName, string separator = ",", bool quote = false)
        {
            DataTable dt = new DataTable();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //CSVを便利に読み込んでくれるTextFieldParserを使います。
            TextFieldParser parser = new TextFieldParser(fileName, Encoding.GetEncoding("shift_jis"));
            //これは可変長のフィールドでフィールドの区切りのマーカーが使われている場合です。
            //フィールドが固定長の場合は
            //parser.TextFieldType = FieldType.FixedWidth;
            parser.TextFieldType = FieldType.Delimited;
            //区切り文字を設定します。
            parser.SetDelimiters(separator);
            //クォーテーションがあるかどうか。
            //但しダブルクォーテーションにしか対応していません。シングルクォーテーションは認識しません。
            parser.HasFieldsEnclosedInQuotes = quote;
            string[] data;
            //ここのif文では、DataTableに必要なカラムを追加するために最初に1行だけ読み込んでいます。
            //データがあるか確認します。
            if (!parser.EndOfData)
            {
                //CSVファイルから1行読み取ります。
                data = parser.ReadFields();
                //カラムの数を取得します。
                int cols = data.Length;
                if (hasHeader)
                {
                    var duplicates = data.GroupBy(name => name).Where(name => name.Count() > 1)
                                        .Select(group => group.Key).ToList();

                    var seqno = 1;
                    foreach(var dup in duplicates)
                    {
                        var indexes = data.Select((name, index) => new { name, index })
                                        .Where(a => a.name == dup)
                                        .Select(a => a.index);
                        foreach(var idx in indexes)
                        {
                            data[idx] += "_" + seqno.ToString("00");
                            seqno++;
                        }
                        seqno = 1;
                    }
                    
                    for (int i = 0; i < cols; i++)
                    {
                        dt.Columns.Add(new DataColumn(data[i]));
                    }
                }
                else
                {
                    for (int i = 0; i < cols; i++)
                    {
                        //カラム名にダミーを設定します。
                        dt.Columns.Add(new DataColumn());
                    }
                    //DataTableに追加するための新規行を取得します。
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < cols; i++)
                    {
                        //カラムの数だけデータをうつします。
                        row[i] = data[i];
                    }
                    //DataTableに追加します。
                    dt.Rows.Add(row);
                }
            }
            //ここのループがCSVを読み込むメインの処理です。
            //内容は先ほどとほとんど一緒です。
            while (!parser.EndOfData)
            {
                data = parser.ReadFields();
                DataRow row = dt.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    row[i] = data[i];
                }
                dt.Rows.Add(row);
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
