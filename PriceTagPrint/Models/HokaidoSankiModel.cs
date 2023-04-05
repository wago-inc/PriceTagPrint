using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Models
{
    public　class HokaidoSankiData
    {
        /// <summary>
        /// 量販店コード
        /// </summary>
        public string VRYOHNCD { get; set; }
        /// <summary>
        /// 量販店名
        /// </summary>
        public string VRYOHNNM { get; set; }
        /// <summary>
        /// 受信日付
        /// </summary>
        public string VRCVDT { get; set; }
        /// <summary>
        /// 納品日付
        /// </summary>
        public string VNOHINDT { get; set; }
        /// <summary>
        /// 発行枚数
        /// </summary>
        public decimal VSURYO { get; set; }
        /// <summary>
        /// 大中分類
        /// </summary>
        public string VCYOBI3 { get; set; }
        /// <summary>
        /// EOS商品コード
        /// </summary>
        public string VCYOBI7 { get; set; }
        /// <summary>
        /// 売価
        /// </summary>
        public decimal VURITK { get; set; }
        /// <summary>
        /// JANコード
        /// </summary>
        public string VHINCD { get; set; }
        /// <summary>
        /// 取引先コード
        /// </summary>
        public string QOLTORID { get; set; }
        /// <summary>
        /// 品番
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string HINNM { get; set; }
        /// <summary>
        /// EOS品名
        /// </summary>
        public string EOSHINNM { get; set; }
        /// <summary>
        /// 棚番
        /// </summary>
        public string TNANM { get; set; }
        /// <summary>
        /// 品番のみ
        /// </summary>
        public string HINBAN { get; set; }
        /// <summary>
        /// 品番と枝番（99999-99）
        /// </summary>
        public string HINEDA { get; set; }
    }
}
