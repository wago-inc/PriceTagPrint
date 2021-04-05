using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.MDB
{
    public class DB_0127_HANSOKU_BAIKA_CONV
    {
        /// <summary>
        /// 得意先コード
        /// </summary>
        public string 得意先CD { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string 名称 { get; set; }
        /// <summary>
        /// 売単価
        /// </summary>
        public decimal 売単価 { get; set; }
        /// <summary>
        /// 値付売価
        /// </summary>
        public decimal 値付売価 { get; set; }
        /// <summary>
        /// 販促文字2
        /// </summary>
        public string 販促文字2 { get; set; }
        /// <summary>
        /// 販促文字表示名
        /// </summary>
        public string 販促文字表示名 { get; set; }

        public DB_0127_HANSOKU_BAIKA_CONV(string 得意先CD, string 名称, decimal 売単価, decimal 値付売価, string 販促文字2, string 販促文字表示名)
        {
            this.得意先CD = 得意先CD;
            this.名称 = 名称;
            this.売単価 = 売単価;
            this.値付売価 = 値付売価;
            this.販促文字2 = 販促文字2;
            this.販促文字表示名 = 販促文字表示名;
        }
    }

    public class DB_0127_HANSOKU_BAIKA_CONV_LIST
    {
        public List<DB_0127_HANSOKU_BAIKA_CONV> list;
        public DB_0127_HANSOKU_BAIKA_CONV_LIST()
        {
            Create();
        }

        private void Create()
        {
            list = new List<DB_0127_HANSOKU_BAIKA_CONV>()
            {
                new DB_0127_HANSOKU_BAIKA_CONV("000127", "3ﾖﾘ", 350, 980, "1", "よりどり３点"),
            };
        }
    }
}
