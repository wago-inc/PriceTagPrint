using PriceTagPrint.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Windows;

namespace PriceTagPrint.MDB
{
    public class DB_0112_URITUKI
    {
        // 商品区分
        public string SKBN { get; set; }
        // 年度
        public decimal NENDO { get; set; }
        // 月
        public decimal TUKI { get; set; }
        // 売切月
        public decimal URITUKI { get; set; }
        public DB_0112_URITUKI(string skbn, decimal nendo, decimal tuki, decimal urituki)
        {
            this.SKBN = skbn;
            this.NENDO = nendo;
            this.TUKI = tuki;
            this.URITUKI = urituki;
        }
    }

    public class DB_0112_URITUKI_LIST
    {
        public List<DB_0112_URITUKI> dB_0112_URITUKIs = new List<DB_0112_URITUKI>();
        public DB_0112_URITUKI_LIST()
        {
            SelectQueryByNendo(2016);
        }
        private void SelectQueryByNendo(int nendo)
        {
            var strSQL = "";
            strSQL = strSQL + Environment.NewLine + "SELECT ";
            strSQL = strSQL + Environment.NewLine + " SKBN, ";
            strSQL = strSQL + Environment.NewLine + " NENDO, ";
            strSQL = strSQL + Environment.NewLine + " TUKI, ";
            strSQL = strSQL + Environment.NewLine + " URITUKI ";
            strSQL = strSQL + Environment.NewLine + "FROM ";
            strSQL = strSQL + Environment.NewLine + " 0112_URITUKI ";
            strSQL = strSQL + Environment.NewLine + "WHERE ";
            strSQL = strSQL + Environment.NewLine + " SKBN = '0' AND ";
            strSQL = strSQL + Environment.NewLine + " NENDO = " + nendo + "; ";

            DataTable mdbDt = new DataTable();
            var results = new List<DB_0112_URITUKI>();
            // 読み込み
            try
            {
                OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString);

                OdbcCommand sqlCommand = new OdbcCommand(strSQL, mdbConn);
                sqlCommand.CommandTimeout = 30;

                OdbcDataAdapter adapter = new OdbcDataAdapter(sqlCommand);

                adapter.Fill(mdbDt);
                adapter.Dispose();
                sqlCommand.Dispose();

                foreach (DataRow dr in mdbDt.Rows)
                {
                    dB_0112_URITUKIs.Add(new DB_0112_URITUKI
                        (
                            dr.Field<string>("SKBN"),
                            dr.Field<decimal>("NENDO"),
                            dr.Field<decimal>("TUKI"),
                            dr.Field<decimal>("URITUKI")
                        ));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public string GetURITUKI(DateTime pNOUHINBI, string pSKBN)
        {
            int wk_NENDO;
            int wk_TUKI;
            int wk_URITUKI;
            int wk_CNT;

            wk_NENDO = pNOUHINBI.Year;                    // ※納品年度
            wk_TUKI = pNOUHINBI.Month;                       // ※納品月

            // --- 委託定番の売切月セット     ※2017/02/14 追加
            if (pSKBN == "5")
            {
                return "55";                                     // ※売切月＝５５
            }

            // --- シーズン定番の売切月セット
            if (pSKBN != "0")
            {
                return wk_TUKI.ToString("00");                           // ※売切月＝納品月
            }

            var dB_0112_URITUKI = dB_0112_URITUKIs.FirstOrDefault(x => x.NENDO == 2016 && x.TUKI == wk_TUKI);

            if(dB_0112_URITUKI == null)
            {
                return "00";
            }
            else
            {
                int dbNendo;
                int dbUrituki;
                dbNendo = int.TryParse(dB_0112_URITUKI.NENDO.ToString(), out dbNendo) ? dbNendo : 0;
                dbUrituki = int.TryParse(dB_0112_URITUKI.URITUKI.ToString(), out dbUrituki) ? dbUrituki : 0;
                // --- 売切月の算出
                wk_CNT = wk_NENDO - dbNendo;                        // ※納品年度－2016年（売切月規定年度）
                if (wk_CNT > 0)
                {
                    wk_URITUKI = dbUrituki + wk_CNT;                // ※前年度売切月＋１
                }
                else
                {
                    wk_URITUKI = dbUrituki;// ※規定年度の売切月
                }


                // --- 売切月のチェック   ※2020/08/31 追加
                // ※2020/09/01に売切月=100になった場合は-10する
                if (wk_URITUKI > 99)
                {
                    wk_URITUKI = wk_URITUKI - 10;
                }
                return wk_URITUKI.ToString("00");
            }
        }
    }
}
