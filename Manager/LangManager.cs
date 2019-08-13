using UnityEngine;
using System.Collections.Generic;

public class LangManager : Manager
{
    public const string ZH_CN = "zh";
    public const string ZH_HK = "hk";
    public const string ENG = "en";

    /// 默认语言
    private const string DEFAULT = ZH_CN;

    /// 设置缓存KEY
    private const string SAVE_KEY = "LANGUAGE";

    /// 语言对应字体
    private Dictionary<string, string> mFonts;

    /// 语言对应货币
    private Dictionary<string, string> mCurrency;

    /// 当前语言
    private string mValue;

    void Awake()
    {
        LogUtil.StartLog("LangManager Awake");
        mFonts = new Dictionary<string, string>();
        mFonts.Add(ZH_CN, "sh-zh.ttf");
        mFonts.Add(ZH_HK, "sh-hk.ttf");
        mFonts.Add(ENG, "sh-en.ttf");

        mCurrency = new Dictionary<string, string>();
        mCurrency.Add(ZH_CN, "￥");
        mCurrency.Add(ZH_HK, "$");
        mCurrency.Add(ENG, "$");
    }

    public override void OnInit()
    {
        base.OnInit();
        mValue = PlayerPrefs.GetString(SAVE_KEY, DEFAULT);
    }

    public void Change(string val)
    {
        mValue = val;
        PlayerPrefs.SetString(SAVE_KEY, val);
#if UNITY_EDITOR
        if (mFonts.ContainsKey(val))
        {
            var source = "Assets/Game/Assets/Fonts/Backup/" + mFonts[val];
            var dest = "Assets/Game/Assets/Fonts/Text/sh.ttf";
            System.IO.File.Copy(source, dest, true);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }

    /// 当前语言
    public string Current()
    {
        if (!AppConst.IsMultiLanguage)
            return DEFAULT;
        return mValue;
    }

    /// 当前货币
    public string Currency()
    {
        var cur = Current();
        return mCurrency[cur];
    }

    /// 是否打开语言切换功能
    public bool Enabled()
    {
        return AppConst.IsMultiLanguage;
    }
}