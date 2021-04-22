using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Windows;

namespace PriceTagPrint.MDB
{
    public class DB_JYUCYU
    {
        /// <summary>
        /// 得意先コード
        /// </summary>
        public int TCODE { get; set; }
        /// <summary>
        /// 値札区分
        /// </summary>
        public int NEFUDA_KBN { get; set; }
        /// <summary>
        /// 発注番号
        /// </summary>
        public int HNO { get; set; }
        /// <summary>
        /// 出荷数
        /// </summary>
        public int TSU { get; set; }
        /// <summary>
        /// ロケーション倉庫コード
        /// </summary>
        public int LOCTANA_SOKO_CODE { get; set; }
        /// <summary>
        /// ロケーションフロアNo
        /// </summary>
        public int LOCTANA_FLOOR_NO { get; set; }
        /// <summary>
        /// ロケーション棚No
        /// </summary>
        public int LOCTANA_TANA_NO { get; set; }
        /// <summary>
        /// ロケーションケースNo
        /// </summary>
        public int LOCTANA_CASE_NO { get; set; }
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
        /// 相手先JANコード
        /// </summary>
        public string JANCD { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string HINMEI { get; set; }
        /// <summary>
        /// サイズ名
        /// </summary>
        public string SAIZUN { get; set; }
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
        /// わたせい用コンストラクタ
        /// </summary>
        public DB_JYUCYU(int tcode, int nefuda_kbn, int hno, int tsu, int loctana_soko_code, int loctana_floor_no, int loctana_tana_no,
                         int loctana_case_no, int bunrui, string scode, int saizus, string jancd, string hinmei, string saizun,
                         int htanka, int jyodai, int bumon)
        {
            this.TCODE = tcode;
            this.NEFUDA_KBN = nefuda_kbn;
            this.HNO = hno;
            this.TSU = tsu;
            this.LOCTANA_SOKO_CODE = loctana_soko_code;
            this.LOCTANA_FLOOR_NO = loctana_floor_no;
            this.LOCTANA_TANA_NO = loctana_tana_no;
            this.LOCTANA_CASE_NO = loctana_case_no;
            this.BUNRUI = bunrui;
            this.SCODE = scode;
            this.SAIZUS = saizus;
            this.JANCD = jancd;
            this.HINMEI = hinmei;
            this.SAIZUN = saizun;
            this.HTANKA = htanka;
            this.JYODAI = jyodai;
            this.BUMON = bumon;
        }
    }

    public class DB_JYUCYU_LIST
    {
        public List<DB_JYUCYU> QueryWhereHno(string hno)
        {

            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " JYUCYU " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " HNO = " + hno;

            DataTable mdbDt = new DataTable();
            var results = new List<DB_JYUCYU>();
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
                        results.Add(new DB_JYUCYU
                            (
                                dr.Field<int>("TCODE"), dr.Field<int>("NEFUDA_KBN"), dr.Field<int>("HNO"),
                                dr.Field<int>("TSU"), dr.Field<int>("LOCTANA_SOKO_CODE"), dr.Field<int>("LOCTANA_FLOOR_NO"),
                                dr.Field<int>("LOCTANA_TANA_NO"), dr.Field<int>("LOCTANA_CASE_NO"), dr.Field<int>("BUNRUI"),
                                dr.Field<string>("SCODE"), dr.Field<int>("SAIZUS"), dr.Field<string>("JANCD"),
                                dr.Field<string>("HINMEI"), dr.Field<string>("SAIZUN"), dr.Field<int>("HTANKA"),
                                dr.Field<int>("JYODAI"), dr.Field<int>("BUMON")
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
            sql += " JYUCYU " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " HNO = " + hno;

            DataTable mdbDt = new DataTable();
            var results = new List<DB_JYUCYU>();
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
