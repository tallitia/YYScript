using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;

public class Util
{
    public static int Int(object o)
    {
        return Convert.ToInt32(o);
    }

    public static float Float(object o)
    {
        return (float)Math.Round(Convert.ToSingle(o), 2);
    }

    public static long Long(object o)
    {
        return Convert.ToInt64(o);
    }

    public static int RandomInt(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float Random(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static string StrInsert(int index,string value,string insert)
    {
        return value.Insert(index, insert);
    }
    
    public static long GetTime()
    {
        TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
        return (long)ts.TotalMilliseconds;
    }

    public static double GetTimestamp()
    {
        return (double)GetTime();
    }

    /// 屏幕坐标转本地坐标
    public static Vector2 ScreenPointToLocalPoint(RectTransform rt, Vector2 pos)
    {
        Vector2 result = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, pos, Camera.main, out result);
        return result;
    }

    /// 搜索子物体组件-GameObject版
    public static T Get<T>(GameObject go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.transform.Find(subnode);
            if (sub != null) return sub.GetComponent<T>();
        }
        return null;
    }

    /// 搜索子物体组件-Transform版
    public static T Get<T>(Transform go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.Find(subnode);
            if (sub != null) return sub.GetComponent<T>();
        }
        return null;
    }

    /// 搜索子物体组件-Component版
    public static T Get<T>(Component go, string subnode) where T : Component
    {
        return go.transform.Find(subnode).GetComponent<T>();
    }

    /// 添加组件
    public static T Add<T>(GameObject go) where T : Component
    {
        if (go != null)
        {
            T[] ts = go.GetComponents<T>();
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i] != null) GameObject.Destroy(ts[i]);
            }
            return go.gameObject.AddComponent<T>();
        }
        return null;
    }

    /// 添加组件
    public static T Add<T>(Transform go) where T : Component
    {
        return Add<T>(go.gameObject);
    }

    /// 查找子对象
    public static Transform Find(Transform trans, string subnode)
    {
        return trans.Find(subnode);
    }

    /// 查找子对象
    public static GameObject Child(GameObject go, string subnode)
    {
        return Child(go.transform, subnode);
    }

    /// 查找子对象
    public static GameObject Child(Transform go, string subnode)
    {
        Transform tran = go.Find(subnode);
        if (tran == null) return null;
        return tran.gameObject;
    }

    /// 取平级对象
    public static GameObject Peer(GameObject go, string subnode)
    {
        return Peer(go.transform, subnode);
    }

    /// 取平级对象
    public static GameObject Peer(Transform go, string subnode)
    {
        Transform tran = go.parent.Find(subnode);
        if (tran == null) return null;
        return tran.gameObject;
    }
	public static GameObject Instance (GameObject _obj)
	{
		return UnityEngine.GameObject.Instantiate (_obj);
	}
    public static GameObject AddChild(Transform parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab, parent);
        //go.transform.localScale = prefab.transform.localScale;
        //go.transform.localPosition = prefab.transform.localPosition;
        return go;
    }

    public static T AddChild<T>(Transform parent, GameObject prefab) where T : Component
    {
        GameObject go = AddChild(parent, prefab);
        return go.GetComponent<T>();
    }

    /// 计算字符串的MD5值
    public static string MD5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    /// 计算文件的MD5值
	public static string MD5File(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

	/// 取得文件大小
    public static long GetFileSize(string file)
    {
        FileStream fs = new FileStream(file, FileMode.Open);
        long size = fs.Length;
        fs.Close();
        return size;
	}

	/// 取得行文本
	public static string GetFileText(string path)
	{
		return File.ReadAllText(path);
	}

	/// 创建文件夹
	public static void CreateFolder(string path)
	{
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);
	}

    /// 清除所有子节点
    public static void ClearChild(Transform go)
    {
        if (go == null) return;
        for (int i = go.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(go.GetChild(i).gameObject);
        }
    }

    /// 清理内存
    public static void ClearMemory()
    {
        GC.Collect(); 
		Resources.UnloadUnusedAssets();
    }

    public static void CallLuaFunction(string name)
    {
        LuaManager manager = MainGame.GetManager<LuaManager>();
        LuaInterface.LuaFunction func = manager.GetFunction(name);
        if (func != null)
        {
            func.Call();
            func.Dispose();
            func = null;
        }
    }

    public static void CallLuaFunction<T>(string name, T arg)
    {
        LuaManager manager = MainGame.GetManager<LuaManager>();
        LuaInterface.LuaFunction func = manager.GetFunction(name);
        if (func != null)
        {
            func.Call<T>(arg);
            func.Dispose();
            func = null;
        }
    }

    public static void CallLuaFunction<T1, T2>(string name, T1 arg1, T2 arg2)
    {
        LuaManager manager = MainGame.GetManager<LuaManager>();
        LuaInterface.LuaFunction func = manager.GetFunction(name);
        if (func != null)
        {
            func.Call<T1, T2>(arg1, arg2);
            func.Dispose();
            func = null;
        }
    }

    public static void CallLuaFunction<T1, T2, T3>(string name, T1 arg1, T2 arg2, T3 arg3)
    {
        LuaManager manager = MainGame.GetManager<LuaManager>();
        LuaInterface.LuaFunction func = manager.GetFunction(name);
        if (func != null)
        {
            func.Call<T1, T2, T3>(arg1, arg2, arg3);
            func.Dispose();
            func = null;
        }
    }

    public static R InvokeLuaFunction<T, R>(string name, T arg)
    {
        LuaManager manager = MainGame.GetManager<LuaManager>();
        LuaInterface.LuaFunction func = manager.GetFunction(name);
        if (func != null)
        {
            R ret = func.Invoke<T, R>(arg);
            func.Dispose();
            func = null;
            return ret;
        }
        return default(R);
    }

    public static R InvokeLuaFunction<T1, T2, R>(string name, T1 arg1, T2 arg2)
    {
        LuaManager manager = MainGame.GetManager<LuaManager>();
        if (manager == null) return default(R);

        LuaInterface.LuaFunction func = manager.GetFunction(name);
        if (func == null) return default(R);

        R ret = func.Invoke<T1, T2, R>(arg1, arg2);
        func.Dispose();
        func = null;
        return ret;
    }

    /// 网络可用
    public static bool NetAvailable
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }

    /// 是否是无线
    public static bool IsWifi
    {
        get
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }

    public static void Log(string str)
    {
        if (AppConst.IsLogger)
	        Debug.Log(str);
    }

    public static void LogWarning(string str)
	{
        Debug.LogWarning(str);
    }

    public static void LogError(string str)
	{
        Debug.LogError(str);
	}

	public static void LogUI(string str)
	{
		UIDebug.Log(str);
	}

    public static void LogFile(string str)
    {
        string path = Application.persistentDataPath + "/log.txt";
		long size = GetFileSize(path);
        bool append = size < 2 * 1024 * 1024;
        StreamWriter sw = new StreamWriter(path, append);
        sw.WriteLine(str);
        sw.Close();
    }

    /// 屏幕坐标转本地坐标
    public static Vector2 ScreenPointToLocal(RectTransform parent, Vector2 pos)
    {
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, pos, Camera.main, out result);
        return result;
    }

    public static Vector2 ScreenPointToWorld(RectTransform parent,Vector2 pos)
    {
        Vector3 result;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(parent, pos, Camera.main, out result);
        return result;
    }

    /// 播放视频
    public static void PlayMovie(string name)
    {
#if UNITY_ANDROID || UNITY_IPHONE
        //--FullScreenMovieControlMode.Full 0 有UI固定时间自动隐藏，返回键在显示UI情况下隐藏UI，不显示UI情况下退出
        //--FullScreenMovieControlMode.Minimal 1 同full
        //--FullScreenMovieControlMode.CancelOnInput 2 没有UI，点击屏幕或返回键退出
        //--FullScreenMovieControlMode.Hidden 3 没有UI，返回键退出
        Handheld.PlayFullScreenMovie(name, Color.black, FullScreenMovieControlMode.CancelOnInput);
#endif
    }

    public static void CopyText(string text)
    {
#if UNITY_ANDROID
#elif UNITY_IPHONE
#else
        TextEditor te = new TextEditor();
        te.text = text;
        te.OnFocus();
        te.Copy();
#endif
    }

    //旧代码
    	public static void SetPosition (GameObject _child, float _x, float _y, float _z)
	{
		if (_child == null)
		{
			return;
		}
		_child.transform.position = new Vector3 (_x, _y, _z);
	}
    	public static void ScaleTo (GameObject _go, float _x, float _y, float _z, float _time, TweenCallback _end)
	{
		Transform _t = _go.GetComponent<Transform> ();
		Tween _tween = DOTween.To (()=>_t.transform.localScale, _pos=>_t.transform.localScale = _pos, new Vector3 (_x, _y, _z), _time);
		_tween = _tween.OnComplete (_end);
		_tween = _tween.SetEase (Ease.Linear);
		_tween.PlayForward ();
	}	
    	public static void AddSpineEvent(GameObject go,LuaInterface.LuaFunction func)
	{
		SkeletonGraphic com = go.GetComponent<SkeletonGraphic>();
		com.AnimationState.Complete += delegate {
                func.Call();
            };
	}
    public static void AddSpineListener(SkeletonGraphic com,LuaInterface.LuaFunction func)
    {
        com.AnimationState.Event += (trackIndexs,eventdata) => {
            func.Call(eventdata.data.name);
        };
    }
    	public static void SortingOrderTo (GameObject _go, int _x, float _time)
	{
		Canvas _t = _go.GetComponent<Canvas> ();
		_t.overrideSorting = true;
		Tween _tween = DOTween.To (()=>_t.sortingOrder, _pos=>_t.sortingOrder = _pos, _x, _time);
		_tween = _tween.SetEase (Ease.Linear);
		_tween.PlayForward ();
	}
    	public static void MoveToWorld (GameObject _go, float _x, float _y, float _z, float _time, Ease _e)
	{
		Transform _t = _go.GetComponent<Transform> ();
		Tween _tween = DOTween.To (()=>_t.transform.position, _pos=>_t.transform.position = _pos, new Vector3 (_x, _y, _z), _time);
		_tween = _tween.SetEase (_e);
		_tween.PlayForward ();
	}
    	public static void SetUIAnchorMin (GameObject _child, float _x, float _y)
	{
		if (_child == null)
		{
			return;
		}
		RectTransform _rt = _child.GetComponent<RectTransform> ();
		if (_rt == null)
		{
			return;
		}
		_x = Mathf.Clamp01 (_x);
		_y = Mathf.Clamp01 (_y);
		_rt.anchorMin = new Vector2 (_x, _y);
	}
    	public static float Distance (Vector3 _start, Vector3 _end)
	{
		return Vector3.Distance (_start, _end);
	}
    	public static void SetUIAnchorMax (GameObject _child, float _x, float _y)
	{
		if (_child == null)
		{
			return;
		}
		RectTransform _rt = _child.GetComponent<RectTransform> ();
		if (_rt == null)
		{
			return;
		}
		_x = Mathf.Clamp01 (_x);
		_y = Mathf.Clamp01 (_y);
		_rt.anchorMax = new Vector2 (_x, _y);
	}
    	public static void SetUISize (GameObject _child, float _x, float _y)
	{
		if (_child == null)
		{
			return;
		}
		RectTransform _rt = _child.GetComponent<RectTransform> ();
		if (_rt == null)
		{
			return;
		}
		_rt.sizeDelta = new Vector2 (_x, _y);
	}
    	public static void SetCanvasSortingOrder (GameObject _go, int _order)
	{
		if (_go == null) return;
		Canvas _c = _go.GetComponent<Canvas> ();
		if (_c == null) return;
		_c.overrideSorting = true;
		_c.sortingOrder = _order;
	}
    	public static GameObject GetChild(GameObject _go, string _subnode)
	{
		Transform _tran = _go.transform.Find(_subnode);
		if (_tran == null) return null;
		return _tran.gameObject;
	}
    	public static void SliderTo (GameObject _go,float startvalue,float endvalue, float _time)
	{
		Slider _t = _go.GetComponent<Slider> ();
		_t.value = startvalue;
		Tween _tween = DOTween.To (()=>_t.value, _pos=>_t.value = _pos, endvalue, _time);
		_tween = _tween.SetEase (Ease.Linear);
		_tween.PlayForward ();
	}
	public static void SetParent (GameObject _child, GameObject _father)
	{
		if (_child == null)
		{
			return;
		}
		if (_father == null) 
		{
			_child.transform.parent = null;
		}
		else
		{
			_child.transform.SetParent (_father.transform);
			SetLocalPosition (_child, 0, 0, 0);
			SetLocalScale (_child, 1, 1, 1);
			SetLocalEulerAngles (_child, 0, 0, 0);
		}
	}
    public static void SetLocalScale (GameObject _child, float _x, float _y, float _z)
	{
		if (_child == null)
		{
			return;
		}
		_child.transform.localScale = new Vector3 (_x, _y, _z);
	}
    	public static void SetLocalPosition (GameObject _child, float _x, float _y, float _z)
	{
		if (_child == null)
		{
			return;
		}
		_child.transform.localPosition = new Vector3 (_x, _y, _z);
	}
    	public static void SetLocalEulerAngles (GameObject _child, float _x, float _y, float _z)
	{
		if (_child == null)
		{
			return;
		}
		_child.transform.localEulerAngles = new Vector3 (_x, _y, _z);
	}
    	public static void DestroyAllChild (GameObject _obj)
	{
		if (_obj == null)
		{
			return;
		}
		Transform _t = _obj.transform;
		int _length = _t.childCount;
		for (int i = 0; i < _length; i++)
		{
			GameObject.Destroy (_t.GetChild(i).gameObject);
		}
	}
        public static void SetUIPivot (GameObject _child, float _x, float _y)
	{
		if (_child == null)
		{
			return;
		}
		RectTransform _rt = _child.GetComponent<RectTransform> ();
		if (_rt == null)
		{
			return;
		}
		_x = Mathf.Clamp01 (_x);
		_y = Mathf.Clamp01 (_y);
		_rt.pivot = new Vector2 (_x, _y);
	}
    	public static AudioListener AddAudioListener(GameObject _go)
	{
		return _go.AddComponent<AudioListener> ();
	}
    	public static AudioSource AddAudioSource(GameObject _go)
	{
		return _go.AddComponent<AudioSource> ();
	}
    	public static void DestroyDelay (GameObject _obj, float _time)
	{
		if (_obj == null) return;
		GameObject.Destroy (_obj, _time);
	}
    	public static void SetText (GameObject _go, string _s)
	{
		if (_go == null)
		{
			return;
		}
		Text _t = _go.GetComponent<Text> ();
		if (_t != null)
		{
			_t.text = _s;
		}
	}
    	public static Tween MoveTo (GameObject _go, float _x, float _y, float _z, float _time, Action _end)
	{
		Transform _t = _go.GetComponent<Transform> ();
		Tween _tween = DOTween.To (()=>_t.transform.localPosition, _pos=>_t.transform.localPosition = _pos, new Vector3 (_x, _y, _z), _time);
		_tween = _tween.SetEase (Ease.Linear);
		_tween.PlayForward ();
		return _tween;
	}
            public static void AddChangeEvent(GameObject go, int id, LuaInterface.LuaFunction func)
    {
        Slider slider = go.GetComponent<Slider>();
        if (slider != null)
        {
            slider.onValueChanged.AddListener((value) => {
                func.Call(slider, id, value);
            });
            return;
        }

        Toggle toggle = go.GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener((value) => {
                func.Call(toggle, id, value);
            });
            return;
        }

        InputField input = go.GetComponent<InputField>();
        if (input != null)
        {
            input.onValueChanged.AddListener((value) => {
                func.Call(input, id, value);
            });
        }

        ScrollRect scroll = go.GetComponent<ScrollRect>();
        if (scroll != null)
        {
            scroll.onValueChanged.AddListener((value) => {
                func.Call(scroll, id, value);
            });
        }
        Dropdown dropdown = go.GetComponent<Dropdown>();
        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener((value) => {
                func.Call(scroll, id, value);
            });
        }
    }
    public static void AddChangeEvent(Transform trans, int id, LuaInterface.LuaFunction func)
    {
        AddChangeEvent(trans.gameObject, id, func);
    }

    public static void AddChangeEvent(Component com, int id, LuaInterface.LuaFunction func)
    {
        AddChangeEvent(com.gameObject, id, func);
    }
    public static void AddSetCanvasSortingOrder(GameObject _go, int _order)
	{

		Canvas _c = _go.GetComponent<Canvas> ();
		if (_c == null)
		{
			_c=_go.AddComponent<Canvas>();
		}

		_c.additionalShaderChannels = AdditionalCanvasShaderChannels.Normal;
		SetCanvasSortingOrder (_go,_order);

	}

    public static T AddMissComponent<T> (GameObject pGo) where T : Component
    {
        T go = pGo.GetComponent<T>();
        if (go == null)
            go = pGo.AddComponent<T>();
        return go;
    }


}