using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Models
{
    public class OkinawaSankiData
    {
        /// <summary>
        /// 発注No
        /// </summary>
        public int HNO { get; set; }
        /// <summary>
        /// 取引先CD
        /// </summary>
        public string TOKCD { get; set; }
        /// <summary>
        /// 値札No
        /// </summary>
        public string NEFUDA_KBN { get; set; }
        /// <summary>
        /// EOS
        /// </summary>
        public string EOS { get; set; }
        /// <summary>
        /// 業者コード
        /// </summary>
        public string JIISYA { get; set; }
        /// <summary>
        /// 部門
        /// </summary>
        public string HINBANCD { get; set; }
        /// <summary>
        /// 部門
        /// </summary>
        public string CYUBUNCD { get; set; }
        /// <summary>
        /// 販売価格
        /// </summary>
        public int BAIKA { get; set; }
        /// <summary>
        /// JANフリー
        /// </summary>
        public string SYOHINCD { get; set; }
        /// <summary>
        /// メッセージ
        /// </summary>
        public string BIKOU1 { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// センターコード
        /// </summary>
        public string CENTCD { get; set; }
        /// <summary>
        /// 発行枚数
        /// </summary>
        public int NSU { get; set; }        
    }
}
