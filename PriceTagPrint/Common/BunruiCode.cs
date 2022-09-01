﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Common
{    
    public class BunruiCode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public BunruiCode(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }

    public class BunruiCodeList
    {
        public List<BunruiCode> list;
        public BunruiCodeList()
        {
            Create();
        }

        private void Create()
        {
            var connectString = DBConnect.OrclConnectString + DBConnect.OrclDataSource;
            try
            {
                using (OracleConnection con = new OracleConnection(connectString))
                {
                    con.Open();

                    string sql = "SELECT * FROM WAG_USR1.CLSMTA WHERE CLSKB = 7";
                    OracleCommand cmd = new OracleCommand(sql, con);

                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list = new List<BunruiCode>();
                        list.Add(new BunruiCode(reader["CLSID"] as string, reader["CLSNM"] as string));
                    }

                    reader.Close();
                    con.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public List<BunruiCode> GetBunruiCodes()
        {
            var results = new List<BunruiCode>();
            var connectString = DBConnect.OrclConnectString + DBConnect.OrclDataSource;
            try
            {
                using (OracleConnection con = new OracleConnection(connectString))
                {
                    con.Open();

                    string sql = "SELECT * FROM WAG_USR1.CLSMTA WHERE CLSKB = 7";
                    OracleCommand cmd = new OracleCommand(sql, con);

                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = reader["CLSID"] as string;
                        var name = reader["CLSNM"] as string;
                        string[] strs = new string[] { id.TrimEnd(), name.TrimEnd() };
                        var dispName = string.Join("：", strs);
                        results.Add(new BunruiCode(id, dispName));
                    }

                    reader.Close();
                    con.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return results;
        }
    }
}
