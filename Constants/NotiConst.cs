public class NotiConst
{
    /// Controller层消息通知
    public const string SPLASH = "Splash";                         //闪屏
    public const string PRELOAD = "Preload";                       //预加载
    public const string STARTUP = "Startup";                       //启动框架

    /// View层消息通知
    public const string UPDATE_PROGRESS = "UpdateProgress";         //更新进度
    public const string UPDATE_FINISH = "UpdateFinish";             //更新完成

    /// 线程事件
    public const string EXTRACT_FILE = "ExtractFile";               //解压文件
    public const string EXTRACT_UPDATE = "ExtractUpdate";           //更新解压进度
    public const string EXTRACT_FINISH = "ExtractFinish";           //解压完成
    public const string EXTRACT_STREAM = "ExtractStream";           //提取StreamingAssets资源
}