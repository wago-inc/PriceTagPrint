using System;
using System.Collections.Generic;
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
        public const int FUJIYA = 1420;
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
        public const int SANKAKU = 145;
        public const int SEVEN = 5201;
        public const int COPO = 7336;
        public const int MARUJI = 7837;
        public const int MANSYO = 7903;
        public const int MANEI = 7908;
        public const int NANKOKU = 8208;
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
        public const string FUJIYA = "1420";
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
        public const string SANKAKU = "0145";
        public const string SEVEN = "5201";
        public const string COPO = "7336";
        public const string MARUJI = "7837";
        public const string MANSYO = "7903";
        public const string MANEI = "7908";
        public const string NANKOKU = "8208";
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
        public const string FUJIYA = "フジヤ";
        public const string HOKKAIDO_SANKI = "北海道三喜";
        public const string HONTENTAKAHASI = "本店タカハシ";
        public const string MAXVALUE = "マックスバリュ西日本";
        public const string MARBURU = "マーブル";
        public const string MANEKI = "マネキ";
        public const string MIYAMA = "ミヤマ";
        public const string YANAGIYA = "ヤスサキ";
        public const string WORKWAY = "ワークウェイ";
        public const string SANKI = "三喜";
        public const string SUNNY_MART = "サニーマート";
        public const string SANKAKU = "三角商事";
        public const string SEVEN = "セブン";
        public const string COPO = "コポ";
        public const string MARUJI = "丸治";
        public const string MANSYO = "万勝";
        public const string MANEI = "萬栄";
        public const string NANKOKU = "ナンコクスーパー";
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
                new Torihikisaki(10, Tid.OKADA,          Tnm.OKADA,          HakkouKind.Input, CreateDirList(TidNum.OKADA)),
                new Torihikisaki(11, Tid.KANETA,         Tnm.KANETA,         HakkouKind.Input, CreateDirList(TidNum.KANETA)),
                new Torihikisaki(12, Tid.KYOEI,          Tnm.KYOEI,          HakkouKind.Both,  CreateDirList(TidNum.KYOEI)),
                new Torihikisaki(13, Tid.COSMOMATUOKA,   Tnm.COSMOMATUOKA,   HakkouKind.Input, CreateDirList(TidNum.COSMOMATUOKA)),
                new Torihikisaki(14, Tid.SANEI,          Tnm.SANEI,          HakkouKind.Input, CreateDirList(TidNum.SANEI)),
                new Torihikisaki(15, Tid.TAIFUKUTOKYO,   Tnm.TAIFUKUTOKYO,   HakkouKind.Input, CreateDirList(TidNum.TAIFUKUTOKYO)),
                new Torihikisaki(16, Tid.TENMAYA,        Tnm.TENMAYA,        HakkouKind.Input, CreateDirList(TidNum.TENMAYA)),
                new Torihikisaki(17, Tid.DOMMY,          Tnm.DOMMY,          HakkouKind.Input, CreateDirList(TidNum.DOMMY)),
                new Torihikisaki(18, Tid.BIGA,           Tnm.BIGA,           HakkouKind.Input, CreateDirList(TidNum.BIGA)),
                new Torihikisaki(19, Tid.FUJI,           Tnm.FUJI,           HakkouKind.Input, CreateDirList(TidNum.FUJI)),
                new Torihikisaki(20, Tid.FUJIYA,         Tnm.FUJIYA,         HakkouKind.Input, CreateDirList(TidNum.FUJIYA)),
                new Torihikisaki(21, Tid.HOKKAIDO_SANKI, Tnm.HOKKAIDO_SANKI, HakkouKind.Input, CreateDirList(TidNum.HOKKAIDO_SANKI)),
                new Torihikisaki(22, Tid.HONTENTAKAHASI, Tnm.HONTENTAKAHASI, HakkouKind.Input, CreateDirList(TidNum.HONTENTAKAHASI)),
                new Torihikisaki(23, Tid.MAXVALUE,       Tnm.MAXVALUE,       HakkouKind.Input, CreateDirList(TidNum.MAXVALUE)),
                new Torihikisaki(24, Tid.MARBURU,        Tnm.MARBURU,        HakkouKind.Input, CreateDirList(TidNum.MARBURU)),
                new Torihikisaki(25, Tid.MANEKI,         Tnm.MANEKI,         HakkouKind.Both,  CreateDirList(TidNum.MANEKI)),
                new Torihikisaki(26, Tid.MIYAMA,         Tnm.MIYAMA,         HakkouKind.Input, CreateDirList(TidNum.MIYAMA)),
                new Torihikisaki(27, Tid.YANAGIYA,       Tnm.YANAGIYA,       HakkouKind.Input, CreateDirList(TidNum.YANAGIYA)),
                new Torihikisaki(28, Tid.WORKWAY,        Tnm.WORKWAY,        HakkouKind.Input, CreateDirList(TidNum.WORKWAY)),
                new Torihikisaki(29, Tid.SUNNY_MART,     Tnm.SUNNY_MART,     HakkouKind.Input, CreateDirList(TidNum.SUNNY_MART)),
                new Torihikisaki(30, Tid.SANKAKU,        Tnm.SANKAKU,        HakkouKind.Input, CreateDirList(TidNum.SANKAKU)),
                new Torihikisaki(31, Tid.SEVEN,          Tnm.SEVEN,          HakkouKind.Input, CreateDirList(TidNum.SEVEN)),
                new Torihikisaki(32, Tid.COPO,           Tnm.COPO,           HakkouKind.Input, CreateDirList(TidNum.COPO)),
                new Torihikisaki(33, Tid.MARUJI,         Tnm.MARUJI,         HakkouKind.Input, CreateDirList(TidNum.MARUJI)),
                new Torihikisaki(34, Tid.MANSYO,         Tnm.MANSYO,         HakkouKind.Input, CreateDirList(TidNum.MANSYO)),
                new Torihikisaki(35, Tid.MANEI,          Tnm.MANEI,          HakkouKind.Input, CreateDirList(TidNum.MANEI)),
                new Torihikisaki(36, Tid.NANKOKU,        Tnm.NANKOKU,        HakkouKind.Input, CreateDirList(TidNum.NANKOKU)),
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
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\タイフク東京", false)
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
                        new DirItem(@"Y:\WAGOAPL\SATO\MLV5_Layout\0112_ヤスサキ\【総額対応】ヤスサキ_V5_RT308R_振分発行", true)
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
            }
            return null;
        }
    }
}
