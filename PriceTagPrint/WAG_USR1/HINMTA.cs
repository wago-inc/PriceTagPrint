using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace PriceTagPrint.WAG_USR1
{
    public class HINMTA
    {
        public string HINCD { get; set; }
        public string HINNMA { get; set; }
        public string JANCD { get; set; }
        public string HINTKSID { get; set; }
        public string HINCLID { get; set; }
        public string HINID { get; set; }
        public string SIZCOLID { get; set; }
        public HINMTA(string hincd, string hinnma, string jancd, string hintksid, string hinclid, string hinid, string sizcolid)
        {
            this.HINCD = hincd;
            this.HINNMA = hinnma;
            this.JANCD = jancd;
            this.HINTKSID = hintksid;
            this.HINCLID = hinclid;
            this.HINID = hinid;
            this.SIZCOLID = sizcolid;
        }
    }

    public class HINMTA_LIST
    {
        public List<HINMTA> QueryWhereAll()
        {
            var sql = "SELECT HINCD, HINNMA, JANCD, HINTKSID, HINCLID, HINID, SIZCOLID " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAG_USR1.HINMTA ";

            DataTable orcDt = new DataTable();
            var results = new List<HINMTA>();
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
                            results.Add(new HINMTA
                                (
                                    row.Field<string>("HINCD"),
                                    row.Field<string>("HINNMA"),
                                    row.Field<string>("JANCD"), 
                                    row.Field<string>("HINTKSID"),
                                    row.Field<string>("HINCLID"),
                                    row.Field<string>("HINID"),
                                    row.Field<string>("SIZCOLID")
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
