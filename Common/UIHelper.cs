using UnityEngine;
using UnityEngine.UI;


public class UIHelper
{
    //仅仅是把图片变灰色，
    public static void SetGray(GameObject pGo, bool pIsGray = true)
    {
        Material m = LuaHelper.GetAssetManager().LoadMaterial("Shaders/Gray.mat");

        Graphic[] comps = pGo.transform.GetComponentsInChildren<UnityEngine.UI.Graphic>();
        foreach (var comp in comps)
        {
            if (pIsGray)
                comp.material = m;
            else
                comp.material = null;
        }
    }

#region RectTransfrom的操作

    public static void SetRectTransfromPos(Transform pSrc, RectTransform pDest)
    {
        
        
    }

    public static void DestoryAllChild(Transform pSrc)
    {
        Transform t;
        for (int i = 0; i < pSrc.childCount; i++)
        {
            t = pSrc.GetChild(i);
            GameObject.Destroy(t.gameObject);
        }
    }

    #endregion



}