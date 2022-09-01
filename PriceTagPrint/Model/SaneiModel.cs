using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Model
{
    public class SaneiData
    {
        /// <summary>
        /// 量販店コード
        /// </summary>
        public string VRYOHNCD { get; set; }
        /// <summary>
        /// 受信日付
        /// </summary>
        public string VRCVDT { get; set; }
        /// <summary>
        /// 納品日付
        /// </summary>
        public string VNOHINDT { get; set; }
        /// <summary>
        /// JANコード
        /// </summary>
        public string VHINCD { get; set; }
        /// <summary>
        /// リスト表示商品コード
        /// </summary>
        public string VHINNMA { get; set; }
        /// <summary>
        /// 商品コード（分類 + 品番 + 枝番）
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// 商品コード（品番のみ）
        /// </summary>
        public string HINID { get; set; }
        /// <summary>
        /// 取引先コード
        /// </summary>
        public string QOLTORID { get; set; }
        /// <summary>
        /// 入数
        /// </summary>
        public decimal VIRISU { get; set; }
        /// <summary>
        /// 売価
        /// </summary>
        public decimal VURITK { get; set; }
        /// <summary>
        /// 発行枚数
        /// </summary>
        public decimal VSURYO { get; set; }
        /// <summary>
        /// 表示用商品コード
        /// </summary>
        public string DSPHINNM { get; set; }
        /// <summary>
        /// サイズコード
        /// </summary>
        public string VSIZNM { get; set; }
        /// <summary>
        /// カラーコード
        /// </summary>
        public string VCOLNM { get; set; }
        /// <summary>
        /// メッセージ
        /// </summary>
        public string MESSAGE { get; set; }        
        /// <summary>
        /// 値札番号
        /// </summary>
        public string NEFUDA { get; set; }
        /// <summary>
        /// 部門コード
        /// </summary>
        public string BUMONCD { get; set; }
        /// <summary>
        /// 追加区分
        /// </summary>
        public string TUIKA_KBN { get; set; }
        /// <summary>
        /// セット情報
        /// </summary>
        public string SET_INFO { get; set; }
    }
}
