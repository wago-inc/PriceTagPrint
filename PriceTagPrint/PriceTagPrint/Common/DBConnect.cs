using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Common
{
    public static class DBConnect
    {
        public const string OrclConnectString = "User ID=WAG_USR1; Password=P; Data Source=ORCL.world;";

        public const string MdbDataSource = @"\\Server00\h\database\得意先商品台帳\得意先商品台帳.mdb";

        public static string MdbConnectionString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=" + MdbDataSource;
    }
}
