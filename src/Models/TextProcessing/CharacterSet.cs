namespace Reoreo125.Memopad.Models.TextProcessing;

public class Characterset
{
    public record CharacterSet(string Name, string SampleText, GdiCharSet CharSet, char CheckChar);

    public static readonly List<CharacterSet> AllCharacterSets = new()
        {
            new("欧文", "AaBbYyZz", GdiCharSet.Ansi, 'A'),
            new("日本語", "あいうえお", GdiCharSet.ShiftJis, 'あ'),
            new("ハングル", "가나다라", GdiCharSet.Hangeul, '가'),
            new("ギリシャ語", "ΑαΒβΓγ", GdiCharSet.Greek, 'Ω'),
            new("トルコ語", "AaBbĞğŞş", GdiCharSet.Turkish, 'Ğ'),
            new("バルト諸国言語", "AaBbYyZzĄąĘęĖė", GdiCharSet.Baltic, 'Ą'),
            new("中央ヨーロッパ言語", "AaBbYyZzÁáČčĎď", GdiCharSet.EastEurope, 'č'),
            new("キリル言語", "AaBbYyZzБбВвГг", GdiCharSet.Russian, 'Б'),
            new("アラビア言語", "ابتثجحخدذرز", GdiCharSet.Arabic, 'ش'),
            new("ヘブライ言語", "אבגדהוזחטיך", GdiCharSet.Hebrew, 'א'),
            new("ベトナム言語", "AaBbYyZzÂâÊêÔô", GdiCharSet.Vietnamese, 'Ꭴ'),
            new("中国語 (GB2312)", "中文样板", GdiCharSet.Gb2312, '样'),
            new("中国語 (Big5)", "中文樣板", GdiCharSet.Big5, '樣'),
            new("シンボル", "AaBbYyZz", GdiCharSet.Symbol, 'α'), // Symbol用
            new("OEM/DOS", "AaBbYyZz", GdiCharSet.Oem, 'A'),
        };
}

public enum GdiCharSet : byte
{
    Ansi = 0,             // 欧文
    Default = 1,
    Symbol = 2,           // シンボル
    Mac = 77,
    ShiftJis = 128,       // 日本語
    Hangeul = 129,
    Johab = 130,
    Gb2312 = 134,         // 中国語 (簡体字)
    Big5 = 136,           // 中国語 (繁体字)
    Greek = 161,          // ギリシャ語
    Turkish = 162,        // トルコ語
    Vietnamese = 163,     // ベトナム語
    Hebrew = 177,         // ヘブライ語
    Arabic = 178,         // アラビア語
    Baltic = 186,         // バルト諸国言語
    Russian = 204,        // キリル言語 (ロシア語)
    Thai = 222,           // タイ語
    EastEurope = 238,     // 中央ヨーロッパ言語
    Oem = 255             // OEM/DOS
}
