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
    public class DB_T05005_SHOHIN_BUNRUI2_DAICHO
    {
        public short 商品分類2コード { get; set; }
        public string 商品分類2名 { get; set; }

        public DB_T05005_SHOHIN_BUNRUI2_DAICHO(short 商品分類2コード, string 商品分類2名)
        {
            this.商品分類2コード = 商品分類2コード;
            this.商品分類2名 = 商品分類2名;
        }
    }

    public class DB_T05005_SHOHIN_BUNRUI2_DAICHO_LIST
    {
        public List<DB_T05005_SHOHIN_BUNRUI2_DAICHO> QueryWhereAll()
        {
            var strSQL = "";
            strSQL = strSQL + Environment.NewLine + "SELECT ";
            strSQL = strSQL + Environment.NewLine + " T05005_商品分類2台帳.商品分類2コード, ";
            strSQL = strSQL + Environment.NewLine + " T05005_商品分類2台帳.商品分類2名 ";
            strSQL = strSQL + Environment.NewLine + "FROM ";
            strSQL = strSQL + Environment.NewLine + " T05005_商品分類2台帳 ";

            DataTable mdbDt = new DataTable();
            var results = new List<DB_T05005_SHOHIN_BUNRUI2_DAICHO>();
            // 読み込み
            try
            {
                OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString_ito);

                OdbcCommand sqlCommand = new OdbcCommand(strSQL, mdbConn);
                sqlCommand.CommandTimeout = 30;

                OdbcDataAdapter adapter = new OdbcDataAdapter(sqlCommand);

                adapter.Fill(mdbDt);
                adapter.Dispose();
                sqlCommand.Dispose();

                foreach (DataRow dr in mdbDt.Rows)
                {
                    results.Add(new DB_T05005_SHOHIN_BUNRUI2_DAICHO
                        (
                            dr.Field<short>("商品分類2コード"),
                            dr.Field<string>("商品分類2名")
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
