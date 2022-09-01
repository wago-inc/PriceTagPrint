using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTagPrint.Common
{
    public class UninstallApp
    {
        /// <summary>
        /// 表示名
        /// </summary>
        public String DisplayName { set; get; }
        /// <summary>
        /// 表示バージョン
        /// </summary>
        public String DisplayVersion { set; get; }
        /// <summary>
        /// インストール日次
        /// </summary>
        public String InstallDate { set; get; }
        /// <summary>
        /// 発行元
        /// </summary>
        public String Publisher { set; get; }
    }
}
