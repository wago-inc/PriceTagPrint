﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTagPrint.Model
{
    public class NankokuData
    {
        /// <summary>
        /// 得意先コード
        /// </summary>
        public int TCODE { get; set; }
        /// <summary>
        /// 発注番号
        /// </summary>
        public int HNO { get; set; }
        /// <summary>
        /// 部門CD
        /// </summary>
        public string BUMON { get; set; }
        /// <summary>
        /// 分類コード
        /// </summary>
        public int BUNRUI { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string SCODE { get; set; }
        /// <summary>
        /// 社内サイズ・カラーコード
        /// </summary>
        public string SAIZUS { get; set; }
        /// <summary>
        /// 相手先品番
        /// </summary>
        public string JANCD { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string HINMEI { get; set; }
        /// <summary>
        /// 規格
        /// </summary>
        public string SAIZUN { get; set; }
        /// <summary>
        /// 出荷数
        /// </summary>
        public int TSU { get; set; }
        /// <summary>
        /// 本体売価
        /// </summary>
        public int HTANKA { get; set; }
        /// <summary>
        /// 税込売価
        /// </summary>
        public int ZBAIKA { get; set; }
        /// <summary>
        /// ロケーション倉庫コード
        /// </summary>
        public int LOCTANA_SOKO_CODE { get; set; }
        /// <summary>
        /// ロケーションフロアNo
        /// </summary>
        public int LOCTANA_FLOOR_NO { get; set; }
        /// <summary>
        /// ロケーション棚No
        /// </summary>
        public int LOCTANA_TANA_NO { get; set; }
        /// <summary>
        /// ロケーションケースNo
        /// </summary>
        public int LOCTANA_CASE_NO { get; set; }
    }
}
