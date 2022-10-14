using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace PriceTagPrint.WAG_USR1
{
    public class EOSAKTRA
    {
        /// <summary>
        /// DATNO
        /// </summary>
        public string DATNO { get; set; }
        /// <summary>
        /// 行番号
        /// </summary>
        public string VROWNO { get; set; }
        /// <summary>
        /// ﾚｺｰﾄﾞ区分
        /// </summary>
        public string AKA001 { get; set; }
        /// <summary>
        /// データ種別
        /// </summary>
        public string AKA002 { get; set; }
        /// <summary>
        /// レコードシーケンス
        /// </summary>
        public string AKA003 { get; set; }
        /// <summary>
        /// 取引先コード
        /// </summary>
        public string AKA004 { get; set; }
        /// <summary>
        /// 仕入先仕訳コ―ド
        /// </summary>
        public string AKA005 { get; set; }
        /// <summary>
        /// 納品日
        /// </summary>
        public string AKA006 { get; set; }
        /// <summary>
        /// 発注日
        /// </summary>
        public string AKA007 { get; set; }
        /// <summary>
        /// 便区分
        /// </summary>
        public string AKA008 { get; set; }
        /// <summary>
        /// 伝票番号
        /// </summary>
        public string AKA009 { get; set; }
        /// <summary>
        /// 発注先企業コード
        /// </summary>
        public string AKA010 { get; set; }
        /// <summary>
        /// 店舗コード
        /// </summary>
        public string AKA011 { get; set; }
        /// <summary>
        /// 納品先コード
        /// </summary>
        public string AKA012 { get; set; }
        /// <summary>
        /// 行番号
        /// </summary>
        public string AKA013 { get; set; }
        /// <summary>
        /// 補充区分
        /// </summary>
        public string AKA014 { get; set; }
        /// <summary>
        /// タグ種別（値札情報）
        /// </summary>
        public string AKA015 { get; set; }
        /// <summary>
        /// 売出区分
        /// </summary>
        public string AKA016 { get; set; }
        /// <summary>
        /// 部門コード
        /// </summary>
        public string AKA017 { get; set; }
        /// <summary>
        /// クラスコード（品種コード）
        /// </summary>
        public string AKA018 { get; set; }
        /// <summary>
        /// 小品種コード（品目コード）
        /// </summary>
        public string AKA019 { get; set; }
        /// <summary>
        /// 自社品番（連番コード）
        /// </summary>
        public string AKA020 { get; set; }
        /// <summary>
        /// カラーコード
        /// </summary>
        public string AKA021 { get; set; }
        /// <summary>
        /// サイズコード
        /// </summary>
        public string AKA022 { get; set; }
        /// <summary>
        /// 取引先コード(仕入先コード)
        /// </summary>
        public string AKA023 { get; set; }
        /// <summary>
        /// 仕入先品番
        /// </summary>
        public string AKA024 { get; set; }
        /// <summary>
        /// タグ情報（値札情報）
        /// </summary>
        public string AKA025 { get; set; }
        /// <summary>
        /// 仕入条件
        /// </summary>
        public string AKA026 { get; set; }
        /// <summary>
        /// 棚番
        /// </summary>
        public string AKA027 { get; set; }
        /// <summary>
        /// 通常売単価
        /// </summary>
        public string AKA028 { get; set; }
        /// <summary>
        /// 初期売単価
        /// </summary>
        public string AKA029 { get; set; }
        /// <summary>
        /// 標準売単価
        /// </summary>
        public string AKA030 { get; set; }
        /// <summary>
        /// バーコード上段
        /// </summary>
        public string AKA031 { get; set; }
        /// <summary>
        /// バーコード下段
        /// </summary>
        public string AKA032 { get; set; }
        /// <summary>
        /// バーコード下段値下
        /// </summary>
        public string AKA033 { get; set; }
        /// <summary>
        /// 発行枚数
        /// </summary>
        public string AKA034 { get; set; }
        /// <summary>
        /// アドレス№
        /// </summary>
        public string AKA035 { get; set; }
        /// <summary>
        /// 季節区分
        /// </summary>
        public string AKA036 { get; set; }
        /// <summary>
        /// 投入月
        /// </summary>
        public string AKA037 { get; set; }
        /// <summary>
        /// 販売終了月
        /// </summary>
        public string AKA038 { get; set; }
        /// <summary>
        /// 色サイズパターンコード
        /// </summary>
        public string AKA039 { get; set; }
    }

    public class EOSAKTRA_LIST
    {
        public List<EOSAKTRA> QueryWhereDatno(string datno)
        {
            var sql = "SELECT " + Environment.NewLine;
            sql += "	DATNO, " + Environment.NewLine;
            sql += "	VROWNO, " + Environment.NewLine;
            sql += "	AKA001, " + Environment.NewLine;
            sql += "	AKA002, " + Environment.NewLine;
            sql += "	AKA003, " + Environment.NewLine;
            sql += "	AKA004, " + Environment.NewLine;
            sql += "	AKA005, " + Environment.NewLine;
            sql += "	AKA006, " + Environment.NewLine;
            sql += "	AKA007, " + Environment.NewLine;
            sql += "	AKA008, " + Environment.NewLine;
            sql += "	AKA009, " + Environment.NewLine;
            sql += "	AKA010, " + Environment.NewLine;
            sql += "	AKA011, " + Environment.NewLine;
            sql += "	AKA012, " + Environment.NewLine;
            sql += "	AKA013, " + Environment.NewLine;
            sql += "	AKA014, " + Environment.NewLine;
            sql += "	AKA015, " + Environment.NewLine;
            sql += "	AKA016, " + Environment.NewLine;
            sql += "	AKA017, " + Environment.NewLine;
            sql += "	AKA018, " + Environment.NewLine;
            sql += "	AKA019, " + Environment.NewLine;
            sql += "	AKA020, " + Environment.NewLine;
            sql += "	AKA021, " + Environment.NewLine;
            sql += "	AKA022, " + Environment.NewLine;
            sql += "	AKA023, " + Environment.NewLine;
            sql += "	AKA024, " + Environment.NewLine;
            sql += "	AKA025, " + Environment.NewLine;
            sql += "	AKA026, " + Environment.NewLine;
            sql += "	AKA027, " + Environment.NewLine;
            sql += "	AKA028, " + Environment.NewLine;
            sql += "	AKA029, " + Environment.NewLine;
            sql += "	AKA030, " + Environment.NewLine;
            sql += "	AKA031, " + Environment.NewLine;
            sql += "	AKA032, " + Environment.NewLine;
            sql += "	AKA033, " + Environment.NewLine;
            sql += "	AKA034, " + Environment.NewLine;
            sql += "	AKA035, " + Environment.NewLine;
            sql += "	AKA036, " + Environment.NewLine;
            sql += "	AKA037, " + Environment.NewLine;
            sql += "	AKA038, " + Environment.NewLine;
            sql += "	AKA039 " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAG_USR1.EOSAKTRA " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " DATNO = '" + datno + "' ";

            DataTable orcDt = new DataTable();
            var results = new List<EOSAKTRA>();
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
                                results.Add(new EOSAKTRA()
                                {
                                    DATNO = reader.GetString(0),
                                    VROWNO = reader.GetString(1),
                                    AKA001 = reader.GetString(2),
                                    AKA002 = reader.GetString(3),
                                    AKA003 = reader.GetString(4),
                                    AKA004 = reader.GetString(5),
                                    AKA005 = reader.GetString(6),
                                    AKA006 = reader.GetString(7),
                                    AKA007 = reader.GetString(8),
                                    AKA008 = reader.GetString(9),
                                    AKA009 = reader.GetString(10),
                                    AKA010 = reader.GetString(11),
                                    AKA011 = reader.GetString(12),
                                    AKA012 = reader.GetString(13),
                                    AKA013 = reader.GetString(14),
                                    AKA014 = reader.GetString(15),
                                    AKA015 = reader.GetString(16),
                                    AKA016 = reader.GetString(17),
                                    AKA017 = reader.GetString(18),
                                    AKA018 = reader.GetString(19),
                                    AKA019 = reader.GetString(20),
                                    AKA020 = reader.GetString(21),
                                    AKA021 = reader.GetString(22),
                                    AKA022 = reader.GetString(23),
                                    AKA023 = reader.GetString(24),
                                    AKA024 = reader.GetString(25),
                                    AKA025 = reader.GetString(26),
                                    AKA026 = reader.GetString(27),
                                    AKA027 = reader.GetString(28),
                                    AKA028 = reader.GetString(29),
                                    AKA029 = reader.GetString(30),
                                    AKA030 = reader.GetString(31),
                                    AKA031 = reader.GetString(32),
                                    AKA032 = reader.GetString(33),
                                    AKA033 = reader.GetString(34),
                                    AKA034 = reader.GetString(35),
                                    AKA035 = reader.GetString(36),
                                    AKA036 = reader.GetString(37),
                                    AKA037 = reader.GetString(38),
                                    AKA038 = reader.GetString(39),
                                    AKA039 = reader.GetString(40)
                                });
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

        public List<string> QueryNefudanoWhereDatno(int tcode, DateTime jusin, DateTime nouhin, string bunrui)
        {
            var subq = "SELECT " + Environment.NewLine;
            subq += "	SUB.DATNO " + Environment.NewLine;
            subq += "FROM " + Environment.NewLine;
            subq += " WAG_USR1.EOSJUTRA SUB " + Environment.NewLine;
            subq += "WHERE " + Environment.NewLine;
            subq += " MAIN.DATNO = SUB.DATNO";
            subq += " AND VRCVDT = '" + jusin.ToString("yyyyMMdd") + "' " + Environment.NewLine;
            subq += " AND VNOHINDT = '" + nouhin.ToString("yyyyMMdd") + "' " + Environment.NewLine;
            subq += " AND VBUNCD = '" + bunrui + "' " + Environment.NewLine;

            var sql = "SELECT " + Environment.NewLine;
            sql += "	MAIN.AKA015 " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAG_USR1.EOSAKTRA MAIN " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " EXISTS (" + subq + ") " + Environment.NewLine;
            sql += "GROUP BY MAIN.AKA015";

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
