using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Windows;

namespace PriceTagPrint.MDB
{
    public class DB_2101_JYUCYU
    {
        /// <summary>
        /// 得意先コード
        /// </summary>
        public int TCODE { get; set; }
        /// <summary>
        /// 発注番号
        /// </summary>
        public int HNO { get; set; }
        /// <summary>
        /// 出荷数
        /// </summary>
        public int TSU { get; set; }
        /// <summary>
        /// 分類コード
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
        /// 商品名
        /// </summary>
        public string HINMEI { get; set; }
        /// <summary>
        /// サイズ名
        /// </summary>
        public string SAIZUN { get; set; }
        /// <summary>
        /// 仕入単価
        /// </summary>
        public int STANKA { get; set; }
        /// <summary>
        /// 販売単価
        /// </summary>
        public int HTANKA { get; set; }
        /// <summary>
        /// 参考上代
        /// </summary>
        public int JYODAI { get; set; }
        /// <summary>
        /// 部門CD
        /// </summary>
        public int BUMON { get; set; }
        /// <summary>
        /// SKU管理番号
        /// </summary>
        public int SKU { get; set; }

        /// <summary>
        /// アイテムコード
        /// </summary>
        public int ITEMCD { get; set; }
        /// <summary>
        /// サイズ
        /// </summary>
        public int? SAIZU { get; set; }
        /// <summary>
        /// カラー（COLCDは同じもの）
        /// </summary>
        public int? COLOR { get; set; }
        /// <summary>
        /// 条件テーブルCD
        /// </summary>
        public double? JTBLCD { get; set; }
        /// <summary>
        /// 下代変換CD
        /// </summary>
        public int HENCD { get; set; }
        /// <summary>
        /// 定番区分
        /// </summary>
        public int TKBN { get; set; }
        /// <summary>
        /// 商品名（日本語）
        /// </summary>
        public string HINMEIN { get; set; }

        public int? LOCTANA_SYOHIN_CD { get; set; }
        public DB_2101_JYUCYU(int tcode, int hno, int tsu, int bunrui, string scode, int saizus, string hinmei, string saizun,
                         int stanka, int htanka, int jyodai, int bumon, int sku, int itemcd, int? saizu, int? color, double? jtblcd,
                         int hencd, int tkbn, string hinmein, int? loctana_syohin_cd)
        {
            this.TCODE = tcode;
            this.HNO = hno;
            this.TSU = tsu;
            this.BUNRUI = bunrui;
            this.SCODE = scode;
            this.SAIZUS = saizus;
            this.HINMEI = hinmei;
            this.SAIZUN = saizun;
            this.STANKA = stanka;
            this.HTANKA = htanka;
            this.JYODAI = jyodai;
            this.BUMON = bumon;
            this.SKU = sku;
            this.ITEMCD = itemcd;
            this.SAIZU = saizu;
            this.COLOR = color;
            this.JTBLCD = jtblcd;
            this.HENCD = hencd;
            this.TKBN = tkbn;
            this.HINMEIN = hinmein;
            this.LOCTANA_SYOHIN_CD = loctana_syohin_cd;
        }

        public DB_2101_JYUCYU(int hno, int tsu, int sku, double? jtblcd)
        {
            this.HNO = hno;
            this.TSU = tsu;
            this.SKU = sku;
            this.JTBLCD = jtblcd;
        }
    }

    public class DB_2101_JYUCYU_LIST
    {
        public List<DB_2101_JYUCYU> QueryWhereHno(string hno)
        {

            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " JYUCYU " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " HNO = " + hno;

            DataTable mdbDt = new DataTable();
            var results = new List<DB_2101_JYUCYU>();
            // 読み込み
            try
            {
                OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString_maneki);

                OdbcCommand sqlCommand = new OdbcCommand(sql, mdbConn);
                sqlCommand.CommandTimeout = 30;

                OdbcDataAdapter adapter = new OdbcDataAdapter(sqlCommand);

                adapter.Fill(mdbDt);
                adapter.Dispose();
                sqlCommand.Dispose();

                foreach (DataRow dr in mdbDt.Rows)
                {
                    results.Add(new DB_2101_JYUCYU
                        (
                            dr.Field<int>("TCODE"),
                            dr.Field<int>("HNO"),
                            dr.Field<int>("TSU"),
                            dr.Field<int>("BUNRUI"),
                            dr.Field<string>("SCODE"),
                            dr.Field<int>("SAIZUS"),
                            dr.Field<string>("HINMEI"),
                            dr.Field<string>("SAIZUN"),
                            dr.Field<int>("STANKA"),
                            dr.Field<int>("HTANKA"),
                            dr.Field<int>("JYODAI"),
                            dr.Field<int>("BUMON"),
                            dr.Field<int>("SKU"),
                            dr.Field<int>("ITEMCD"),
                            dr.Field<int?>("SAIZU"),
                            dr.Field<int?>("COLOR"),
                            dr.Field<double?>("JTBLCD"),
                            dr.Field<int>("HENCD"),
                            dr.Field<int>("TKBN"),
                            dr.Field<string>("HINMEIN"),
                            dr.Field<int?>("LOCTANA_SYOHIN_CD")
                        ));
                }

                return results;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return null;
            }
        }

        public List<DB_2101_JYUCYU> QueryWhereHnoSkuBetween(string hno, string sttSku, string endSku)
        {
            var sql = "SELECT HNO, TSU, SKU, JTBLCD " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " JYUCYU " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " HNO = " + hno + Environment.NewLine;
            sql += " AND SKU BETWEEN " + sttSku + " AND " + endSku;

            DataTable mdbDt = new DataTable();
            var results = new List<DB_2101_JYUCYU>();
            // 読み込み
            try
            {
                OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString_maneki);

                OdbcCommand sqlCommand = new OdbcCommand(sql, mdbConn);
                sqlCommand.CommandTimeout = 30;

                OdbcDataAdapter adapter = new OdbcDataAdapter(sqlCommand);

                adapter.Fill(mdbDt);
                adapter.Dispose();
                sqlCommand.Dispose();

                foreach (DataRow dr in mdbDt.Rows)
                {
                    results.Add(new DB_2101_JYUCYU
                        (
                            dr.Field<int>("HNO"),
                            dr.Field<int>("TSU"),
                            dr.Field<int>("SKU"),
                            dr.Field<double?>("JTBLCD")
                        ));
                }

                return results;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return null;
            }
        }

        public bool QueryWhereHnoExists(string hno)
        {
            var sql = "SELECT HNO " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " JYUCYU " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " HNO = " + hno;

            DataTable mdbDt = new DataTable();
            var results = new List<DB_2101_JYUCYU>();
            // 読み込み
            try
            {
                OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString_maneki);

                OdbcCommand sqlCommand = new OdbcCommand(sql, mdbConn);
                sqlCommand.CommandTimeout = 30;

                OdbcDataAdapter adapter = new OdbcDataAdapter(sqlCommand);

                adapter.Fill(mdbDt);
                adapter.Dispose();
                sqlCommand.Dispose();

                return mdbDt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }
    }
}
