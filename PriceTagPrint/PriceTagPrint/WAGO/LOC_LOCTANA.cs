using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace PriceTagPrint.WAGO
{
    public class LOC_LOCTANA
    {
        public int LOCTANA_SOKO_CODE { get; set; }
        public int LOCTANA_FLOOR_NO { get; set; }
        public int LOCTANA_TANA_NO { get; set; }
        public int LOCTANA_CASE_NO { get; set; }
        public int LOCTANA_SYOHIN_CODE { get; set; }
        public int LOCTANA_SIZECOLOR_CODE { get; set; }

        public LOC_LOCTANA(int soko_code, int floor_no, int tana_no, int case_no,
                           int syohin_code, int sizecolor_code)
        {
            this.LOCTANA_SOKO_CODE = soko_code;
            this.LOCTANA_FLOOR_NO = floor_no;
            this.LOCTANA_TANA_NO = tana_no;
            this.LOCTANA_CASE_NO = case_no;
            this.LOCTANA_SYOHIN_CODE = syohin_code;
            this.LOCTANA_SIZECOLOR_CODE = sizecolor_code;
        }
    }

    public class LOC_LOCTANA_LIST
    {
        public List<LOC_LOCTANA> QueryWhereWagoAllLocation()
        {
            var sql = "SELECT " + Environment.NewLine;
            sql += " LOCTANA_SOKO_CODE, " + Environment.NewLine;
            sql += " LOCTANA_FLOOR_NO, " + Environment.NewLine;
            sql += " LOCTANA_TANA_NO, " + Environment.NewLine;
            sql += " LOCTANA_CASE_NO, " + Environment.NewLine;
            sql += " LOCTANA_SYOHIN_CODE, " + Environment.NewLine;
            sql += " LOCTANA_SIZECOLOR_CODE " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " WAGO.LOC_LOCTANA " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " LOCTANA_SOKO_CODE <> 1 ";

            DataTable orcDt = new DataTable();
            var results = new List<LOC_LOCTANA>();
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
                            results.Add(new LOC_LOCTANA
                                (
                                    row.Field<short>("LOCTANA_SOKO_CODE"),
                                    row.Field<short>("LOCTANA_FLOOR_NO"),
                                    row.Field<short>("LOCTANA_TANA_NO"),
                                    row.Field<short>("LOCTANA_CASE_NO"),
                                    row.Field<int>("LOCTANA_SYOHIN_CODE"),
                                    row.Field<short>("LOCTANA_SIZECOLOR_CODE")
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
