using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;


public class MainGame : MonoBehaviour
{
    private static MainGame s_Instance;
    public static AppView appView;

    private Coroutine restartCo;

    void Awake()
    {
        if (GameUtil.mainGame != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        if (s_Instance == null)
            s_Instance = this;
    }

    void Start()
    {
        if (GameUtil.mainGame != null)
            return;

        GameUtil.mainGame = s_Instance;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = AppConst.GameFrameRate;

        SDKAdapter.GetInstance().SetTDEvent("launch_game");

        DOTween.Init(true, true).SetCapacity(200, 50);

        AddManager();
        StartApp();
        
    }

    private void AddManager()
    {
        gameObject.AddComponent<AssetManager>();
        gameObject.AddComponent<AudioManager>();
        gameObject.AddComponent<LangManager>();
        gameObject.AddComponent<LayerManager>();
        gameObject.AddComponent<NetworkManager>();
        gameObject.AddComponent<PoolManager>();
        gameObject.AddComponent<ThreadManager>();
        gameObject.AddComponent<TimerManager>();
        gameObject.AddComponent<LuaManager>();
    }

    public static T GetManager<T>() where T : IManager
    {
        return s_Instance.gameObject.GetComponent<T>();
    }
    
    public static void InitManagers()
    {
        var all =  s_Instance.gameObject.GetComponents<IManager>();
        foreach (var item in all)
            item.OnInit();
    }

    //App开始
    public void StartApp()
    {
        LogUtil.StartLog("StartApp");
        appView = new AppView();
        appView.Run();
    }

    //进入游戏
    public static void EnterGame()
    {
        LogUtil.StartLog("EnterGame");
        LuaManager lua = GetManager<LuaManager>();
        lua.StartLua();
    }

    //重启游戏
    public static void RestartGame()
    {
        Util.Log("ExitGame");
        SceneManager.sceneLoaded += s_Instance.OnSceneLoaded;
        SceneManager.LoadScene(0);
    }

    private void OnSceneLoaded(Scene scence, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopCo(restartCo);
        restartCo = StartCo(Restart());
    }

    // 重启游戏
    private IEnumerator Restart()
    {
        var all = gameObject.GetComponents<IManager>();
        foreach (var item in all)
        {
            item.OnRestart();
        }
        EnterGame();
        yield return null;
        //清理内存
        GC.Collect();
        yield return null;
    }

    public static Coroutine StartCo(IEnumerator pRoutine)
    {
        if (s_Instance != null)
            return s_Instance.StartCoroutine(pRoutine);

        return null;
    }

    public static void StopCo(Coroutine pRoutine)
    {
        if (s_Instance != null && pRoutine != null)
            s_Instance.StopCoroutine(pRoutine);

    }

}
