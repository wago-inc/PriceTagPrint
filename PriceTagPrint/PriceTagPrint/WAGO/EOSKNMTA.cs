using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace PriceTagPrint.WAGO
{
    public class EOSKNMTA
    {
        public string VRYOHNNM { get; set; }
        public string VRYOHNCD { get; set; }

        public EOSKNMTA(string vryohnnm, string vryohncd)
        {
            this.VRYOHNNM = vryohnnm;
            this.VRYOHNCD = vryohncd;
        }
    }

    public class EOSKNMTA_LIST
    {
        public List<EOSKNMTA> QueryWhereTcode(int tcode)
        {
            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAG_USR1.EOSKNMTA " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " VRYOHNCD = '" + tcode.ToString("000000") + "' ";

            DataTable orcDt = new DataTable();
            var results = new List<EOSKNMTA>();
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
                            results.Add(new EOSKNMTA
                                (
                                    row.Field<string>("VRYOHNNM"), row.Field<string>("VRYOHNCD")
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
