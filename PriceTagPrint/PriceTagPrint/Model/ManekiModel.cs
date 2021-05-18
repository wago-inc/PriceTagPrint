using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Model
{
    public class ManekiData
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
        /// SKU管理番号
        /// </summary>
        public int SKU { get; set; }
        /// <summary>
        /// アイテムコード
        /// </summary>
        public int ITEMCD { get; set; }
        /// <summary>
        /// 定番区分
        /// </summary>
        public int TKBN { get; set; }
        /// <summary>
        /// 条件テーブルCD
        /// </summary>
        public string JTBLCD { get; set; }
        /// <summary>
        /// サイズ
        /// </summary>
        public int SAIZU { get; set; }
        /// <summary>
        /// カラー（COLCDは同じもの）
        /// </summary>
        public int COLCD { get; set; }
        /// <summary>
        /// 部門CD
        /// </summary>
        public int BUMON { get; set; }
        /// <summary>
        /// 下代変換CD
        /// </summary>
        public int HENCD { get; set; }
        /// <summary>
        /// 参考上代
        /// </summary>
        public int JYODAI { get; set; }
        /// <summary>
        /// 販売単価
        /// </summary>
        public int HTANKA { get; set; }
        /// <summary>
        /// 商品名（日本語）
        /// </summary>
        public string HINMEIN { get; set; }
        /// <summary>
        /// 仕入単価
        /// </summary>
        public int STANKA { get; set; }
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
        /// 出荷数
        /// </summary>
        public int TSU { get; set; }
        /// <summary>
        /// 値札区分
        /// </summary>
        public int NEFUDA_KBN { get; set; }

        public int LOCTANA_SYOHIN_CD { get; set; }
    }
}
