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
    public class DB_0118_HINBAN
    {
        public string HINBANCD { get; set; }
        public string CYUBUNCD { get; set; }
        public string HINBANNM { get; set; }
        public string CYUBUNNM { get; set; }
        public DB_0118_HINBAN(string hinbancd, string cyubuncd, string hinbannm, string cyubunnm)
        {
            this.HINBANCD = hinbancd;
            this.CYUBUNCD = cyubuncd;
            this.HINBANNM = hinbannm;
            this.CYUBUNNM = cyubunnm;
        }
    }

    public class DB_0118_HINBAN_LIST
    {
        public List<DB_0118_HINBAN> dB_0118_HINBANs = new List<DB_0118_HINBAN>();
        public DB_0118_HINBAN_LIST()
        {
            Create();
        }
        private void Create()
        {
            var strSQL = "";
            strSQL = strSQL + Environment.NewLine + "SELECT HINBANCD, CYUBUNCD, HINBANNM, CYUBUNNM ";
            strSQL = strSQL + Environment.NewLine + "FROM ";
            strSQL = strSQL + Environment.NewLine + " 0118_HINBAN " + "; ";

            DataTable mdbDt = new DataTable();
            var results = new List<DB_0118_HINBAN>();
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
                        dB_0118_HINBANs.Add(new DB_0118_HINBAN
                            (
                                dr.Field<string>("HINBANCD"),
                                dr.Field<string>("CYUBUNCD"),
                                dr.Field<string>("HINBANNM"),
                                dr.Field<string>("CYUBUNNM")
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