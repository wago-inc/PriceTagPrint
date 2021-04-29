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
    public class DB_T05007_SHOHIN_TEKIYO_DAICHO
    {
        public short 商品摘要コード { get; set; }
        public string 商品摘要名 { get; set; }

        public DB_T05007_SHOHIN_TEKIYO_DAICHO(short 商品摘要コード, string 商品摘要名)
        {
            this.商品摘要コード = 商品摘要コード;
            this.商品摘要名 = 商品摘要名;
        }
    }

    public class DB_T05007_SHOHIN_TEKIYO_DAICHO_LIST
    {
        public List<DB_T05007_SHOHIN_TEKIYO_DAICHO> QueryWhereAll()
        {
            var strSQL = "";
            strSQL = strSQL + Environment.NewLine + "SELECT T05007_商品摘要台帳.*";
            strSQL = strSQL + Environment.NewLine + "FROM ";
            strSQL = strSQL + Environment.NewLine + " T05007_商品摘要台帳 ";

            DataTable mdbDt = new DataTable();
            var results = new List<DB_T05007_SHOHIN_TEKIYO_DAICHO>();
            // 読み込み
            try
            {
                using (OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString_ito))
                {
                    mdbConn.Open();

                    OdbcDataAdapter adapter = new OdbcDataAdapter(strSQL, mdbConn);
                    adapter.Fill(mdbDt);
                    foreach (DataRow dr in mdbDt.Rows)
                    {
                        results.Add(new DB_T05007_SHOHIN_TEKIYO_DAICHO
                            (
                                dr.Field<short>("商品摘要コード"),
                                dr.Field<string>("商品摘要名")
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
    }
}
