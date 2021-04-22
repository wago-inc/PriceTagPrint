using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Windows;

namespace PriceTagPrint.MDB
{
    
    public class DB_0112_EOS_HACHU
    {
        // 発注番号
        public int HNO { get; set; }
        // 得意先コード
        public string TOKCD { get; set; }
        // 店舗コード
        public short TENCD { get; set; }
        // 伝票番号
        public string DENPYONO { get; set; }
        // 伝票行番号
        public int DENPYOGYO { get; set; }
        // 相手先商品コード
        public string SYOHINCD { get; set; }
        // ＪＡＮコード
        public string JANCD { get; set; }
        // 発注日
        public DateTime HATYUBI { get; set; }
        // 発注数
        public short HSU { get; set; }
        // 納品予定数
        public short NSU { get; set; }
        // 原単価
        public int GENKA { get; set; }
        // 売単価
        public int BAIKA { get; set; }
        // 納品日
        public DateTime NOUHINBI { get; set; }
        // 商品コード
        public string HINCD { get; set; }
        // 分類コード
        public int BUNRUI { get; set; }
        // 商品コード
        public string SCODE { get; set; }
        // 社内サイズカラーコード
        public int SAIZUS { get; set; }        
        // EOS DT_04 商品コード
        public string EOS_SYOHINNM { get; set; }
        

        public DB_0112_EOS_HACHU(int hno, string tokcd, short tencd, string denpyono, int denpyogyo, string syohincd, 
                               string jancd, DateTime hatyubi, short hsu, short nsu, int genka, int baika, 
                               DateTime nouhinbi, string hincd, int bunrui, string scode, int saizus, string eos_syohinnm)
        {
            this.HNO = hno;
            this.TOKCD = tokcd;
            this.TENCD = tencd;
            this.DENPYONO = denpyono;
            this.DENPYOGYO = denpyogyo;
            this.SYOHINCD = syohincd;
            this.JANCD = jancd;
            this.HATYUBI = hatyubi;
            this.HSU = hsu;
            this.NSU = nsu;
            this.GENKA = genka;
            this.BAIKA = baika;
            this.NOUHINBI = nouhinbi;
            this.HINCD = hincd;
            this.BUNRUI = bunrui;
            this.SCODE = scode;
            this.SAIZUS = saizus;
            this.EOS_SYOHINNM = eos_syohinnm;
        }
    }
    public class DB_0112_EOS_HACHU_LIST
    {
        public List<DB_0112_EOS_HACHU> QueryWhereHno(string hno)
        {

            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " 0112_EOS_HACHU " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " HNO = " + hno;

            DataTable mdbDt = new DataTable();
            var results = new List<DB_0112_EOS_HACHU>();
            // 読み込み
            try
            {
                using (OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString))
                {
                    mdbConn.Open();

                    OdbcDataAdapter adapter = new OdbcDataAdapter(sql, mdbConn);
                    adapter.Fill(mdbDt);
                    foreach (DataRow dr in mdbDt.Rows)
                    {
                        results.Add(new DB_0112_EOS_HACHU
                            (
                                dr.Field<int>("HNO"), dr.Field<string>("TOKCD"), dr.Field<short>("TENCD"),
                                dr.Field<string>("DENPYONO"), dr.Field<int>("DENPYOGYO"), dr.Field<string>("SYOHINCD"),
                                dr.Field<string>("JANCD"), dr.Field<DateTime>("HATYUBI"), dr.Field<short>("HSU"),
                                dr.Field<short>("NSU"), dr.Field<int>("GENKA"), dr.Field<int>("BAIKA"),
                                dr.Field<DateTime>("NOUHINBI"), dr.Field<string>("HINCD"), dr.Field<int>("BUNRUI"),
                                dr.Field<string>("SCODE"), dr.Field<int>("SAIZUS"), dr.Field<string>("EOS_SYOHINNM")
                            ));
                    }

                    return results;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public bool QueryWhereHnoExists(string hno)
        {
            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " 0112_EOS_HACHU " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " HNO = " + hno;

            DataTable mdbDt = new DataTable();
            var results = new List<DB_0112_EOS_HACHU>();
            // 読み込み
            try
            {
                using (OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString))
                {
                    mdbConn.Open();

                    OdbcDataAdapter adapter = new OdbcDataAdapter(sql, mdbConn);
                    adapter.Fill(mdbDt);
                    

                    return mdbDt.Rows.Count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
        }
    }
}
