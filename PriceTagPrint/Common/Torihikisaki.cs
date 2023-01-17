using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PriceTagPrint.Common
{
    public enum HakkouKind
    {
        None,
        Auto,
        Input,
        Both
    }

    public class DirItem
    {
        public string Path { get; set; }
        public bool IsAuto { get; set; }

        public DirItem(string path, bool isAuto)
        {
            this.Path = path;
            this.IsAuto = isAuto;
        }
    }
    public static class TidNum
    {
        public const int YASUSAKI = 112;
        public const int YAMANAKA = 127;
        public const int MARUYOSI = 102;
        public const int OKINAWA_SANKI = 122;
        public const int WATASEI = 7858;
        public const int AJU = 170;
        public const int ABUABU = 8104;
        public const int ITOGOFUKU = 7705;
        public const int OKADA = 7501;
        public const int KANETA = 7500;
        public const int KYOEI = 101;
        public const int COSMOMATUOKA = 7883;
        public const int SANEI = 117;
        public const int TAIFUKUTOKYO = 7846;
        public const int TENMAYA = 109;
        public const int DOMMY = 129;
        public const int BIGA = 8108;
        public const int FUJI = 103;
        public const int ZENSHOREN = 107;
        public const int FUJIYA = 416;
        public const int HOKKAIDO_SANKI = 121;
        public const int HONTENTAKAHASI = 8103;
        public const int MAXVALUE = 110;
        public const int MARBURU = 8110;
        public const int MANEKI = 2101;
        public const int MIYAMA = 8102;
        public const int YANAGIYA = 7840;
        public const int WORKWAY = 7510;
        public const int SANKI = 118;
        public const int SUNNY_MART = 105;
        public const int SUNNY_MART_NR = 106;
        public const int SANKAKU = 145;
        public const int SEVEN = 5201;
        public const int COPO = 7336;
        public const int MARUJI = 7837;
        public const int MANSYO = 7903;
        public const int MANEI = 7908;
        public const int NANKOKU = 8208;
        public const int IZUMI = 160;
        public const int AKANOREN = 165;
        public const int KEIOSTORE = 8115;
        public const int OTHER01 = -1;
        public const int OTHER02 = -2;
        public const int OTHER03 = -3;
        public const int OTHER04 = -4;
        public const int OTHER05 = -5;
        public const int OTHER06 = -6;
        public const int OTHER07 = -7;
        public const int OTHER08 = -8;
        public const int OTHER09 = -9;
    }

    public static class Tid
    {
        public const string YASUSAKI = "0112";
        public const string YAMANAKA = "0127";
        public const string MARUYOSI = "0102";
        public const string OKINAWA_SANKI = "0122";
        public const string WATASEI = "7858";
        public const string AJU = "0170";
        public const string ABUABU = "8104";
        public const string ITOGOFUKU = "7705";
        public const string OKADA = "7501";
        public const string KANETA = "7500";
        public const string KYOEI = "0101";
        public const string COSMOMATUOKA = "7883";
        public const string SANEI = "0117";
        public const string TAIFUKUTOKYO = "7846";
        public const string TENMAYA = "0109";
        public const string DOMMY = "0129";
        public const string BIGA = "8108";
        public const string FUJI = "0103";
        public const string ZENSHOREN = "0107";
        public const string FUJIYA = "0416";
        public const string HOKKAIDO_SANKI = "0121";
        public const string HONTENTAKAHASI = "8103";
        public const string MAXVALUE = "0110";
        public const string MARBURU = "8110";
        public const string MANEKI = "2101";
        public const string MIYAMA = "8102";
        public const string YANAGIYA = "7840";
        public const string WORKWAY = "7510";
        public const string SANKI = "0118";
        public const string SUNNY_MART = "0105";
        public const string SUNNY_MART_NR = "0106";
        public const string SANKAKU = "0145";
        public const string SEVEN = "5201";
        public const string COPO = "7336";
        public const string MARUJI = "7837";
        public const string MANSYO = "7903";
        public const string MANEI = "7908";
        public const string NANKOKU = "8208";
        public const string IZUMI = "0160";
        public const string AKANOREN = "0165";
        public const string KEIOSTORE = "8115";
        public const string OTHER01 = "OT01";
        public const string OTHER02 = "OT02";
        public const string OTHER03 = "OT03";
        public const string OTHER04 = "OT04";
        public const string OTHER05 = "OT05";
        public const string OTHER06 = "OT06";
    }

    public static class Tnm
    {
        public const string YASUSAKI = "ヤスサキ";
        public const string YAMANAKA = "ヤマナカ";
        public const string MARUYOSI = "マルヨシ";
        public const string OKINAWA_SANKI = "沖縄三喜マルエー";
        public const string WATASEI = "わたせい";
        public const string AJU = "アージュ";
        public const string ABUABU = "アブアブ赤札堂";
        public const string ITOGOFUKU = "イトウゴフク";
        public const string OKADA = "おかだ";
        public const string KANETA = "カネタ";
        public const string KYOEI = "キョーエイ";
        public const string COSMOMATUOKA = "コスモマツオカ";
        public const string SANEI = "サンエー";
        public const string TAIFUKUTOKYO = "タイフク東京";
        public const string TENMAYA = "天満屋ストア";
        public const string DOMMY = "ドミー";
        public const string BIGA = "ビッグエー";
        public const string FUJI = "フジ";
        public const string ZENSHOREN = "全小連";
        public const string FUJIYA = "フジヤ";
        public const string HOKKAIDO_SANKI = "北海道三喜";
        public const string HONTENTAKAHASI = "本店タカハシ";
        public const string MAXVALUE = "マックスバリュ西日本";
        public const string MARBURU = "マーブル";
        public const string MANEKI = "マネキ";
        public const string MIYAMA = "ミヤマ";
        public const string YANAGIYA = "柳屋";
        public const string WORKWAY = "ワークウェイ";
        public const string SANKI = "三喜";
        public const string SUNNY_MART = "サニーマート";
        public const string SUNNY_MART_NR = "サニーマート(日流)";
        public const string SANKAKU = "三角商事";
        public const string SEVEN = "セブン";
        public const string COPO = "コポ";
        public const string MARUJI = "丸治";
        public const string MANSYO = "万勝";
        public const string MANEI = "萬栄";
        public const string NANKOKU = "ナンコクスーパー";
        public const string IZUMI = "イズミ";
        public const string AKANOREN = "あかのれん";
        public const string KEIOSTORE = "京王ストア";
        public const string OTHER01 = "インナー商品シール";
        public const string OTHER02 = "和合ＪＡＮタグ";
        public const string OTHER03 = "品番・サイズ・素材シール";
        public const string OTHER04 = "品質シール";
        public const string OTHER05 = "品番・カラーシール";
        public const string OTHER06 = "祭事商材ラベル";
    }
    public class Torihikisaki
    {
        public int Id { get; set; }
        public string Tcode { get; set; }
        public string Name { get; set; }
        public HakkouKind Kind { get; set; }
        public List<DirItem> LayoutDirs { get; set; }   // ※手入力のみ設定する
        public Torihikisaki(int id, string tcode, string name, HakkouKind kind, List<DirItem> layoutDirs = null)
        {
            this.Id = id;
            this.Tcode = tcode;
            this.Name = name;
            this.Kind = kind;
            this.LayoutDirs = layoutDirs ?? new List<DirItem>();
        }
    }

    public class TorihikisakiList
    {
        public List<Torihikisaki> list;
        public TorihikisakiList()
        {
            Create();
        }

        private void Create()
        {
            var dirs = new List<string>();
            list = new List<Torihikisaki>()
            {
                new Torihikisaki(1,  Tid.YASUSAKI,       Tnm.YASUSAKI,       HakkouKind.Both,  CreateDirList(TidNum.YASUSAKI)),
                new Torihikisaki(2,  Tid.YAMANAKA,       Tnm.YAMANAKA,       HakkouKind.Both,  CreateDirList(TidNum.YAMANAKA)),
                new Torihikisaki(3,  Tid.MARUYOSI,       Tnm.MARUYOSI,       HakkouKind.Both,  CreateDirList(TidNum.MARUYOSI)),
                new Torihikisaki(4,  Tid.OKINAWA_SANKI,  Tnm.OKINAWA_SANKI,  HakkouKind.Auto),
                new Torihikisaki(5,  Tid.WATASEI,        Tnm.WATASEI,        HakkouKind.Both,  CreateDirList(TidNum.WATASEI)),
                new Torihikisaki(6,  Tid.SANKI,          Tnm.SANKI,          HakkouKind.Input, CreateDirList(TidNum.SANKI)),
                new Torihikisaki(7,  Tid.AJU,            Tnm.AJU,            HakkouKind.Input, CreateDirList(TidNum.AJU)),
                new Torihikisaki(8,  Tid.ABUABU,         Tnm.ABUABU,         HakkouKind.Input, CreateDirList(TidNum.ABUABU)),
                new Torihikisaki(9,  Tid.ITOGOFUKU,      Tnm.ITOGOFUKU,      HakkouKind.Both,  CreateDirList(TidNum.ITOGOFUKU)),
                new Torihikisaki(10, Tid.OKADA,          Tnm.OKADA,          HakkouKind.Both,  CreateDirList(TidNum.OKADA)),
                new Torihikisaki(11, Tid.KANETA,         Tnm.KANETA,         HakkouKind.Input, CreateDirList(TidNum.KANETA)),
                new Torihikisaki(12, Tid.KYOEI,          Tnm.KYOEI,          HakkouKind.Both,  CreateDirList(TidNum.KYOEI)),
                new Torihikisaki(13, Tid.COSMOMATUOKA,   Tnm.COSMOMATUOKA,   HakkouKind.Both,  CreateDirList(TidNum.COSMOMATUOKA)),
                new Torihikisaki(14, Tid.SANEI,          Tnm.SANEI,          HakkouKind.Both,  CreateDirList(TidNum.SANEI)),
                new Torihikisaki(15, Tid.TAIFUKUTOKYO,   Tnm.TAIFUKUTOKYO,   HakkouKind.Input, CreateDirList(TidNum.TAIFUKUTOKYO)),
                new Torihikisaki(16, Tid.TENMAYA,        Tnm.TENMAYA,        HakkouKind.Both,  CreateDirList(TidNum.TENMAYA)),
                new Torihikisaki(17, Tid.DOMMY,          Tnm.DOMMY,          HakkouKind.Input, CreateDirList(TidNum.DOMMY)),
                new Torihikisaki(18, Tid.BIGA,           Tnm.BIGA,           HakkouKind.Input, CreateDirList(TidNum.BIGA)),
                new Torihikisaki(19, Tid.FUJI,           Tnm.FUJI,           HakkouKind.Input, CreateDirList(TidNum.FUJI)),
                new Torihikisaki(20, Tid.FUJIYA,         Tnm.FUJIYA,         HakkouKind.Both,  CreateDirList(TidNum.FUJIYA)),
                new Torihikisaki(21, Tid.HOKKAIDO_SANKI, Tnm.HOKKAIDO_SANKI, HakkouKind.Both,  CreateDirList(TidNum.HOKKAIDO_SANKI)),
                new Torihikisaki(22, Tid.HONTENTAKAHASI, Tnm.HONTENTAKAHASI, HakkouKind.Both,  CreateDirList(TidNum.HONTENTAKAHASI)),
                new Torihikisaki(23, Tid.MAXVALUE,       Tnm.MAXVALUE,       HakkouKind.Input, CreateDirList(TidNum.MAXVALUE)),
                new Torihikisaki(24, Tid.MARBURU,        Tnm.MARBURU,        HakkouKind.Input, CreateDirList(TidNum.MARBURU)),
                new Torihikisaki(25, Tid.MANEKI,         Tnm.MANEKI,         HakkouKind.Both,  CreateDirList(TidNum.MANEKI)),
                new Torihikisaki(26, Tid.MIYAMA,         Tnm.MIYAMA,         HakkouKind.Both,  CreateDirList(TidNum.MIYAMA)),
                new Torihikisaki(27, Tid.YANAGIYA,       Tnm.YANAGIYA,       HakkouKind.Input, CreateDirList(TidNum.YANAGIYA)),
                new Torihikisaki(28, Tid.WORKWAY,        Tnm.WORKWAY,        HakkouKind.Input, CreateDirList(TidNum.WORKWAY)),
                new Torihikisaki(29, Tid.SUNNY_MART,     Tnm.SUNNY_MART,     HakkouKind.Input, CreateDirList(TidNum.SUNNY_MART)),
                new Torihikisaki(30, Tid.SANKAKU,        Tnm.SANKAKU,        HakkouKind.Input, CreateDirList(TidNum.SANKAKU)),
                new Torihikisaki(31, Tid.SEVEN,          Tnm.SEVEN,          HakkouKind.Input, CreateDirList(TidNum.SEVEN)),
                new Torihikisaki(32, Tid.COPO,           Tnm.COPO,           HakkouKind.Input, CreateDirList(TidNum.COPO)),
                new Torihikisaki(33, Tid.MARUJI,         Tnm.MARUJI,         HakkouKind.Input, CreateDirList(TidNum.MARUJI)),
                new Torihikisaki(34, Tid.MANSYO,         Tnm.MANSYO,         HakkouKind.Input, CreateDirList(TidNum.MANSYO)),
                new Torihikisaki(35, Tid.MANEI,          Tnm.MANEI,          HakkouKind.Input, CreateDirList(TidNum.MANEI)),
                new Torihikisaki(36, Tid.NANKOKU,        Tnm.NANKOKU,        HakkouKind.Both,  CreateDirList(TidNum.NANKOKU)),
                new Torihikisaki(37, Tid.IZUMI,          Tnm.IZUMI,          HakkouKind.Both,  CreateDirList(TidNum.IZUMI)),
                new Torihikisaki(38, Tid.AKANOREN,       Tnm.AKANOREN,       HakkouKind.Both,  CreateDirList(TidNum.AKANOREN)),
                new Torihikisaki(39, Tid.KEIOSTORE,      Tnm.KEIOSTORE,      HakkouKind.Both,  CreateDirList(TidNum.KEIOSTORE)),
                new Torihikisaki(40, Tid.OTHER01,        Tnm.OTHER01,        HakkouKind.Input, CreateDirList(TidNum.OTHER01)),
                new Torihikisaki(41, Tid.OTHER02,        Tnm.OTHER02,        HakkouKind.Input, CreateDirList(TidNum.OTHER02)),
                new Torihikisaki(42, Tid.OTHER03,        Tnm.OTHER03,        HakkouKind.Input, CreateDirList(TidNum.OTHER03)),
                new Torihikisaki(43, Tid.OTHER04,        Tnm.OTHER04,        HakkouKind.Input, CreateDirList(TidNum.OTHER04)),
                new Torihikisaki(44, Tid.OTHER05,        Tnm.OTHER05,        HakkouKind.Input, CreateDirList(TidNum.OTHER05)),
                new Torihikisaki(45, Tid.OTHER06,        Tnm.OTHER06,        HakkouKind.Input, CreateDirList(TidNum.OTHER06)),
            };
        }

        private List<DirItem> CreateDirList(int tcode)
        {
            switch (tcode)
            {
                case TidNum.AJU:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\アージュ_値札（オンライン発行）総額表示", true)
                    };
                case TidNum.ABUABU:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\アブアブ赤札堂_V5_ST308R（2021総額対応）", true)
                    };
                case TidNum.ITOGOFUKU:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\7705_イトウゴフク\2020年総額表示_V5_ST308R\手入力用", false)
                    };
                case TidNum.OKADA:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\わたせい_おかだ\おかだ", false)
                    };
                case TidNum.KANETA:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\カネタ_ビッグエー\ニューライフカネタ", false)
                    };
                case TidNum.KYOEI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0101_キョーエイ\【総額表示】_V5_ST308R\手打ち用", false)
                    };
                case TidNum.COSMOMATUOKA:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\マツオカ RT308R【総額表示】", false)
                    };
                case TidNum.SANEI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\【総額】サンエー_V5_RT308R", false)
                    };
                case TidNum.SANKI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0118_三喜", false),
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0118_三喜_ラベルチャレンジプライス", false)
                    };
                case TidNum.TAIFUKUTOKYO:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\7846_タイフク東京\2020年総額表示_V5_ST308R", false)
                    };
                case TidNum.TENMAYA:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\天満屋ストア値札発行_V5_ST308R", false)
                    };
                case TidNum.DOMMY:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\ドミー【増税_価格併記】_V5_ST308R", false)
                    };
                case TidNum.BIGA:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\カネタ_ビッグエー\ビッグ・エー", false)
                    };
                case TidNum.FUJI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0103_フジ\フジ_総額表示対応_MLV5_ST308R\フジ値下ラベル", false),
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0103_フジ\フジ_総額表示対応_MLV5_ST308R\フジバラエティ", false),
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0103_フジ\フジ_総額表示対応_MLV5_ST308R\フジ値札発行", false)
                    };
                case TidNum.FUJIYA:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\フジヤ【総額表示】", false)
                    };
                case TidNum.HOKKAIDO_SANKI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0121_北海道三喜\【総額新フォーマット】北海道三喜_V5_ST308R", false)
                    };
                case TidNum.HONTENTAKAHASI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\本店タカハシ", false)
                    };
                case TidNum.MAXVALUE:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\マルナカ【2020年改訂第2版】_V5_ST308R", false)
                    };
                case TidNum.MARUYOSI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0102_マルヨシ\マルヨシセンター(総額対応)_V5 ST308R", false)
                    };
                case TidNum.MARBURU:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\マーブル", false)
                    };
                case TidNum.MANEKI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\2101_マネキ\【総額表示】_V5_ST308R", false)
                    };
                case TidNum.MIYAMA:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\ミヤマ", false)
                    };
                case TidNum.YANAGIYA:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\柳屋_総額表示対応_V5_ST308R", false)
                    };
                case TidNum.YASUSAKI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0112_ヤスサキ\【総額対応】ヤスサキ_V5_RT308R_振分発行", true),
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0112_ヤスサキ\【2段併記】ヤスサキ_JAN2段併記_RT308R", false)
                    };
                case TidNum.WATASEI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\7858_わたせい\【総額対応】わたせい_V5_RT308R", false)
                    };
                case TidNum.WORKWAY:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\ワークウェイ（総額対応）", false)
                    };
                case TidNum.SUNNY_MART:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0105_サニーマート", false)
                    };
                case TidNum.SANKAKU:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0145_三角商事", false)
                    };
                case TidNum.SEVEN:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\5201_セブン", false)
                    };
                case TidNum.COPO:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\7336_コポ", false)
                    };
                case TidNum.MARUJI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\7837_丸治", false)
                    };
                case TidNum.MANSYO:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\7903_万勝", false)
                    };
                case TidNum.MANEI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\7908_萬栄", false)
                    };
                case TidNum.NANKOKU:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\8208_ナンコクスーパー", false)
                    };
                case TidNum.YAMANAKA:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0127_ヤマナカ\【総額対応】ヤマナカ_V5_RT308R_振分発行", false)
                    };
                case TidNum.IZUMI:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0160_イズミ\イズミ【BMS値札メッセージ】", true)
                    };
                case TidNum.AKANOREN:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0165_あかのれん", false)
                    };
                case TidNum.KEIOSTORE:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\8115_京王ストア", false)
                    };
                case TidNum.OTHER01:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\その他\01_インナー商品シール", false)
                    };
                case TidNum.OTHER02:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\その他\02_和合JANタグ", false)
                    };
                case TidNum.OTHER03:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\その他\03_品番・サイズ・素材シール", false)
                    };
                case TidNum.OTHER04:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\その他\04_品質シール", false)
                    };
                case TidNum.OTHER05:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\その他\05_品番・カラーシール", false)
                    };
                case TidNum.OTHER06:
                    return new List<DirItem>()
                    {
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\その他\06_商材ラベル", false)
                    };
            }
            return null;
        }
    }

    public class TenmayaHenkan
    {
        public string 変換値 { get; set; }
        public string 変換A { get; set; }

        public TenmayaHenkan(string 変換値, string 変換A)
        {
            this.変換値 = 変換値;
            this.変換A = 変換A;
        }
    }
    public class TenmayaHenkanList
    {
        public List<TenmayaHenkan> sList;
        public List<TenmayaHenkan> cList;

        public TenmayaHenkanList()
        {
            Create();
        }

        private void Create()
        {
            sList = new List<TenmayaHenkan>()
            {
                new TenmayaHenkan("0001", "48*48"),
                new TenmayaHenkan("0002", "ΦΦΦ"),
                new TenmayaHenkan("0003", "16cm"),
                new TenmayaHenkan("0004", "ﾑｼﾃｲ"),
                new TenmayaHenkan("0010", "176X261"),
                new TenmayaHenkan("0011", "261X261"),
                new TenmayaHenkan("0012", "261X352"),
                new TenmayaHenkan("0013", "352X352"),
                new TenmayaHenkan("0014", "352X440"),
                new TenmayaHenkan("0015", "191X286"),
                new TenmayaHenkan("0016", "286X286"),
                new TenmayaHenkan("0017", "286X382"),
                new TenmayaHenkan("0018", "382X382"),
                new TenmayaHenkan("0020", "130A"),
                new TenmayaHenkan("0021", "140A"),
                new TenmayaHenkan("0022", "150A"),
                new TenmayaHenkan("0023", "160A"),
                new TenmayaHenkan("0024", "170A"),
                new TenmayaHenkan("0058", "58cm"),
                new TenmayaHenkan("0059", "60cm"),
                new TenmayaHenkan("0060", "63cm"),
                new TenmayaHenkan("0061", "65cm"),
                new TenmayaHenkan("0062", "68cm"),
                new TenmayaHenkan("0063", "70cm"),
                new TenmayaHenkan("0064", "72cm"),
                new TenmayaHenkan("0065", "74cm"),
                new TenmayaHenkan("0066", "76cm"),
                new TenmayaHenkan("0067", "78cm"),
                new TenmayaHenkan("0068", "80cm"),
                new TenmayaHenkan("0069", "82cm"),
                new TenmayaHenkan("0070", "84cm"),
                new TenmayaHenkan("0071", "86cm"),
                new TenmayaHenkan("0072", "88cm"),
                new TenmayaHenkan("0073", "90cm"),
                new TenmayaHenkan("0074", "91cm"),
                new TenmayaHenkan("0080", "100cm"),
                new TenmayaHenkan("0081", "110cm"),
                new TenmayaHenkan("0082", "120cm"),
                new TenmayaHenkan("0083", "130cm"),
                new TenmayaHenkan("0084", "140cm"),
                new TenmayaHenkan("0085", "150cm"),
                new TenmayaHenkan("0086", "160cm"),
                new TenmayaHenkan("0087", "170cm"),
                new TenmayaHenkan("0088", "180cm"),
                new TenmayaHenkan("0089", "190cm"),
                new TenmayaHenkan("0508", "7号"),
                new TenmayaHenkan("0509", "9号"),
                new TenmayaHenkan("0511", "11号"),
                new TenmayaHenkan("0513", "13号"),
                new TenmayaHenkan("0515", "15号"),
                new TenmayaHenkan("0517", "17号"),
                new TenmayaHenkan("0519", "19号"),
                new TenmayaHenkan("0521", "21号"),
                new TenmayaHenkan("0523", "23号"),
                new TenmayaHenkan("9901", "3S"),
                new TenmayaHenkan("9902", "SS"),
                new TenmayaHenkan("9903", "S"),
                new TenmayaHenkan("9904", "M"),
                new TenmayaHenkan("9905", "L"),
                new TenmayaHenkan("9906", "LL"),
                new TenmayaHenkan("9907", "3L"),
                new TenmayaHenkan("9908", "4L"),
                new TenmayaHenkan("9909", "O"),
                new TenmayaHenkan("9910", "OM"),
                new TenmayaHenkan("9911", "OL"),
                new TenmayaHenkan("9912", "EL"),
                new TenmayaHenkan("9913", "ﾌﾘｰｻｲｽﾞ"),
                new TenmayaHenkan("9914", "70cm"),
                new TenmayaHenkan("9915", "80cm"),
                new TenmayaHenkan("9916", "85cm"),
                new TenmayaHenkan("9917", "90cm"),
                new TenmayaHenkan("9918", "95cm"),
                new TenmayaHenkan("9930", "27ｲﾝﾁ"),
                new TenmayaHenkan("9931", "28ｲﾝﾁ"),
                new TenmayaHenkan("9932", "29ｲﾝﾁ"),
                new TenmayaHenkan("9933", "30ｲﾝﾁ"),
                new TenmayaHenkan("9934", "31ｲﾝﾁ"),
                new TenmayaHenkan("9935", "32ｲﾝﾁ"),
                new TenmayaHenkan("9936", "33ｲﾝﾁ"),
                new TenmayaHenkan("9937", "34ｲﾝﾁ"),
                new TenmayaHenkan("5000", "16-18cm"),
                new TenmayaHenkan("5001", "19-21cm"),
                new TenmayaHenkan("5002", "20-22cm"),
                new TenmayaHenkan("5003", "22-24cm"),
                new TenmayaHenkan("5004", "24-26cm"),

            };
            cList = new List<TenmayaHenkan>()
            {
                new TenmayaHenkan("01", "ｸﾛ"),
                new TenmayaHenkan("02", "ｼﾛ"),
                new TenmayaHenkan("10", "ﾀﾞｰｸｸﾞﾚｰ"),
                new TenmayaHenkan("11", "ﾐﾄﾞﾙｸﾞﾚｰ"),
                new TenmayaHenkan("12", "ﾗｲﾄｸﾞﾚｰ"),
                new TenmayaHenkan("19", "ｸﾞﾚｰｹｲ"),
                new TenmayaHenkan("20", "ﾊﾟｰﾌﾟﾙ"),
                new TenmayaHenkan("21", "ｳﾞｧｨｵﾚｯﾄ"),
                new TenmayaHenkan("22", "ﾗｲﾗｯｸ"),
                new TenmayaHenkan("29", "ﾊﾟｰﾌﾟﾙｹｲ"),
                new TenmayaHenkan("30", "ｱｶ"),
                new TenmayaHenkan("31", "ﾛｰｽﾞﾋﾟﾝｸ"),
                new TenmayaHenkan("32", "ﾋﾟﾝｸ"),
                new TenmayaHenkan("39", "ﾚｯﾄﾞｹｲ"),
                new TenmayaHenkan("40", "ｴﾝｼﾞ"),
                new TenmayaHenkan("41", "ｵﾚﾝｼﾞ"),
                new TenmayaHenkan("42", "ｻｰﾓﾝｵﾚﾝｼﾞ"),
                new TenmayaHenkan("49", "ｵﾚﾝｼﾞｹｲ"),
                new TenmayaHenkan("50", "ｸﾘｰﾑｲｴﾛｰ"),
                new TenmayaHenkan("51", "ﾚﾓﾝｲｴﾛｰ"),
                new TenmayaHenkan("52", "ｱｲﾎﾞﾘｰ"),
                new TenmayaHenkan("59", "ｲｴﾛｰｹｲ"),
                new TenmayaHenkan("60", "ﾓｽｸﾞﾘｰﾝ"),
                new TenmayaHenkan("61", "ｸﾞﾘｰﾝ"),
                new TenmayaHenkan("62", "ﾗｲﾄｸﾞﾘｰﾝ"),
                new TenmayaHenkan("69", "ｸﾞﾘｰﾝｹｲ"),
                new TenmayaHenkan("70", "ｺﾝ"),
                new TenmayaHenkan("71", "ﾌﾞﾙｰ"),
                new TenmayaHenkan("72", "ｻｯｸｽ"),
                new TenmayaHenkan("79", "ﾌﾞﾙｰｹｲ"),
                new TenmayaHenkan("80", "ﾀﾞｰｸﾌﾞﾗｳﾝ"),
                new TenmayaHenkan("81", "ｷｬﾒﾙ"),
                new TenmayaHenkan("82", "ﾍﾞｰｼﾞｭ"),
                new TenmayaHenkan("89", "ﾌﾞﾗｳﾝｹｲ"),
                new TenmayaHenkan("90", "ｺﾞｰﾙﾄﾞ"),
                new TenmayaHenkan("91", "ｼﾙﾊﾞｰ"),
                new TenmayaHenkan("92", "ｺﾊﾟｰ")
            };
        }

        public TenmayaHenkan GetSizeHenkanchiByName(string name, TenmayaHenkan defVal = null)
        {
            var result = defVal;
            var list = sList.Where(x => x.変換A.ToUpperInvariant().StartsWith(name.ToUpperInvariant())).OrderBy(x => x.変換値);
            if (list.Any())
            {
                if (list.Count() == 1)
                {
                    result = list.First();
                }
                else
                {
                    var ci = System.Globalization.CultureInfo.CurrentCulture.CompareInfo;
                    result = list.FirstOrDefault(item =>
                        ci.Compare(item.変換A.ToUpperInvariant(), name.ToUpperInvariant(), System.Globalization.CompareOptions.IgnoreWidth) == 0);
                }
            }
            return result;
        }

        public TenmayaHenkan GetColorHenkanchiByName(string name, TenmayaHenkan defVal = null)
        {
            var result = defVal;
            var list = cList.Where(x => x.変換A.ToUpperInvariant().StartsWith(name.ToUpperInvariant())).OrderBy(x => x.変換値);
            if (list.Any())
            {
                if (list.Count() == 1)
                {
                    result = list.First();
                }
                else
                {
                    var ci = System.Globalization.CultureInfo.CurrentCulture.CompareInfo;
                    result = list.FirstOrDefault(item =>
                        ci.Compare(item.変換A.ToUpperInvariant(), name.ToUpperInvariant(), System.Globalization.CompareOptions.IgnoreWidth) == 0);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// フジヤ週数確認
    /// </summary>
    public class FujiyaSyuuSu
    {
        public string 週数 { get; set; }
        public int 開始月 { get; set; }
        public int 開始日 { get; set; }
        public int 終了月 { get; set; }
        public int 終了日 { get; set; }

        public FujiyaSyuuSu(string 週数, int 開始月, int 開始日, int 終了月, int 終了日)
        {
            this.週数 = 週数;
            this.開始月 = 開始月;
            this.開始日 = 開始日;
            this.終了月 = 終了月;
            this.終了日 = 終了日;
        }
    }
    public class FujiyaSyuuSuList
    {
        public List<FujiyaSyuuSu> syusuList;

        public FujiyaSyuuSuList()
        {
            Create();
        }

        public void Create()
        {
            syusuList = new List<FujiyaSyuuSu>()
            {
                new FujiyaSyuuSu("1",   1,  1,  1,  7),    //  ～ 1/7
                new FujiyaSyuuSu("2",   1,  8,  1, 14),    //  ～ 1/14
                new FujiyaSyuuSu("3",   1, 15,  1, 21),    //  ～ 1/21
                new FujiyaSyuuSu("4",   1, 22,  1, 28),    //  ～ 1/28
                new FujiyaSyuuSu("5",   1, 29,  2,  4),    //  ～ 2/4
                new FujiyaSyuuSu("6",   2,  5,  2, 11),    //  ～ 2/11
                new FujiyaSyuuSu("7",   2, 12,  2, 18),    //  ～ 2/18
                new FujiyaSyuuSu("8",   2, 19,  2, 25),    //  ～ 2/25
                new FujiyaSyuuSu("9",   2, 26,  3,  4),    //  ～ 3/4
                new FujiyaSyuuSu("10",  3,  5,  3, 11),    //  ～ 3/11
                new FujiyaSyuuSu("11",  3, 12,  3, 18),    //  ～ 3/18
                new FujiyaSyuuSu("12",  3, 19,  3, 25),    //  ～ 3/25
                new FujiyaSyuuSu("13",  3, 26,  4,  1),    //  ～ 4/1
                new FujiyaSyuuSu("14",  4,  2,  4,  8),    //  ～ 4/8
                new FujiyaSyuuSu("15",  4,  9,  4, 15),    //  ～ 4/15
                new FujiyaSyuuSu("16",  4, 16,  4, 22),    //  ～ 4/22
                new FujiyaSyuuSu("17",  4, 23,  4, 29),    //  ～ 4/29
                new FujiyaSyuuSu("18",  4, 30,  5,  6),    //  ～ 5/6
                new FujiyaSyuuSu("19",  5,  7,  5, 13),    //  ～ 5/13
                new FujiyaSyuuSu("20",  5, 14,  5, 20),    //  ～ 5/20
                new FujiyaSyuuSu("21",  5, 21,  5, 27),    //  ～ 5/27
                new FujiyaSyuuSu("22",  5, 28,  6,  3),    //  ～ 6/3
                new FujiyaSyuuSu("23",  6,  4,  6, 10),    //  ～ 6/10
                new FujiyaSyuuSu("24",  6, 11,  6, 17),    //  ～ 6/17
                new FujiyaSyuuSu("25",  6, 18,  6, 24),    //  ～ 6/24
                new FujiyaSyuuSu("26",  6, 25,  7,  1),    //  ～ 7/1
                new FujiyaSyuuSu("27",  7,  2,  7,  8),    //  ～ 7/8
                new FujiyaSyuuSu("28",  7,  9,  7, 15),    //  ～ 7/15
                new FujiyaSyuuSu("29",  7, 16,  7, 22),    //  ～ 7/22
                new FujiyaSyuuSu("30",  7, 23,  7, 29),    //  ～ 7/29
                new FujiyaSyuuSu("31",  7, 30,  8,  5),    //  ～ 8/5
                new FujiyaSyuuSu("32",  8,  6,  8, 12),    //  ～ 8/12
                new FujiyaSyuuSu("33",  8, 13,  8, 19),    //  ～ 8/19
                new FujiyaSyuuSu("34",  8, 20,  8, 26),    //  ～ 8/26
                new FujiyaSyuuSu("35",  8, 27,  9,  2),    //  ～ 9/2
                new FujiyaSyuuSu("36",  9,  3,  9,  9),    //  ～ 9/9
                new FujiyaSyuuSu("37",  9, 10,  9, 16),    //  ～ 9/16
                new FujiyaSyuuSu("38",  9, 17,  9, 23),    //  ～ 9/23
                new FujiyaSyuuSu("39",  9, 24,  9, 30),    //  ～ 9/30
                new FujiyaSyuuSu("40", 10,  1, 10,  7),    //  ～ 10/7
                new FujiyaSyuuSu("41", 10,  8, 10, 14),    //  ～ 10/14
                new FujiyaSyuuSu("42", 10, 15, 10, 21),    //  ～ 10/21
                new FujiyaSyuuSu("43", 10, 22, 10, 28),    //  ～ 10/28
                new FujiyaSyuuSu("44", 10, 29, 11,  4),    //  ～ 11/4
                new FujiyaSyuuSu("45", 11,  5, 11, 11),    //  ～ 11/11
                new FujiyaSyuuSu("46", 11, 12, 11, 18),    //  ～ 11/18
                new FujiyaSyuuSu("47", 11, 19, 11, 25),    //  ～ 11/25
                new FujiyaSyuuSu("48", 11, 26, 12,  2),    //  ～ 12/2
                new FujiyaSyuuSu("49", 12,  3, 12,  9),    //  ～ 12/9
                new FujiyaSyuuSu("50", 12, 10, 12, 16),    //  ～ 12/16
                new FujiyaSyuuSu("51", 12, 17, 12, 23),    //  ～ 12/23
                new FujiyaSyuuSu("52", 12, 24, 12, 31)    //  ～ 12/31
            };
        }

        public FujiyaSyuuSu GetFujimaSyuuSuByDate(DateTime date, FujiyaSyuuSu defVal = null)
        {
            return syusuList.FirstOrDefault(x => new DateTime(date.Year, x.開始月, x.開始日) <= date && date <= new DateTime(date.Year, x.終了月, x.終了日)) ?? defVal;
        }
    }
}
