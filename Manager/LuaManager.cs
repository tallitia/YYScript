using LuaInterface;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class LuaManager : LuaClient, IManager
{
    private LuaLoader luaLoader;
    private LuaFunction appPaused = null;
    private LuaFunction appResume = null;

    public void OnInit()
    {

    }

    public void StartLua()
    {
        if (!isInit)
            Init();
        luaLoader.InitBundles();
        luaState.Start();
        StartMain();
        StartLooper();
    }

    private void OnApplicationPause(bool isPause)
    { 
        if(isPause && appPaused != null) 
            appPaused.Call();
        else if (appResume != null) 
            appResume.Call();
    }

    protected override LuaFileUtils InitLoader()
    {
        luaLoader = new LuaLoader();
        return luaLoader;
    }

    protected override void LoadLuaFiles()
    {
    }

    protected override void OpenLibs()
    {
        luaState.OpenLibs(LuaDLL.luaopen_sproto_core);
        base.OpenLibs();
        OpenCJson();
    }

    protected override void StartMain()
    {
        luaState.DoFile("main.lua");

        LuaFunction main = luaState.GetFunction("main");
        main.Call();
        main.Dispose();
        main = null;     

        appPaused = luaState.GetFunction("pause");
        appResume = luaState.GetFunction("resume");
    }

    //获得lua函数
    public LuaFunction GetFunction(string name)
    {
        if (luaState != null)
            return luaState.GetFunction(name);
        
        return null;
    }

    public void OnRestart()
    {
        Destroy();
    }
}