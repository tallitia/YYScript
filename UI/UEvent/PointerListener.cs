using UnityEngine;
using UnityEngine.EventSystems;
using LuaInterface;


public class PointerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public LuaFunction PointerDown;
    public LuaFunction PointerUp;

    public object data;
    void OnDestroy()
    {
        if (PointerDown != null)
        {
            PointerDown.Dispose();
            PointerDown = null;
        }
        if (PointerUp != null)
        {
            PointerUp.Dispose();
            PointerUp = null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (PointerDown != null)
            PointerDown.Call(gameObject, eventData,data);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (PointerUp != null)
            PointerUp.Call(gameObject, eventData,data);
    }
    
    static public PointerListener Get(GameObject go)
    {
        PointerListener listener = go.GetComponent<PointerListener>();
        if (listener == null) listener = go.AddComponent<PointerListener>();
        return listener;
    }

    static public PointerListener Get(Transform trans)
    {
        PointerListener listener = trans.GetComponent<PointerListener>();
        if (listener == null) listener = trans.gameObject.AddComponent<PointerListener>();
        return listener;
    }

    static public PointerListener Get(Component com)
    {
        PointerListener listener = com.GetComponent<PointerListener>();
        if (listener == null) listener = com.gameObject.AddComponent<PointerListener>();
        return listener;
    }

    static public PointerListener Get(GameObject go, object data)
    {
        PointerListener listener = go.GetComponent<PointerListener>();
        if (listener == null) listener = go.AddComponent<PointerListener>();
        listener.data = data;
        return listener;
    }

    static public PointerListener Get(Transform trans, object data)
    {
        PointerListener listener = trans.GetComponent<PointerListener>();
        if (listener == null) listener = trans.gameObject.AddComponent<PointerListener>();
        listener.data = data;
        return listener;
    }

    static public PointerListener Get(Component com, object data)
    {
        PointerListener listener = com.GetComponent<PointerListener>();
        if (listener == null) listener = com.gameObject.AddComponent<PointerListener>();
        listener.data = data;
        return listener;
    }
}
