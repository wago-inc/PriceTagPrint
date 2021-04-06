using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace PriceTagPrint.WAGO
{
    public class EOSJUTRA
    {
        /// <summary>
        /// DATNO
        /// </summary>
        public string DATNO { get; set; }
        /// <summary>
        /// 得意先CD
        /// </summary>
        public string VRYOHNCD { get; set; }
        /// <summary>
        /// 受信日
        /// </summary>
        public string VRCVDT { get; set; }
        /// <summary>
        /// 分類コード
        /// </summary>
        public string VBUNCD { get; set; }

        public string VROWNO { get; set; }
        /// <summary>
        /// 取引先コード
        /// </summary>
        public string QOLTORID { get; set; }
        /// <summary>
        /// 納品日
        /// </summary>
        public string VNOHINDT { get; set; }
        /// <summary>
        /// 品番
        /// </summary>
        public string VHINCD { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string VHINNMA { get; set; }
        /// <summary>
        /// 発行枚数
        /// </summary>
        public decimal VSURYO { get; set; }
        /// <summary>
        /// 本体売価
        /// </summary>
        public decimal VURITK { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// JAN13桁
        /// </summary>
        public string VCYOBI7 { get; set; }

        public string VHEAD1 { get; set; }

        public string VBODY1 { get; set; }

        /// <summary>
        /// ヤマナカ用
        /// </summary>
        /// <param name="vryohncd"></param>
        /// <param name="vrcvdt"></param>
        /// <param name="vbuncd"></param>
        /// <param name="qoltorid"></param>
        /// <param name="vnohindt"></param>
        /// <param name="vhincd"></param>
        /// <param name="vhinnma"></param>
        /// <param name="vsuryo"></param>
        /// <param name="vuritk"></param>
        /// <param name="hincd"></param>
        /// <param name="vcyobi7"></param>
        public EOSJUTRA(string vryohncd, string vrcvdt, string vbuncd, string qoltorid, string vnohindt,
                        string vhincd, string vhinnma, decimal vsuryo, decimal vuritk, string hincd, string vcyobi7,
                        string vhead1, string vbody1)
        {
            this.VRYOHNCD = vryohncd;
            this.VRCVDT = vrcvdt;
            this.VBUNCD = vbuncd;
            this.QOLTORID = qoltorid;
            this.VNOHINDT = vnohindt;
            this.VHINCD = vhincd;
            this.VHINNMA = vhinnma;
            this.VSURYO = vsuryo;
            this.VURITK = vuritk;
            this.HINCD = hincd;
            this.VCYOBI7 = vcyobi7;
            this.VHEAD1 = vhead1;
            this.VBODY1 = vbody1;
        }
    }

    public class EOSJUTRA_LIST
    {
        public List<EOSJUTRA> QueryWhereTcodeAndDates(int tcode, DateTime jusin, DateTime nouhin, string bunrui = "", string sttHin = "", string endHin = "")
        {
            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAG_USR1.EOSJUTRA " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " VRYOHNCD = '" + tcode.ToString("000000") + "' " + Environment.NewLine;
            sql += " AND VRCVDT = '" + jusin.ToString("yyyyMMdd") + "' " + Environment.NewLine;
            sql += " AND VNOHINDT = '" + nouhin.ToString("yyyyMMdd") + "' ";
            if (!string.IsNullOrEmpty(bunrui))
            {
                sql += Environment.NewLine;
                sql += " AND VBUNCD = '" + bunrui + "' ";
            }
            if (!string.IsNullOrEmpty(sttHin))
            {
                sql += Environment.NewLine;
                sql += " AND VHINCD >= '" + sttHin + "' ";
            }
            if (!string.IsNullOrEmpty(endHin))
            {
                sql += Environment.NewLine;
                sql += " AND VHINCD <= '" + endHin + "' ";
            }
            DataTable orcDt = new DataTable();
            var results = new List<EOSJUTRA>();
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
                            results.Add(new EOSJUTRA
                                (
                                    row.Field<string>("VRYOHNCD"), row.Field<string>("VRCVDT"), row.Field<string>("VBUNCD"),
                                    row.Field<string>("QOLTORID"), row.Field<string>("VNOHINDT"), row.Field<string>("VHINCD"),
                                    row.Field<string>("VHINNMA"), row.Field<decimal>("VSURYO"), row.Field<decimal>("VURITK"),
                                    row.Field<string>("HINCD"), row.Field<string>("VCYOBI7"), row.Field<string>("VHEAD1"),
                                    row.Field<string>("VBODY1")
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
