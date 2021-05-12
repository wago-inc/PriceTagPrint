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
    public class DB_0118_KUBUN
    {
        public short CALL_KEY { get; set; }
        public string KBN_KEY1 { get; set; }
        public string KBN_NAME1 { get; set; }
        public string KBN_NAME2 { get; set; }
        public string KBN_NAME3 { get; set; }
        public DB_0118_KUBUN(short callkey, string kbnkey1, string kbnname1, string kbnname2, string kbnname3)
        {
            this.CALL_KEY = callkey;
            this.KBN_KEY1 = kbnkey1;
            this.KBN_NAME1 = kbnname1;
            this.KBN_NAME2 = kbnname2;
            this.KBN_NAME3 = kbnname3;
        }
    }

    public class DB_0118_KUBUN_LIST
    {
        public List<DB_0118_KUBUN> dB_0118_KUBUNs = new List<DB_0118_KUBUN>();
        public DB_0118_KUBUN_LIST()
        {
            Create();
        }
        private void Create()
        {
            var strSQL = "";
            strSQL = strSQL + Environment.NewLine + "SELECT CALL_KEY, KBN_KEY1, KBN_NAME1, KBN_NAME2, KBN_NAME3 ";
            strSQL = strSQL + Environment.NewLine + "FROM ";
            strSQL = strSQL + Environment.NewLine + " 0118_KUBUN " + "; ";

            DataTable mdbDt = new DataTable();
            var results = new List<DB_0118_KUBUN>();
            // 読み込み
            try
            {
                using (OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString))
                {
                    mdbConn.Open();

                    OdbcDataAdapter adapter = new OdbcDataAdapter(strSQL, mdbConn);
                    adapter.Fill(mdbDt);
                    foreach (DataRow dr in mdbDt.Rows)
                    {
                        dB_0118_KUBUNs.Add(new DB_0118_KUBUN
                            (
                                dr.Field<short>("CALL_KEY"),
                                dr.Field<string>("KBN_KEY1"),
                                dr.Field<string>("KBN_NAME1"),
                                dr.Field<string>("KBN_NAME2"),
                                dr.Field<string>("KBN_NAME3")
                            ));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
