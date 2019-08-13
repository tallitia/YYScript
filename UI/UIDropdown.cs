using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using LuaInterface;

[RequireComponent(typeof(ChinarDropdown))]
public class UIDropdown : MonoBehaviour
{
    private ChinarDropdown mDropdown;
    private List<Dropdown.OptionData> mOptions;
    private LuaFunction func;
    void Awake()
    {
        mOptions = new List<Dropdown.OptionData>();
        mDropdown = GetComponent<ChinarDropdown>();
    }

    public void AddOption(string text, Sprite sprite)
    {
        var option = new Dropdown.OptionData(text, sprite);
        mOptions.Add(option);
    }

    public void RemoveOption(string text)
    {
        for (int i=0; i<mOptions.Count; i++)
        {
            if (mOptions[i].text == text)
            {
                mOptions.RemoveAt(i);
                break;
            }
        }
    }

    public void Clear()
    {
        mOptions.Clear();
        mDropdown.ClearOptions();
    }

    public void Refresh()
    {
        mDropdown.ClearOptions();
        mDropdown.AddOptions(mOptions);
    }

    public void AddEvent(LuaFunction func,LuaFunction brocast = null)
    {
        if (mDropdown == null || func == null)
            return;
        this.func = func;
        if (brocast != null)
            mDropdown.brocast = brocast;
        mDropdown.onValueChanged.AddListener((value) =>
        {
            func.Call(value);
        });
    }

    /// <summary>
    /// 展示某一个Item信息
    /// </summary>
    /// <param name="index"></param>
    public void ShowDropItem(int index)
    {
        var data = mDropdown.options[index];
        if (data == null)
            return;
        mDropdown.value = index;
        func.Call(index);
    }
}
