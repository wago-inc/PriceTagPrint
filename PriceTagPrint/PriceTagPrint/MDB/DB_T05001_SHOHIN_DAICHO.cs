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
    public class DB_T05001_SHOHIN_DAICHO
    {
        public string バーコード { get; set; }
        public byte 値札No { get; set; }

        public DB_T05001_SHOHIN_DAICHO(string バーコード, byte 値札No)
        {
            this.バーコード = バーコード;
            this.値札No = 値札No;
        }
    }

    public class DB_T05001_SHOHIN_DAICHO_LIST
    {
        public List<DB_T05001_SHOHIN_DAICHO> QueryWhereNefudaNo(int nefudaNo)
        {
            var strSQL = "";
            strSQL = strSQL + Environment.NewLine + "SELECT *";
            strSQL = strSQL + Environment.NewLine + "FROM ";
            strSQL = strSQL + Environment.NewLine + " T05001_商品台帳 ";
            strSQL = strSQL + Environment.NewLine + "WHERE ";
            strSQL = strSQL + Environment.NewLine + " 値札発行有無 = 1 ";
            strSQL = strSQL + Environment.NewLine + "AND 値札No = " + nefudaNo;

            DataTable mdbDt = new DataTable();
            var results = new List<DB_T05001_SHOHIN_DAICHO>();
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
                        results.Add(new DB_T05001_SHOHIN_DAICHO
                            (
                                dr.Field<string>("バーコード"),
                                dr.Field<byte>("値札No")
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
