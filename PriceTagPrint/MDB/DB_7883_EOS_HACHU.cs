using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Windows;

namespace PriceTagPrint.MDB
{
    public class DB_7883_EOS_HACHU
    {
        /// <summary>
        /// 発注番号
        /// </summary>
        public int HNO { get; set; }
        /// <summary>
        /// 発注日
        /// </summary>
        public DateTime HATYUBI { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// 分類コード　※以下変換マスターの項目
        /// </summary>
        public int BUNRUI { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string SCODE { get; set; }
        /// <summary>
        /// 社内サイズ・カラーコード
        /// </summary>
        public int SAIZUS { get; set; }
        /// <summary>
        /// サイズ・カラー名（※カクテル）
        /// </summary>
        public string HINNMB { get; set; }
        /// <summary>
        /// 受信日
        /// </summary>
        public string EOS_RCVDT { get; set; }
        /// <summary>
        /// 伝票番号
        /// </summary>
        public string EOS_DENPYONO { get; set; }
        /// <summary>
        /// 発注日
        /// </summary>
        public string EOS_HATYUBI { get; set; }
        /// <summary>
        /// 品番コード
        /// </summary>
        public string EOS_HINBANCD { get; set; }
        /// <summary>
        /// 店舗コード
        /// </summary>
        public string EOS_TENCD { get; set; }
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string EOS_SIRESAKICD { get; set; }
        /// <summary>
        /// 相手先商品コード
        /// </summary>
        public string EOS_SYOHINCD { get; set; }
        /// <summary>
        /// 伝票行番号
        /// </summary>
        public string EOS_DENPYOGYO { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string EOS_SYOHINNM { get; set; }
        /// <summary>
        /// 発注数
        /// </summary>
        public short EOS_HSU { get; set; }
        /// <summary>
        /// 納品予定数
        /// </summary>
        public int EOS_NSU { get; set; }
        /// <summary>
        /// 原単価
        /// </summary>
        public int EOS_GENKA { get; set; }
        /// <summary>
        /// 売単価
        /// </summary>
        public int EOS_BAIKA { get; set; }
        /// <summary>
        /// 売上日
        /// </summary>
        public string EOS_URIAGEBI { get; set; }

        public DB_7883_EOS_HACHU(int hno, DateTime hatyubi, string hincd, int bunrui, string scode, int saizus, string hinnmb, string eos_rcvdt, 
                                 string eos_denpyono, string eos_hatyubi, string eos_hinbancd, string eos_tencd, string eos_siresakicd, string eos_syohincd, 
                                 string eos_denpyogyo, string eos_syohinnm, short eos_hsu, int eos_nsu, int eos_genka, int eos_baika, string eos_uriagebi)
        {
            this.HNO = hno;
            this.HATYUBI = hatyubi;
            this.HINCD = hincd;
            this.BUNRUI = bunrui;
            this.SCODE = scode;
            this.SAIZUS = saizus;
            this.HINNMB = hinnmb;
            this.EOS_RCVDT = eos_rcvdt;
            this.EOS_DENPYONO = eos_denpyono;
            this.EOS_HATYUBI = eos_hatyubi;
            this.EOS_HINBANCD = eos_hinbancd;
            this.EOS_TENCD = eos_tencd;
            this.EOS_SIRESAKICD = eos_siresakicd;
            this.EOS_SYOHINCD = eos_syohincd;
            this.EOS_DENPYOGYO = eos_denpyogyo;
            this.EOS_SYOHINNM = eos_syohinnm;
            this.EOS_HSU = eos_hsu;
            this.EOS_NSU = eos_nsu;
            this.EOS_GENKA = eos_genka;
            this.EOS_BAIKA = eos_baika;
            this.EOS_URIAGEBI = eos_uriagebi;
        }
    }

    public class DB_7883_EOS_HACHU_LIST
    {
        public List<DB_7883_EOS_HACHU> QueryWhereDate(DateTime hatyubi)
        {
            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " 7883_EOS_HACHU " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " HATYUBI = #" + hatyubi.ToShortDateString() + "#;";

            DataTable mdbDt = new DataTable();
            var results = new List<DB_7883_EOS_HACHU>();
            // 読み込み
            try
            {
                OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString);
                OdbcCommand sqlCommand = new OdbcCommand(sql, mdbConn);
                sqlCommand.CommandTimeout = 30;

                OdbcDataAdapter adapter = new OdbcDataAdapter(sqlCommand);

                adapter.Fill(mdbDt);
                adapter.Dispose();
                sqlCommand.Dispose();

                foreach (DataRow dr in mdbDt.Rows)
                {
                    results.Add(new DB_7883_EOS_HACHU
                        (
                            dr.Field<int>("HNO"),
                            dr.Field<DateTime>("HATYUBI"),
                            dr.Field<string>("HINCD"),
                            dr.Field<int>("BUNRUI"),
                            dr.Field<string>("SCODE"),
                            dr.Field<int>("SAIZUS"),
                            dr.Field<string>("HINNMB"),
                            dr.Field<string>("EOS_RCVDT"),
                            dr.Field<string>("EOS_DENPYONO"),
                            dr.Field<string>("EOS_HATYUBI"),
                            dr.Field<string>("EOS_HINBANCD"),
                            dr.Field<string>("EOS_TENCD"),
                            dr.Field<string>("EOS_SIRESAKICD"),
                            dr.Field<string>("EOS_SYOHINCD"),
                            dr.Field<string>("EOS_DENPYOGYO"),
                            dr.Field<string>("EOS_SYOHINNM"),
                            dr.Field<short>("EOS_HSU"),
                            dr.Field<int>("EOS_NSU"),
                            dr.Field<int>("EOS_GENKA"),
                            dr.Field<int>("EOS_BAIKA"),
                            dr.Field<string>("EOS_URIAGEBI")
                        ));
                }

                return results;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
