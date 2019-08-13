using System;

[Serializable]
public class UIResourceComponentItem
{
    public string key;     //保存组件的Key
    public string typeName; //组件类型
    public UnityEngine.Object value; //组件

}

[Serializable]
public class UIResource
{
    public UIResourceComponentItem[] componentItems;
}