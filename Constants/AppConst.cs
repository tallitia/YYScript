using System.Collections.Generic;
using UnityEngine;

public class AppConst
{
#if UNITY_STANDALONE
    public static string OS = "pc";
#elif UNITY_ANDROID
    public static string OS = "android";
#elif UNITY_IPHONE
    public static string OS = "ios";
#else
    public static string OS = "temp";
#endif

#if UNITY_EDITOR
    public const bool BundleMode = true;                                //AssetBundle模式
#else
    public const bool BundleMode = true;                                 //AssetBundle模式
#endif

    public const bool CheckVersion = true;                               // 是否需要检查版本

    public const int TimerInterval = 1;
    public const int GameFrameRate = 60;                                 //游戏帧频

    public const string AppName = "chuhan";                             //应用程序名称
    public const string AppPrefix = AppName + "_";                       //应用程序前缀

    public const string FontName = "fonts_text.ab";                      //字体的AB包名
    public const string VerionName = "version.txt";                      //版本管理文件名
    public const string XVersionName = "xversion.txt";                   //数据管理文件名
    public static string AppVersion = "";                                //游戏版本号，setting.xml读取覆盖
    public static string AppID = "";                                     //游戏ID，setting.xml读取覆盖
    public static string AppChannel = "";                                //渠道，setting.xml读取覆盖
    public static string ConfigURI = "";                                 //配置文件地址，setting.xml读取
    public static int AppBuild = 0;                                      //代码版本，setting.xml读取覆盖

    public static bool IsVerify = false;                                 //是否是提审状态, 配置文件读取
    public static bool IsLogger = true;                                  //是否打印日志, 配置文件读取
    public static bool IsDebug = false;                                  //是否是调试模式
    public static bool IsShowSplash = false;                             //是否显示闪屏
    public static bool IsMultiLanguage = false;                          //是否打开多语言
    public static int ConfigBuild = 0;

    public static bool isWin        = false;
    public static bool isAndroid    = false;
    public static bool isEditor     = false;
    public static bool isIOS        = false;
    public static bool isMobile     = false;

    public const string PatchAsset = "patch.ab";                         //补丁包名
    /// 本地缓存AB路径
    public static string CachePath
    {
        get { return Application.persistentDataPath + "/cache/" + AppVersion + "/"; }
    }

    /// 本地缓存data路径
    public static string DataPath
    {
        get { return Application.persistentDataPath + "/data/"; }
    }

    /// stream缓存的AB路径
    public static string AssetsPath
    {
        get {
            return Application.streamingAssetsPath + "/ab/";
        }
    }

    /// 本地配置路径
    public static string SettingPath
    {
        get { return Application.streamingAssetsPath + "/Setting.xml"; }
    }

    /// AB压缩包路径
    public static string ABZipPath
    {
        get { return Application.streamingAssetsPath + "/ab.zip"; }
    }

    /// 配置文件地址
    public static string ConfigPath
    {
        get { return ConfigURI + "/" + AppVersion + "/config.txt"; }
    }

    /// 资源服资源路径
    public static string AssetURI
    {
        get;set;
    }

    /// 资源服资源路径
    public static string AssetBundleURI
    {
        get { return AssetURI + OS + "/"; }
    }

    /// 配置表资源路径
    public static string DataURI
    {
        get { return AssetURI + "data/"; }
    }

    public static string LoginURL
    {
        set; get;
    }

    public static string ServerFile
    {
        set; get;
    }
    public static string OrderURL
    {
        set; get;
    }

    /// 判断系统是否是32位
    public static bool Is32Bit
    {
        get { return System.IntPtr.Size != 8; }
    }
}