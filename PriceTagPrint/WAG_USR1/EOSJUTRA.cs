using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace PriceTagPrint.WAG_USR1
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
        /// 行番号
        /// </summary>
        public string VROWNO { get; set; }
        /// <summary>
        /// 分類コード
        /// </summary>
        public string VBUNCD { get; set; }        
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
        /// カラーコード
        /// </summary>
        public string VCOLCD { get; set; }
        /// <summary>
        /// カラー名
        /// </summary>
        public string VCOLNM { get; set; }
        /// <summary>
        /// サイズコード
        /// </summary>
        public string VSIZCD { get; set; }
        /// <summary>
        /// サイズ名
        /// </summary>
        public string VSIZNM { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string VHINNMA { get; set; }
        /// <summary>
        /// 規格名
        /// </summary>
        public string VHINNMB { get; set; }
        /// <summary>
        /// 入数
        /// </summary>
        public decimal VIRISU { get; set; }
        /// <summary>
        /// 発行枚数
        /// </summary>
        public decimal VSURYO { get; set; }
        /// <summary>
        /// 本体原価
        /// </summary>
        public decimal VGNKTK { get; set; }
        /// <summary>
        /// 本体売価
        /// </summary>
        public decimal VURITK { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// JAN13桁(マルヨシスマクラ対応)
        /// </summary>
        public string VCYOBI1 { get; set; }
        /// <summary>
        /// 北海道三喜 大中分類
        /// </summary>
        public string VCYOBI3 { get; set; }        
        /// <summary>
        /// JAN13桁
        /// </summary>
        public string VCYOBI7 { get; set; }
        /// <summary>
        /// 天満屋EOSコード
        /// </summary>
        public string VCYOBI11 { get; set; }
        /// <summary>
        /// 用途区分(FLG)
        /// </summary>
        public string VTOKKB { get; set; }
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
        /// <param name="vcolcd"></param>
        /// <param name="vsizcd"></param>
        /// <param name="vsiznm"></param>
        /// <param name="vhinnma"></param>
        /// <param name="vsuryo"></param>
        /// <param name="vgnktk"></param>
        /// <param name="vuritk"></param>
        /// <param name="hincd"></param>
        /// <param name="vcyobi7"></param>
        public EOSJUTRA(string datno, string vryohncd, string vrcvdt, string vrowno, string vbuncd, 
                        string qoltorid, string vnohindt, string vhincd, string vcolcd, string vcolnm,
                        string vsizcd, string vsiznm, string vhinnma, string vhinnmb, decimal virisu,
                        decimal vsuryo, decimal vgnktk, decimal vuritk, string hincd, string vcyobi1,
                        string vcyobi3, string vcyobi7, string vcyobi11, string vtokkb, string vhead1, string vbody1)
        {
            this.DATNO = datno;
            this.VRYOHNCD = vryohncd;
            this.VRCVDT = vrcvdt;
            this.VROWNO = vrowno;
            this.VBUNCD = vbuncd;            
            this.QOLTORID = qoltorid;
            this.VNOHINDT = vnohindt;
            this.VHINCD = vhincd;
            this.VCOLCD = vcolcd;
            this.VCOLNM = vcolnm;
            this.VSIZCD = vsizcd;
            this.VSIZNM = vsiznm;
            this.VHINNMA = vhinnma;
            this.VHINNMB = vhinnmb;
            this.VIRISU = virisu;
            this.VSURYO = vsuryo;
            this.VURITK = vuritk;
            this.VGNKTK = vgnktk;
            this.HINCD = hincd;
            this.VCYOBI1 = vcyobi1;
            this.VCYOBI3 = vcyobi3;
            this.VCYOBI7 = vcyobi7;
            this.VCYOBI11 = vcyobi11;
            this.VTOKKB = vtokkb;
            this.VHEAD1 = vhead1;
            this.VBODY1 = vbody1;
        }
    }

    public class EOSJUTRA_LIST
    {
        public List<EOSJUTRA> QueryWhereTcodeAndDates(int tcode, DateTime jusin, DateTime nouhin, string bunrui = "", string sttHin = "", string endHin = "")
        {
            var sql = "SELECT " + Environment.NewLine;
            sql += "	DATNO, " + Environment.NewLine;
            sql += "	VRYOHNCD, " + Environment.NewLine;
            sql += "	VRCVDT, " + Environment.NewLine;
            sql += "	VROWNO, " + Environment.NewLine;
            sql += "	VBUNCD, " + Environment.NewLine;
            sql += "	QOLTORID, " + Environment.NewLine;
            sql += "	VNOHINDT, " + Environment.NewLine;
            sql += "	VHINCD, " + Environment.NewLine;
            sql += "	VCOLCD, " + Environment.NewLine;
            sql += "	VCOLNM, " + Environment.NewLine;
            sql += "	VSIZCD, " + Environment.NewLine;
            sql += "	VSIZNM, " + Environment.NewLine;
            sql += "	VHINNMA, " + Environment.NewLine;
            sql += "	VHINNMB, " + Environment.NewLine;
            sql += "	VIRISU, " + Environment.NewLine;
            sql += "	VSURYO, " + Environment.NewLine;
            sql += "	VGNKTK, " + Environment.NewLine;
            sql += "	VURITK, " + Environment.NewLine;
            sql += "	HINCD, " + Environment.NewLine;
            sql += "	VCYOBI1, " + Environment.NewLine;
            sql += "	VCYOBI3, " + Environment.NewLine;
            sql += "	VCYOBI7, " + Environment.NewLine;
            sql += "	VCYOBI11, " + Environment.NewLine;
            sql += "	VTOKKB, " + Environment.NewLine;
            sql += "	VHEAD1, " + Environment.NewLine;
            sql += "	VBODY1 " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAG_USR1.EOSJUTRA " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " DATKB = '1' " + Environment.NewLine;
            sql += " AND VRYOHNCD = '" + tcode.ToString("000000") + "' " + Environment.NewLine;
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
                            results.Add(new EOSJUTRA
                                (
                                    row.Field<string>("DATNO"),
                                    row.Field<string>("VRYOHNCD"), 
                                    row.Field<string>("VRCVDT"),
                                    row.Field<string>("VROWNO"),
                                    row.Field<string>("VBUNCD"),
                                    row.Field<string>("QOLTORID"), 
                                    row.Field<string>("VNOHINDT"), 
                                    row.Field<string>("VHINCD"), 
                                    row.Field<string>("VCOLCD"),
                                    row.Field<string>("VCOLNM"),
                                    row.Field<string>("VSIZCD"), 
                                    row.Field<string>("VSIZNM"),
                                    row.Field<string>("VHINNMA"),
                                    row.Field<string>("VHINNMB"),
                                    row.Field<decimal>("VIRISU"),
                                    row.Field<decimal>("VSURYO"), 
                                    row.Field<decimal>("VGNKTK"),
                                    row.Field<decimal>("VURITK"), 
                                    row.Field<string>("HINCD"),
                                    row.Field<string>("VCYOBI1"),
                                    row.Field<string>("VCYOBI3"),
                                    row.Field<string>("VCYOBI7"),
                                    row.Field<string>("VCYOBI11"),
                                    row.Field<string>("VTOKKB"),
                                    row.Field<string>("VHEAD1"), 
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

        public List<string> QueryDatnoWhereTcodeAndDates(int tcode, DateTime jusin, DateTime nouhin, string bunrui = "", string sttHin = "", string endHin = "")
        {
            var sql = "SELECT " + Environment.NewLine;
            sql += "	DATNO " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAG_USR1.EOSJUTRA " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " DATKB = '1' " + Environment.NewLine;
            sql += " AND VRYOHNCD = '" + tcode.ToString("000000") + "' " + Environment.NewLine;
            sql += " AND VRCVDT = '" + jusin.ToString("yyyyMMdd") + "' " + Environment.NewLine;
            sql += " AND VNOHINDT = '" + nouhin.ToString("yyyyMMdd") + "' " + Environment.NewLine;            
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
            sql += "GROUP BY DATNO";

            DataTable orcDt = new DataTable();
            var results = new List<string>();
            var connectString = DBConnect.OrclConnectString + DBConnect.OrclDataSource;
            try
            {
                using (OracleConnection orcConn = new OracleConnection(connectString))
                {
                    orcConn.Open();

                    OracleCommand cmd = new OracleCommand(sql, orcConn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                results.Add(reader.GetString(0));
                            }
                        }
                    }
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
