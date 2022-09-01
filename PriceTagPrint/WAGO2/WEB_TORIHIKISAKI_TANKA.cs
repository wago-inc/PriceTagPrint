using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace PriceTagPrint.WAGO2
{
    public class WEB_TORIHIKISAKI_TANKA
    {
        public string TID { get; set; }
        public short TCODE { get; set; }
        public short TENPO { get; set; }
        public int HCODE { get; set; }
        public short SAIZU { get; set; }
        public short BUNRUI { get; set; }
        public string SCODE { get; set; }
        public string SKBN { get; set; }
        public string TSCODE { get; set; }
        public string BIKOU1 { get; set; }
        public string BIKOU2 { get; set; }
        public string NEFUDA_KBN { get; set; }
        public string NETUKE_BUNRUI { get; set; }

        /// <summary>
        /// ヤスサキ用
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="tcode"></param>
        /// <param name="tenpo"></param>
        /// <param name="hcode"></param>
        /// <param name="saizu"></param>
        /// <param name="bunrui"></param>
        /// <param name="scode"></param>
        /// <param name="skbn"></param>
        /// <param name="bikou1"></param>
        /// <param name="bikou2"></param>
        /// <param name="nefuda_kbn"></param>
        /// <param name="netuke_bunrui"></param>
        public WEB_TORIHIKISAKI_TANKA(string tid, short tcode, short tenpo, int hcode, short saizu, short bunrui,
                                      string scode, string skbn, string tscode, string bikou1, string bikou2,
                                      string nefuda_kbn, string netuke_bunrui)
        {
            this.TID = tid;
            this.TCODE = tcode;
            this.TENPO = tenpo;
            this.HCODE = hcode;
            this.SAIZU = saizu;
            this.BUNRUI = bunrui;
            this.SCODE = scode;
            this.SKBN = skbn;
            this.TSCODE = tscode;
            this.BIKOU1 = bikou1;
            this.BIKOU2 = bikou2;
            this.NEFUDA_KBN = nefuda_kbn;
            this.NETUKE_BUNRUI = netuke_bunrui;
        }
    }

    public class WEB_TORIHIKISAKI_TANKA_LIST
    {
        public List<WEB_TORIHIKISAKI_TANKA> QueryWhereTcodeTenpo(string tcode, string tenpo)
        {
            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAGO2.WEB_TORIHIKISAKI_TANKA " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " TCODE = " + tcode + Environment.NewLine;
            sql += " AND TENPO = " + tenpo;

            DataTable orcDt = new DataTable();
            var results = new List<WEB_TORIHIKISAKI_TANKA>();
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
                            results.Add(new WEB_TORIHIKISAKI_TANKA
                                (
                                    row.Field<string>("TID"), 
                                    row.Field<short>("TCODE"), 
                                    row.Field<short>("TENPO"),
                                    row.Field<int>("HCODE"), 
                                    row.Field<short>("SAIZU"), 
                                    row.Field<short>("BUNRUI"),
                                    row.Field<string>("SCODE"), 
                                    row.Field<string>("SKBN"),
                                    row.Field<string>("TSCODE"),
                                    row.Field<string>("BIKOU1"),
                                    row.Field<string>("BIKOU2"), 
                                    row.Field<string>("NEFUDA_KBN"), 
                                    row.Field<string>("NETUKE_BUNRUI")
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
