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
        /// 得意先CD
        /// </summary>
        public string TOKCD { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// 品番
        /// </summary>
        public string EOSHINID { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string EOSHINNA { get; set; }
        /// <summary>
        /// 商品規格名
        /// </summary>
        public string EOSHINNB { get; set; }        
        /// <summary>
        /// デプトクラスコード
        /// </summary>
        public string COLCD { get; set; }
        /// <summary>
        /// デプトクラスコード
        /// </summary>
        public string SIZCD { get; set; }
        /// <summary>
        /// EOS売価単価
        /// </summary>
        public decimal EOSURITK { get; set; }

        public TOKMTE(string tokcd, string hincd, string eoshinid, string eoshinna, string eoshinnb, string colcd, 
                      string sizcd, decimal eosuritk)
        {            
            this.TOKCD = tokcd;
            this.HINCD = hincd;
            this.EOSHINID = eoshinid;
            this.EOSHINNA = eoshinna;
            this.EOSHINNB = eoshinnb;
            this.COLCD = colcd;
            this.SIZCD = sizcd;
            this.EOSURITK = eosuritk;
        }
    }

    public class TOKMTE_LIST
    {
        public List<TOKMTE> QueryWhereTcode(int tcode)
        {
            var sql = "SELECT TOKCD, HINCD, EOSHINID, EOSHINNA, EOSHINNB, COLCD, SIZCD, EOSURITK " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAG_USR1.TOKMTE " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " TOKCD = '" + tcode.ToString("000000") + "' ";

            DataTable orcDt = new DataTable();
            var results = new List<TOKMTE>();
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
                            results.Add(new TOKMTE
                                (
                                    row.Field<string>("TOKCD"), row.Field<string>("HINCD"), row.Field<string>("EOSHINID"),
                                    row.Field<string>("EOSHINNA"), row.Field<string>("EOSHINNB"), row.Field<string>("COLCD"),
                                    row.Field<string>("SIZCD"), row.Field<decimal>("EOSURITK")
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
