using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace PriceTagPrint.WAG_USR1
{
    public class TOKMTE
    {
        /// <summary>
        /// デプトクラスコード
        /// </summary>
        public string COLCD { get; set; }
        /// <summary>
        /// 得意先CD
        /// </summary>
        public string TOKCD { get; set; }
        /// <summary>
        /// 品番
        /// </summary>
        public string EOSHINID { get; set; }

        public TOKMTE(string colcd, string tokcd, string eoshinid)
        {
            this.COLCD = colcd;
            this.TOKCD = tokcd;
            this.EOSHINID = eoshinid;
        }
    }

    public class TOKMTE_LIST
    {
        public List<TOKMTE> QueryWhereTcode(int tcode)
        {
            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAG_USR1.TOKMTE " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " TOKCD = '" + tcode.ToString("000000") + "' ";

            DataTable orcDt = new DataTable();
            var results = new List<TOKMTE>();
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
                            results.Add(new TOKMTE
                                (
                                    row.Field<string>("COLCD"), row.Field<string>("TOKCD"), row.Field<string>("EOSHINID")
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
