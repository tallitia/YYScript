using UnityEngine;
using System.Collections;
using System.IO;
using LuaInterface;

public class LuaLoader : LuaFileUtils
{
    private const string EXT = ".lua.bytes";

    private string mPrefix = "";
    private AssetManager mAsset;
    public LuaLoader()
    {
        instance = this;
        beZip = AppConst.BundleMode;
        mPrefix = AppConst.Is32Bit ? "Lua32" : "Lua64";
        mAsset = MainGame.GetManager<AssetManager>();
    }

    public void InitBundles()
    {
        if (!beZip) return;

        AssetBundle ab = mAsset.LoadAssetBundle("luaconfig.ab");
        TextAsset config = ab.LoadAsset<TextAsset>("LuaConfig");
        string[] lines = config.text.Split('\n');
        mAsset.UnloadAssetBundle("luaconfig.ab");

        for (int i = 0; i < lines.Length; i++)
        {
            if (!lines[i].StartsWith(mPrefix.ToLower()))
                continue;

            ab = mAsset.LoadAssetBundle(lines[i]);
            if (ab != null) AddSearchBundle(lines[i], ab);
        }
        //Load Database Lua
        
   
    }

    public override byte[] ReadFile(string fileName)
    {
        if (beZip)
        {
            string path = mPrefix + "/" + fileName;
            if (fileName.StartsWith("database"))
            {
                //fileName = path.Replace("/", "_") + EXT;
                //return File.ReadAllBytes(AppConst.AssetsPath + fileName.ToLower());
            }
            string abname = mAsset.GetAssetBundleName(path);
            AssetBundle ab = null;
            if (!zipMap.TryGetValue(abname.ToLower(), out ab))
                return null;

			fileName = Path.GetFileNameWithoutExtension(fileName);
			fileName += EXT;
            TextAsset asset = ab.LoadAsset<TextAsset>(fileName);

            byte[] bytes = asset.bytes;
			Resources.UnloadAsset(asset);
            return bytes;
        }
        else
        {
            string path = FindFile(fileName);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                return File.ReadAllBytes(path);
            return null;
        }
    }

}