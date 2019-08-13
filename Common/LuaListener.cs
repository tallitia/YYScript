using LuaInterface;
using UnityEngine;
using UnityEngine.EventSystems;

public class LuaListener : MonoBehaviour, IPointerClickHandler
{
    public LuaFunction onStart;
    public LuaFunction onDestroy;
    public LuaFunction onClick;
    
	public object data;
    void Start() 
	{ 
		if (onStart != null) 
			onStart.Call(data); 
	}
    void OnDestroy() 
	{ 
		if (onDestroy != null) 
		{
			onDestroy.Call(data); 
            onDestroy.Dispose();
            onDestroy = null;
		}
        if (onStart != null)
        {
            onStart.Dispose();
            onStart = null;
        }
        if (onClick != null)
        {
            onClick.Dispose();
            onClick = null;
        }
		data = null;
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick.Call(gameObject, data, eventData);
    }

    static public LuaListener Get(GameObject go)
    {
        LuaListener listener = go.GetComponent<LuaListener>();
        if (listener == null) listener = go.AddComponent<LuaListener>();
        return listener;
    }

    static public LuaListener Get(Transform trans)
    {
        LuaListener listener = trans.GetComponent<LuaListener>();
        if (listener == null) listener = trans.gameObject.AddComponent<LuaListener>();
        return listener;
    }

    static public LuaListener Get(Component com)
    {
        LuaListener listener = com.GetComponent<LuaListener>();
        if (listener == null) listener = com.gameObject.AddComponent<LuaListener>();
        return listener;
	}

	static public LuaListener Get(GameObject go, object data)
	{
		LuaListener listener = go.GetComponent<LuaListener>();
		if (listener == null) listener = go.AddComponent<LuaListener>();
		listener.data = data;
		return listener;
	}

	static public LuaListener Get(Transform trans, object data)
	{
		LuaListener listener = trans.GetComponent<LuaListener>();
		if (listener == null) listener = trans.gameObject.AddComponent<LuaListener>();
		listener.data = data;
		return listener;
	}

	static public LuaListener Get(Component com, object data)
	{
		LuaListener listener = com.GetComponent<LuaListener>();
		if (listener == null) listener = com.gameObject.AddComponent<LuaListener>();
		listener.data = data;
		return listener;
	}
    //旧代码
    	static public LuaListener GetGameObject(GameObject go)
	{
		LuaListener listener = go.GetComponent<LuaListener>();
		if (listener == null) listener = go.AddComponent<LuaListener>();
		return listener;
	}
}
