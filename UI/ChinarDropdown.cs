using LuaInterface;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChinarDropdown : Dropdown {

    int count = 0;
    public LuaFunction brocast;

    private void BrocastItem(OptionData data,DropdownItem item)
    {
        if (data == null)
            return;
        if (brocast != null)
            brocast.Call(data.text, item);
    }

    protected override DropdownItem CreateItem(DropdownItem itemTemplate)
    {
        DropdownItem item = base.CreateItem(itemTemplate);
        BrocastItem(options[count], item);
        count++;
        return item;
    }

    protected override void DestroyBlocker(GameObject blocker)
    {
        count = 0;
        base.DestroyBlocker(blocker);
    }
}
