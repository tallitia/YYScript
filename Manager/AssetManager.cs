using UnityEngine;
using System.Collections.Generic;

/// 在移动平台下AssetBundle加载只会加载头部文件
/// 加载完不Unload并不会引起性能问题 
/// 故暂时屏蔽Unload
public class AssetManager : Manager
{
    private class AssetData
    {
        public AssetBundle assetbundle;
        public int reference = 0;
    }

    private AssetBundleManifest mManifest;
    private Dictionary<string, int> mAssetConfig;
    private Dictionary<string, string> mAssetIndex;
    private Dictionary<string, AssetData> mAssetCache;

    /// 需要去Cache Path读取的资源
    private Dictionary<string, bool> mAssetMap = new Dictionary<string, bool>();

    void Awake()
    {
        LogUtil.StartLog("AssetManager Awake");
        mAssetCache = new Dictionary<string, AssetData>();
        mAssetIndex = new Dictionary<string, string>();
    }
   
    public override void OnInit()
    {
        if (AppConst.BundleMode)
        {
            mManifest = LoadAssetManifest(GetAssetPath("index.ab"));
            mAssetConfig = LoadAssetConfig(GetAssetPath("assetconfig.ab"));
        }
    }

    public void Destory()
    {
        mManifest = null;

        if (mAssetCache != null && mAssetCache.Count > 0)
        {
            foreach (var pair in mAssetCache)
            {
                pair.Value.assetbundle.Unload(false);
            }
            mAssetCache.Clear();
            mAssetCache = null;
        }
    }

    public override void OnRestart()
    {
        base.OnRestart();
        UnloadAllAssetBundle(true);
    }

    private string GetAssetPath(string name)
    {
        if (name == AppConst.FontName && AppConst.IsMultiLanguage)
        {
            // 字体的AB名称，需要根据当前语言去加载对应的字体AB
            var filename = System.IO.Path.GetFileNameWithoutExtension(name);
            var ext = System.IO.Path.GetExtension(name);
            var lang = MainGame.GetManager<LangManager>();
            name = filename + "_" + lang.Current() + ext;
        }

        if (mAssetMap.ContainsKey(name))
            return AppConst.CachePath + name;
        return AppConst.AssetsPath + name;
    }



    private AssetBundleManifest LoadAssetManifest(string path)
    {
        AssetBundle ab = AssetBundle.LoadFromFile(path);
        AssetBundleManifest manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        ab.Unload(false);
        return manifest;
    }

    private Dictionary<string, int> LoadAssetConfig(string path)
    {
        AssetBundle ab = AssetBundle.LoadFromFile(path);
        TextAsset textasset = ab.LoadAsset<TextAsset>("AssetConfig");
        string[] lines = textasset.text.Split('\n');
        ab.Unload(true);

        Dictionary<string, int> dict = new Dictionary<string, int>();
        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split('|');
            dict.Add(line[0], int.Parse(line[1]));
        }
        return dict;
    }

    protected AssetBundle LoadRawAssetBundle(string abname)
    {
        string path = GetAssetPath(abname);
        AssetBundle ab = AssetBundle.LoadFromFile(path);
        if (ab != null)
        {
            Util.Log("Load AssetBundle: " + path);
            AssetData asset = new AssetData();
            asset.assetbundle = ab;
            asset.reference++;
            mAssetCache.Add(abname, asset);
            return ab;
        }

        Util.Log("Load AssetBundle failed: " + path);
        return null;
    }

    protected void UnloadRawAssetBundle(string abname)
    {
        Util.Log("Unload AssetBundle: " + abname);
        AssetData asset = mAssetCache[abname];
        asset.assetbundle.Unload(false);
        mAssetCache.Remove(abname);
    }

    protected void LoadDependencies(string abname)
    {
        string[] dependencies = mManifest.GetAllDependencies(abname);
        if (dependencies.Length == 0) return;

        for (int i = 0; i < dependencies.Length; i++)
        {
            AssetData asset = null;
            if (mAssetCache.TryGetValue(dependencies[i], out asset))
                asset.reference++;
            else
                LoadRawAssetBundle(dependencies[i]);
        }
    }

    protected void UnloadDependencies(string abname)
    {
        string[] dependencies = mManifest.GetAllDependencies(abname);
        if (dependencies.Length == 0) return;

        for (int i = 0; i < dependencies.Length; i++)
        {
            AssetData asset = null;
            if (mAssetCache.TryGetValue(dependencies[i], out asset) && --asset.reference == 0)
                UnloadRawAssetBundle(dependencies[i]);
        }
    }

    public AssetBundle LoadAssetBundle(string abname)
    {
        AssetData asset = null;
        if (mAssetCache.TryGetValue(abname, out asset))
        {
            asset.reference++;
            return asset.assetbundle;
        }
        else
        {
            LoadDependencies(abname);
            return LoadRawAssetBundle(abname);
        }
    }

    public void UnloadAssetBundle(string abname)
    {
        AssetData asset = null;
        if (mAssetCache.TryGetValue(abname, out asset) && --asset.reference == 0)
        {
            UnloadDependencies(abname);
            UnloadRawAssetBundle(abname);
        }
    }

    public void UnloadAllAssetBundle(bool dispose)
    {
        if (mAssetCache != null && mAssetCache.Count > 0)
        {
            foreach (var pair in mAssetCache)
            {
                pair.Value.assetbundle.Unload(dispose);
            }
            mAssetCache.Clear();
        }
    }

    /// 获取AssetBundle，不存在就去加载
    public AssetBundle GetAssetBundle(string abname)
    {
        AssetData asset = null;
        if (mAssetCache.TryGetValue(abname, out asset))
            return asset.assetbundle;

        return LoadAssetBundle(abname);
    }


    public string GetAssetBundleName(string path)
    {
        string abname = string.Empty;
        if (mAssetIndex.TryGetValue(path.ToLower(), out abname))
            return abname;

        string[] folders = path.Split('/');
        int config = 0;
        string key;

        if (folders.Length > 2)
        {
            key = folders[0] + "/" + folders[1];
            if (mAssetConfig.TryGetValue(key, out config))
            {
                if (config == 1)
                    abname = key.Replace("/", "_");
                else if (config == 2)
                    abname = path.Split('.')[0].Replace("/", "_");
                else if (config == 3 && folders.Length == 3)
                    abname = key.Replace("/", "_");
                else if (config == 3)
                    abname = key.Replace("/", "_") + "_" + folders[2];
                else if (config == 4)
                    abname = key.Replace("/", "_");

                abname = abname.ToLower() + ".ab";
                mAssetIndex.Add(path.ToLower(), abname);
                return abname;
            }
        }

        key = folders[0];
        if (mAssetConfig.TryGetValue(key, out config))
        {
            //路径 | 打包方式(1:文件夹打包成一个AB, 2:文件打成一个AB, 3:文件夹里的文件夹打成一个AB)
            if (config == 1)
                abname = key;
            else if (config == 2)
                abname = path.Split('.')[0].Replace("/", "_");
            else if (config == 3)
                abname = key + "_" + folders[1];
            else if (config == 4)
                abname = key;

            abname = abname.ToLower() + ".ab";
            mAssetIndex.Add(path.ToLower(), abname);
            return abname;
        }

        Util.Log("Failed to get AssetBundle name: " + path);
        return null;
    }

    public Object LoadAsset(string path)
    {
        string extension = System.IO.Path.GetExtension(path);
        if (extension == ".prefab")
            return LoadAsset<GameObject>(path);

        return LoadAsset<Object>(path);
    }

    public Sprite LoadSprite(string path)
    {
        return LoadAsset<Sprite>(path);
    }

    public Sprite[] LoadSprites(string path)
    {
        var all = LoadAll(path);
        var list = new List<Sprite>();
        for (int i=0; i<all.Length; i++)
        {
            if (all[i] is Sprite) list.Add(all[i] as Sprite);
        }
        return list.ToArray();
    }

    public Texture LoadTexture(string path)
    {
        return LoadAsset<Texture>(path);
    }

    public TextAsset LoadText(string path)
    {
        return LoadAsset<TextAsset>(path);
    }

    public Material LoadMaterial(string path)
    {
        return LoadAsset<Material>(path);
    }
    public AudioClip LoadAudioClip(string path)
    {
        return LoadAsset<AudioClip>(path);
    }

    public Object[] LoadAll(string path)
    {
        if (AppConst.BundleMode)
        {
            string abname = GetAssetBundleName(path);
            if (abname == null)
                return null;

            AssetBundle ab = GetAssetBundle(abname);
            if (ab != null)
            {
                string assetname = System.IO.Path.GetFileNameWithoutExtension(path);
                Object[] assets = ab.LoadAssetWithSubAssets(assetname);
                if (assets == null || assets.Length == 0)
                    Util.Log("Load Asset '" + path + "' Failed!!");
                return assets;
            }
            Util.Log("Load '" + path + "' failed! Please load the AssetBundle '" + abname + "' first.");
            return null;
        }
        else
        {
#if UNITY_EDITOR
            path = "Assets/Game/Assets/" + path;
            Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            if (assets == null || assets.Length == 0)
                Util.Log("Load Asset '" + path + "' Failed!!");
            return assets;
#else
            return Resources.LoadAll(path);
#endif
        }
    }

    public T LoadAsset<T>(string path) where T : Object
    {
        if (AppConst.BundleMode)
        {
            string abname = GetAssetBundleName(path);
            if (abname == null)
                return null;

            AssetBundle ab = GetAssetBundle(abname);
            if (ab != null)
            {
                string assetname = System.IO.Path.GetFileNameWithoutExtension(path);
                T asset = ab.LoadAsset<T>(assetname);
                if (asset == null) Util.Log("Load Asset '" + path + "' Failed!!");
                return asset;
            }
            Util.Log("Load '" + path + "' failed! Please load the AssetBundle '" + abname + "' first.");
            return null;
        }
        else
        {
#if UNITY_EDITOR
            path = "Assets/Game/Assets/" + path;
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            //if (asset == null) Util.Log("Load Asset '" + path + "' Failed!!");
            return asset;
#else
            return Resources.Load<T>(path);
#endif
        }
    }

    public T LoadAsset<T>(string abname, string name) where T : Object
    {
        AssetBundle ab = GetAssetBundle(abname);
        if (ab != null)
        {
            T asset = ab.LoadAsset<T>(name);
            if (asset == null) Util.Log("Load Asset '" + name + "' in '" + abname + "' Failed!!");
            return asset;
        }
        Util.Log("Load '" + name + "' failed! AssetBundle: " + abname);
        return null;
    }

    //不用缓存加载asset, 加载完马上assetbundle.unload(false)
    public Object LoadUncachedAsset(string path)
    {
        string abname = GetAssetBundleName(path);
        if (abname == null)
            return null;

        string assetname = System.IO.Path.GetFileNameWithoutExtension(path);
        AssetBundle ab = LoadAssetBundle(abname);
        if (ab != null)
        {
            Util.Log("Load '" + assetname + "' in the AssetBundle '" + abname + "'.");
            Object asset = ab.LoadAsset(assetname);
            UnloadAssetBundle(abname);
            return asset;
        }
        return null;
    }

    //添加资源标记 
    public void AddAssetFlag(string name)
    {
        if (!mAssetMap.ContainsKey(name))
            mAssetMap.Add(name, true);
    }

    public void UnloadAsset(Object asset)
    {
        Resources.UnloadAsset(asset);
    }

    public void UnloadUnusedAssets()
    {
        Resources.UnloadUnusedAssets();
    }
}
