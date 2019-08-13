using UnityEngine;
using UnityEngine.UI;
using LuaInterface;

public class UIListener : MonoBehaviour
{
    public LuaFunction onChange;
    public LuaFunction onSubmit;
    public object data;
    void Start()
    {
        InputField input = GetComponent<InputField>();
        if (input != null)
        {
            if (onChange != null)
            {
                input.onValueChanged.AddListener((value) => {
                    onChange.Call(value, data);
                });
            }
            if (onSubmit != null)
            {
                input.onEndEdit.AddListener((value) => {
                    onSubmit.Call(value, data);
                });
            }
            return;
        }

        Slider slider = GetComponent<Slider>();
        if (slider != null)
        {
            slider.onValueChanged.AddListener((value) => {
                onChange.Call(value, data);
            });
            return;
        }

        Toggle toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener((value) => {
                onChange.Call(value, data);
            });
            return;
        }

        ScrollRect scroll = GetComponent<ScrollRect>();
        if (scroll != null)
        {
            scroll.onValueChanged.AddListener((value) => {
                onChange.Call(value, data);
            });
            return;
        }

        Dropdown dropdown = GetComponent<Dropdown>();
        if(dropdown != null)
        {
            dropdown.onValueChanged.AddListener((value) =>{
                onChange.Call(value, data);
            });
            return;
        }
    }

    void OnDestroy()
    {
        if (onChange != null)
        {
            onChange.Dispose();
            onChange = null;
        }
        if (onSubmit != null)
        {
            onSubmit.Dispose();
            onSubmit = null;
        }
        data = null;
    }
    
    static public UIListener Get(GameObject go)
    {
        UIListener listener = go.GetComponent<UIListener>();
        if (listener == null) listener = go.AddComponent<UIListener>();
        return listener;
    }

    static public UIListener Get(Transform trans)
    {
        UIListener listener = trans.GetComponent<UIListener>();
        if (listener == null) listener = trans.gameObject.AddComponent<UIListener>();
        return listener;
    }

    static public UIListener Get(Component com)
    {
        UIListener listener = com.GetComponent<UIListener>();
        if (listener == null) listener = com.gameObject.AddComponent<UIListener>();
        return listener;
    }

    static public UIListener Get(GameObject go, object data)
    {
        UIListener listener = go.GetComponent<UIListener>();
        if (listener == null) listener = go.AddComponent<UIListener>();
        listener.data = data;
        return listener;
    }

    static public UIListener Get(Transform trans, object data)
    {
        UIListener listener = trans.GetComponent<UIListener>();
        if (listener == null) listener = trans.gameObject.AddComponent<UIListener>();
        listener.data = data;
        return listener;
    }

    static public UIListener Get(Component com, object data)
    {
        UIListener listener = com.GetComponent<UIListener>();
        if (listener == null) listener = com.gameObject.AddComponent<UIListener>();
        listener.data = data;
        return listener;
    }
}
