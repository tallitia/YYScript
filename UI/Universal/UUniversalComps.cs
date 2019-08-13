using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UUniversalComps : MonoBehaviour {

    public UIResource resources = new UIResource();
    private bool m_IsDispose = false;

    protected void OnDestroy()
    {
        if (m_IsDispose)
            return;
        m_IsDispose = true;
        if(resources != null && resources.componentItems != null)
        {
            for (int i = 0; i < resources.componentItems.Length; i++)
            {
                UIResourceComponentItem p_item = resources.componentItems[i];
                p_item.value = null;
                p_item = null;
            }
            resources.componentItems = null;
        }
    }
}
