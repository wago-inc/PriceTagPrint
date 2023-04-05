using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Models
{
    public class CosmoMatsuokaData
    {
        /// <summary>
        /// 商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string HINNMA { get; set; }
        /// <summary>
        /// サイズカラー
        /// </summary>
        public string HINNMB { get; set; }
        /// <summary>
        /// 品番
        /// </summary>
        public string HINBAN { get; set; }
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string SIRCD { get; set; }
        /// <summary>
        /// 仕入年月日
        /// </summary>
        public string SIRDATE { get; set; }
        /// <summary>
        /// 原価
        /// </summary>
        public long GENKA { get; set; }
        /// <summary>
        /// 売価
        /// </summary>
        public long BAIKA { get; set; }
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCD { get; set; }
        /// <summary>
        /// サブクラスNo
        /// </summary>
        public string SUBCLASSNo { get; set; }
        /// <summary>
        /// 棚番号
        /// </summary>
        public string TNANO { get; set; }
        /// <summary>
        /// 発注数
        /// </summary>
        public int HSU { get; set; }
        /// <summary>
        /// 季節コード
        /// </summary>
        public string SEASONCD { get; set; }
    }
}
