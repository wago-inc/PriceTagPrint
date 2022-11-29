using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using PriceTagPrint.MDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace PriceTagPrint.WAGO2
{
    public class WEB_ORDER_CNV
    {
        public short HOST_TORIHIKISAKI_CD { get; set; } // TCODE
        public int KENPINLIST_NO { get; set; }  // HNO
        public string DENPRT_BUNRUI { get; set; }   // BUMON
        public short HOST_BUNRUI_CD { get; set; }   // BUNRUI
        public string HOST_SYOHIN_CD { get; set; }  // SCODE
        public short ORDERLIST_SIZECOLOR_CD { get; set; }   // SAIZUS
        public string DENPRT_SYOHIN_CODE { get; set; }  // JANCD
        public string SIZECOLOR_SETSUMEI4 { get; set; } // 分類 + "-" + 品番
        public string SYOHIN_NAME1 { get; set; }    // HINMEI
        public string SIZECOLOR_NAME1 { get; set; } // SAIZUN
        public int NETUKE_HBAIKA { get; set; }   // HTANKA
        public int NETUKE_ZBAIKA { get; set; }  // ZBAIKA
        public string LOCATION_CD { get; set; } // ロケーション
        public int HACHU_SU { get; set; }   // TSU

        public WEB_ORDER_CNV(short hOST_TORIHIKISAKI_CD, int kENPINLIST_NO, string dENPRT_BUNRUI, short hOST_BUNRUI_CD, 
                             string hOST_SYOHIN_CD, short oRDERLIST_SIZECOLOR_CD, string dENPRT_SYOHIN_CODE, string sIZECOLOR_SETSUMEI4, 
                             string sYOHIN_NAME1, string sIZECOLOR_NAME1, int nETUKE_HBAIKA, int nETUKE_ZBAIKA, string lOCATION_CD, int hACHU_SU)
        {
            HOST_TORIHIKISAKI_CD = hOST_TORIHIKISAKI_CD;
            KENPINLIST_NO = kENPINLIST_NO;
            DENPRT_BUNRUI = dENPRT_BUNRUI;
            HOST_BUNRUI_CD = hOST_BUNRUI_CD;
            HOST_SYOHIN_CD = hOST_SYOHIN_CD;
            ORDERLIST_SIZECOLOR_CD = oRDERLIST_SIZECOLOR_CD;
            DENPRT_SYOHIN_CODE = dENPRT_SYOHIN_CODE;
            SIZECOLOR_SETSUMEI4 = sIZECOLOR_SETSUMEI4;
            SYOHIN_NAME1 = sYOHIN_NAME1;
            SIZECOLOR_NAME1 = sIZECOLOR_NAME1;
            NETUKE_HBAIKA = nETUKE_HBAIKA;
            NETUKE_ZBAIKA = nETUKE_ZBAIKA;
            LOCATION_CD = lOCATION_CD;
            HACHU_SU = hACHU_SU;
        }
    }

    public class WEB_ORDER_CNV_LIST
    {
        public List<WEB_ORDER_CNV> QueryWhereTcodekenpinListNo(int tcode, string kenpinListNo)
        {
            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAGO2.WEB_ORDER_CNV " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " KENPINLIST_NO = " + kenpinListNo + Environment.NewLine;
            sql += " AND HOST_TORIHIKISAKI_CD = " + tcode;

            DataTable orcDt = new DataTable();
            var results = new List<WEB_ORDER_CNV>();
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
                            results.Add(new WEB_ORDER_CNV
                                (
                                    row.Field<short>("HOST_TORIHIKISAKI_CD"),
                                    row.Field<int>("KENPINLIST_NO"),
                                    row.Field<string>("DENPRT_BUNRUI"),
                                    row.Field<short>("HOST_BUNRUI_CD"),
                                    row.Field<string>("HOST_SYOHIN_CD"),
                                    row.Field<short>("ORDERLIST_SIZECOLOR_CD"),
                                    row.Field<string>("DENPRT_SYOHIN_CODE"),
                                    row.Field<string>("SIZECOLOR_SETSUMEI4"),
                                    row.Field<string>("SYOHIN_NAME1"),
                                    row.Field<string>("SIZECOLOR_NAME1"),
                                    row.Field<int>("NETUKE_HBAIKA"),
                                    row.Field<int>("NETUKE_ZBAIKA"),
                                    row.Field<string>("LOCATION_CD"),
                                    row.Field<int>("HACHU_SU")
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

        public List<WEB_ORDER_CNV> QueryWherekenpinListNo(string kenpinListNo)
        {
            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAGO2.WEB_ORDER_CNV " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " KENPINLIST_NO = " + kenpinListNo;

            DataTable orcDt = new DataTable();
            var results = new List<WEB_ORDER_CNV>();
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
                            results.Add(new WEB_ORDER_CNV
                                (
                                    row.Field<short>("HOST_TORIHIKISAKI_CD"),
                                    row.Field<int>("KENPINLIST_NO"),
                                    row.Field<string>("DENPRT_BUNRUI"),
                                    row.Field<short>("HOST_BUNRUI_CD"),
                                    row.Field<string>("HOST_SYOHIN_CD"),
                                    row.Field<short>("ORDERLIST_SIZECOLOR_CD"),
                                    row.Field<string>("DENPRT_SYOHIN_CODE"),
                                    row.Field<string>("SIZECOLOR_SETSUMEI4"),
                                    row.Field<string>("SYOHIN_NAME1"),
                                    row.Field<string>("SIZECOLOR_NAME1"),
                                    row.Field<int>("NETUKE_HBAIKA"),
                                    row.Field<int>("NETUKE_ZBAIKA"),
                                    row.Field<string>("LOCATION_CD"),
                                    row.Field<int>("HACHU_SU")
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
