using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace PriceTagPrint.WAGO
{
    public class TOKMSTPF
    {
        public decimal TCODE { get; set; }
        public decimal TENPO { get; set; }
        public string TNAMEN { get; set; }
        public string RYAKU { get; set; }
        public string JISYA { get; set; }

        public TOKMSTPF(decimal tcode, decimal tenpo, string tnamen, string ryaku, string jisya)
        {
            this.TCODE = tcode;
            this.TENPO = tenpo;
            this.TNAMEN = tnamen;
            this.RYAKU = ryaku;
            this.JISYA = jisya;
        }
    }

    public class TOKMSTPF_LIST
    {
        public List<TOKMSTPF> QueryWhereTcodeTenpo(int tcode, int tenpo = 9999)
        {
            var sql = "SELECT TCODE, TENPO, TNAMEN, RYAKU, JISYA " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAGO.TOKMSTPF " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " TCODE = " + tcode + " " + Environment.NewLine;
            sql += "AND TENPO = " + tenpo;

            DataTable orcDt = new DataTable();
            var results = new List<TOKMSTPF>();
            var connectString = DBConnect.OrclConnectString + DBConnect.OrclDataSource;
            try
            {
                using (OracleConnection orcConn = new OracleConnection(connectString))
                {
                    orcConn.Open();

                    OracleCommand cmd = new OracleCommand(sql, orcConn);

                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        orcDt.Load(reader);
                        foreach (DataRow row in orcDt.Rows)
                        {
                            results.Add(new TOKMSTPF
                                (
                                    row.Field<decimal>("TCODE"), 
                                    row.Field<decimal>("TENPO"), 
                                    row.Field<string>("TNAMEN"),
                                    row.Field<string>("RYAKU"),
                                    row.Field<string>("JISYA")
                                ));
                        }
                    }

                    reader.Close();
                    orcConn.Close();
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
