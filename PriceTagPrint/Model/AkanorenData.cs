using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTagPrint.Model
{
    public class AkanorenData
    {
        /// <summary>
        /// ﾚｺｰﾄﾞ区分
        /// </summary>
        public string RECTYPE { get; set; }
        /// <summary>
        /// データ種別
        /// </summary>
        public string DTKIND { get; set; }
        /// <summary>
        /// レコードシーケンス
        /// </summary>
        public string RECSEQ { get; set; }
        /// <summary>
        /// 取引先コード(13桁0埋め)
        /// </summary>
        public string TORISAKICD13 { get; set; }
        /// <summary>
        /// 仕入先仕訳コ―ド
        /// </summary>
        public string SIRSIWCD { get; set; }
        /// <summary>
        /// 納品日
        /// </summary>
        public string NOHINDT { get; set; }
        /// <summary>
        /// 発注日
        /// </summary>
        public string HATDT { get; set; }
        /// <summary>
        /// 便区分
        /// </summary>
        public string BINTYPE { get; set; }
        /// <summary>
        /// 伝票番号
        /// </summary>
        public string DENNO { get; set; }
        /// <summary>
        /// 発注先企業コード
        /// </summary>
        public string HATSAKICDE { get; set; }
        /// <summary>
        /// 店舗コード
        /// </summary>
        public string TENPOCD { get; set; }
        /// <summary>
        /// 納品先コード
        /// </summary>
        public string NOHINSAKICD { get; set; }
        /// <summary>
        /// 行番号
        /// </summary>
        public string ROWNO { get; set; }
        /// <summary>
        /// 補充区分
        /// </summary>
        public string HOJUTYPE { get; set; }
        /// <summary>
        /// タグ種別（値札情報）
        /// </summary>
        public string TAGKIND { get; set; }
        /// <summary>
        /// 売出区分
        /// </summary>
        public string URITYPE { get; set; }
        /// <summary>
        /// 部門コード
        /// </summary>
        public string BUMOCD { get; set; }
        /// <summary>
        /// クラスコード（品種コード）
        /// </summary>
        public string CLASSCD { get; set; }
        /// <summary>
        /// 小品種コード（品目コード）
        /// </summary>
        public string SHOHINSYUCD { get; set; }
        /// <summary>
        /// 自社品番（連番コード）
        /// </summary>
        public string AKHINBAN { get; set; }
        /// <summary>
        /// カラーコード
        /// </summary>
        public string COLCD { get; set; }
        /// <summary>
        /// サイズコード
        /// </summary>
        public string SIZCD { get; set; }
        /// <summary>
        /// 取引先コード(仕入先コード)
        /// </summary>
        public string TORISAKICD { get; set; }
        /// <summary>
        /// 仕入先品番
        /// </summary>
        public string WGHINBAN { get; set; }
        /// <summary>
        /// タグ情報（値札情報）
        /// </summary>
        public string TAGINFO { get; set; }
        /// <summary>
        /// 仕入条件
        /// </summary>
        public string SIRJOKEN { get; set; }
        /// <summary>
        /// 棚番
        /// </summary>
        public string TNACD { get; set; }
        /// <summary>
        /// 通常売単価
        /// </summary>
        public decimal BTANKA { get; set; }
        /// <summary>
        /// 初期売単価
        /// </summary>
        public decimal STANKA { get; set; }
        /// <summary>
        /// 標準売単価
        /// </summary>
        public decimal HTANKA { get; set; }
        /// <summary>
        /// バーコード上段
        /// </summary>
        public string JANCD01 { get; set; }
        /// <summary>
        /// バーコード下段
        /// </summary>
        public string JANCD02 { get; set; }
        /// <summary>
        /// バーコード下段値下
        /// </summary>
        public string JANCD02SAGE { get; set; }
        /// <summary>
        /// 発行枚数
        /// </summary>
        public decimal MAISU { get; set; }
        /// <summary>
        /// アドレス№
        /// </summary>
        public string ADRNO { get; set; }
        /// <summary>
        /// 季節区分
        /// </summary>
        public string SEASON { get; set; }
        /// <summary>
        /// 投入月
        /// </summary>
        public string TONYTUKI { get; set; }
        /// <summary>
        /// 販売終了月
        /// </summary>
        public string ENDTUKI { get; set; }
        /// <summary>
        /// 色サイズパターンコード
        /// </summary>
        public string COLSIZPTCD { get; set; }
    }
}
