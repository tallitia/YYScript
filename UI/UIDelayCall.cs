using UnityEngine;
using LuaInterface;

public class UIDelayCall : MonoBehaviour
{
    public LuaFunction onFinish;
    public float delayTime = 0f;
    public object data;

    void Update()
    {
        delayTime -= Time.unscaledDeltaTime;
        if (delayTime <= 0f)
        {
            enabled = false;
            if (onFinish != null)
                onFinish.Call(data);
            return;
        }
    }

    void OnDestroy()
    {
        if (onFinish != null)
        {
            onFinish.Dispose();
            onFinish = null;
        }
        data = null;
    }

    static public UIDelayCall Get(GameObject go)
    {
        UIDelayCall listener = go.GetComponent<UIDelayCall>();
        if (listener == null) listener = go.AddComponent<UIDelayCall>();
        listener.enabled = true;
        return listener;
    }

    static public UIDelayCall Get(Transform trans)
    {
        UIDelayCall listener = trans.GetComponent<UIDelayCall>();
        if (listener == null) listener = trans.gameObject.AddComponent<UIDelayCall>();
        listener.enabled = true;
        return listener;
    }

    static public UIDelayCall Get(Component com)
    {
        UIDelayCall listener = com.GetComponent<UIDelayCall>();
        if (listener == null) listener = com.gameObject.AddComponent<UIDelayCall>();
        listener.enabled = true;
        return listener;
    }
}