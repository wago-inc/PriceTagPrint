using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace PriceTagPrint.WAGO
{
    public class HINMTA
    {
        public string HINCD { get; set; }
        public string JANCD { get; set; }

        public HINMTA(string hincd, string jancd)
        {
            this.HINCD = hincd;
            this.JANCD = jancd;
        }
    }

    public class HINMTA_LIST
    {
        public List<HINMTA> QueryWhereAll()
        {
            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAG_USR1.HINMTA ";

            DataTable orcDt = new DataTable();
            var results = new List<HINMTA>();
            try
            {
                using (OracleConnection orcConn = new OracleConnection(DBConnect.OrclConnectString))
                {
                    orcConn.Open();

                    OracleCommand cmd = new OracleCommand(sql, orcConn);

                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        orcDt.Load(reader);
                        foreach (DataRow row in orcDt.Rows)
                        {
                            results.Add(new HINMTA
                                (
                                    row.Field<string>("HINCD"), row.Field<string>("JANCD")
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
