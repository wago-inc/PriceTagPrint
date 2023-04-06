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
        /// 表示用自社品番
        /// </summary>
        public string DISPAKHINBAN { get; set; }
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
        /// 表示用和合品番
        /// </summary>
        public string DISPHINBAN { get; set; }
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

    public class AKBUNRUICD
    {
        public string BUMOCD { get; set; }
        public string CLASSCD { get; set; }
        public string BUNRUICD { get; set; }
        public string NAME { get; set; }

        public AKBUNRUICD(string bUMOCD, string cLASSCD, string bUNRUICD, string nAME)
        {
            BUMOCD = bUMOCD;
            CLASSCD = cLASSCD;
            BUNRUICD = bUNRUICD;
            NAME = !string.IsNullOrEmpty(bUNRUICD) ? bUNRUICD + "：" + nAME : "";
        }
    }

    public class AKBUNRUICDLIST
    {

        public List<AKBUNRUICD> list;
        public AKBUNRUICDLIST()
        {
            Create();
        }

        private void Create()
        {
            list = new List<AKBUNRUICD>() 
            {
                new AKBUNRUICD("100", "1", "1001", "ミッシートレンド"),
                new AKBUNRUICD("100", "2", "1002", "ミッシークラシック"),
                new AKBUNRUICD("100", "3", "1003", "ミセスクラシック"),
                new AKBUNRUICD("100", "4", "1004", "ノンエイジ"),
                new AKBUNRUICD("100", "5", "1005", "オケージョン"),
                new AKBUNRUICD("110", "1", "1101", "ミッシートレンド"),
                new AKBUNRUICD("110", "2", "1102", "ミッシークラシック"),
                new AKBUNRUICD("110", "3", "1103", "ミセスクラシック"),
                new AKBUNRUICD("110", "4", "1104", "ノンエイジ"),
                new AKBUNRUICD("110", "5", "1105", "オケージョン"),
                new AKBUNRUICD("120", "1", "1201", "ティーンズトレンド"),
                new AKBUNRUICD("120", "2", "1202", "ミッシートレンド"),
                new AKBUNRUICD("120", "3", "1203", "ノンエイジ"),
                new AKBUNRUICD("120", "4", "1204", "ミッシークラシック"),
                new AKBUNRUICD("130", "1", "1301", "レギュラーサイズコンテンポラリー"),
                new AKBUNRUICD("130", "2", "1302", "レギュラーサイズコンサバティブ"),
                new AKBUNRUICD("130", "3", "1303", "レギュラーサイズコーディネイト"),
                new AKBUNRUICD("130", "4", "1304", "レギュラーサイズベーシック"),
                new AKBUNRUICD("130", "5", "1305", "Ｌサイズコンテンポラリー"),
                new AKBUNRUICD("130", "6", "1306", "Ｌサイズコンサバティブ"),
                new AKBUNRUICD("130", "7", "1307", "Ｌサイズコーディネイト"),
                new AKBUNRUICD("130", "8", "1308", "Ｌサイズベーシック"),
                new AKBUNRUICD("140", "1", "1401", "半製品"),
                new AKBUNRUICD("140", "2", "1402", "製品"),
                new AKBUNRUICD("150", "1", "1501", "レギュラーサイズコンテンポラリー"),
                new AKBUNRUICD("150", "2", "1502", "レギュラーサイズコンサバティブ"),
                new AKBUNRUICD("150", "3", "1503", "レギュラーサイズコーディネイト"),
                new AKBUNRUICD("150", "4", "1504", "レギュラーサイズベーシック"),
                new AKBUNRUICD("150", "5", "1505", "Ｌサイズコンテンポラリー"),
                new AKBUNRUICD("150", "6", "1506", "Ｌサイズコンサバティブ"),
                new AKBUNRUICD("150", "7", "1507", "Ｌサイズコーディネイト"),
                new AKBUNRUICD("150", "8", "1508", "Ｌサイズベーシック"),
                new AKBUNRUICD("160", "1", "1601", "レディースティーンズ"),
                new AKBUNRUICD("160", "2", "1602", "レディースミッシー"),
                new AKBUNRUICD("160", "3", "1603", "レディースミセス"),
                new AKBUNRUICD("160", "4", "1604", "メンズ"),
                new AKBUNRUICD("160", "5", "1605", "ボーイズ"),
                new AKBUNRUICD("160", "6", "1606", "ガールズ"),
                new AKBUNRUICD("160", "7", "1607", "ノンエイジノンセックス"),
                new AKBUNRUICD("170", "1", "1701", "トドラーボーイズ"),
                new AKBUNRUICD("170", "2", "1702", "トドラーガールズ"),
                new AKBUNRUICD("170", "3", "1703", "スクールボーイズ"),
                new AKBUNRUICD("170", "4", "1704", "スクールガールズ"),
                new AKBUNRUICD("170", "5", "1705", "オンスクール"),
                new AKBUNRUICD("180", "1", "1801", "インナー"),
                new AKBUNRUICD("180", "2", "1802", "ファッショングッズ"),
                new AKBUNRUICD("180", "3", "1803", "消耗品"),
                new AKBUNRUICD("180", "4", "1804", "スリーピング"),
                new AKBUNRUICD("180", "5", "1805", "リビング"),
                new AKBUNRUICD("180", "6", "1806", "アウター"),
                new AKBUNRUICD("180", "7", "1807", "マタニティ"),
                new AKBUNRUICD("200", "1", "2001", "ヤング"),
                new AKBUNRUICD("200", "2", "2002", "ノンエイジ"),
                new AKBUNRUICD("210", "1", "2101", "ヤングアダルト"),
                new AKBUNRUICD("210", "2", "2102", "アダルト"),
                new AKBUNRUICD("210", "3", "2103", "シニア"),
                new AKBUNRUICD("210", "4", "2104", "ノンエイジ"),
                new AKBUNRUICD("220", "1", "2201", "ヤングトレンド"),
                new AKBUNRUICD("220", "2", "2202", "ノンエイジ"),
                new AKBUNRUICD("220", "3", "2203", "ファッショングッズ"),
                new AKBUNRUICD("230", "1", "2301", "オンビジネス"),
                new AKBUNRUICD("230", "2", "2302", "オフビジネス"),
                new AKBUNRUICD("230", "3", "2303", "フォーマル"),
                new AKBUNRUICD("230", "4", "2304", "グッズ"),
                new AKBUNRUICD("240", "1", "2401", "メンズトップス"),
                new AKBUNRUICD("240", "2", "2402", "メンズボトムス"),
                new AKBUNRUICD("240", "3", "2403", "メンズノンエイジインナー"),
                new AKBUNRUICD("240", "4", "2404", "ボーイズトドラー"),
                new AKBUNRUICD("240", "5", "2405", "ガールズトドラー"),
                new AKBUNRUICD("240", "6", "2406", "ボーイズスクール"),
                new AKBUNRUICD("240", "7", "2407", "ガールズスクール"),
                new AKBUNRUICD("250", "1", "2501", "ティーンズ"),
                new AKBUNRUICD("250", "2", "2502", "ミッシー"),
                new AKBUNRUICD("250", "3", "2503", "ミセス"),
                new AKBUNRUICD("250", "4", "2504", "ハイミセス"),
                new AKBUNRUICD("250", "5", "2505", "ノンエイジ"),
                new AKBUNRUICD("260", "1", "2601", "レディースミッシー"),
                new AKBUNRUICD("260", "2", "2602", "レディースミセス"),
                new AKBUNRUICD("260", "3", "2603", "メンズヤング"),
                new AKBUNRUICD("260", "4", "2604", "メンズアダルト"),
                new AKBUNRUICD("260", "5", "2605", "ボーイズトドラー"),
                new AKBUNRUICD("260", "6", "2606", "ガールズトドラー"),
                new AKBUNRUICD("260", "7", "2607", "ボーイズスクール"),
                new AKBUNRUICD("260", "8", "2608", "ガールズスクール"),
                new AKBUNRUICD("270","1","2701","レディーススリーピング"),
                new AKBUNRUICD("270","2","2702","メンズスリーピング"),
                new AKBUNRUICD("270","3","2703","ボーイズトドラースリーピング"),
                new AKBUNRUICD("270","4","2704","ガールズトドラースリーピング"),
                new AKBUNRUICD("270","5","2705","ボーイズスクールスリーピング"),
                new AKBUNRUICD("270","6","2706","ガールズスクールスリーピング"),
                new AKBUNRUICD("270","7","2707","レディースホームウェア"),
                new AKBUNRUICD("270","8","2708","メンズ・ジュニアホームウェア"),
                new AKBUNRUICD("280","1","2801","ティーンズ"),
                new AKBUNRUICD("280","2","2802","ミッシー"),
                new AKBUNRUICD("280","3","2803","ミセス"),
                new AKBUNRUICD("280","4","2804","ハイミセス"),
                new AKBUNRUICD("280","5","2805","ノンエイジ"),
                new AKBUNRUICD("350","1","3501","レディース"),
                new AKBUNRUICD("350","2","3502","メンズ"),
                new AKBUNRUICD("350","3","3503","ボーイズ"),
                new AKBUNRUICD("350","4","3504","ガールズ"),
                new AKBUNRUICD("350","5","3505","ノンセックス"),
                new AKBUNRUICD("360","1","3601","レディースティーンズ"),
                new AKBUNRUICD("360","2","3602","レディースミッシー"),
                new AKBUNRUICD("360","3","3603","レディースミセス"),
                new AKBUNRUICD("360","4","3604","メンズ"),
                new AKBUNRUICD("360","5","3605","ボーイズ"),
                new AKBUNRUICD("360","6","3606","ガールズ"),
                new AKBUNRUICD("360","7","3607","オンスクール"),
                new AKBUNRUICD("360","8","3608","グッズ"),
                new AKBUNRUICD("370","1","3701","ミッシートレンド"),
                new AKBUNRUICD("370","2","3702","ミッシークラシック"),
                new AKBUNRUICD("370","3","3703","ミセスクラシック"),
                new AKBUNRUICD("370","4","3704","ノンエイジ"),
                new AKBUNRUICD("370","5","3705","オケージョン"),
                new AKBUNRUICD("380","2","3802","ミセス"),
                new AKBUNRUICD("380","3","3803","ハイミセス"),
                new AKBUNRUICD("380","4","3804","ノンエイジ"),
                new AKBUNRUICD("380","5","3805","オケージョン"),
                new AKBUNRUICD("410", "1", "410", "重寝具"),
                new AKBUNRUICD("410", "2", "410", "軽寝具"),
                new AKBUNRUICD("410", "3", "410", "ホームグッズ"),
                new AKBUNRUICD("450", "1", "450", "リビングルーム"),
                new AKBUNRUICD("450", "2", "450", "ジュニアルーム"),
                new AKBUNRUICD("450", "3", "450", "キッチン"),
                new AKBUNRUICD("450", "4", "450", "サニタリー"),
                new AKBUNRUICD("450", "5", "450", "ホール"),
                new AKBUNRUICD("660", "1", "660", "日用品"),
                new AKBUNRUICD("670", "1", "670", "生活雑貨"),
            };
        }
    }
}
