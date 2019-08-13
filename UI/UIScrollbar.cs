using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScrollbar : MonoBehaviour, IEndDragHandler
{
    public Scrollbar bar;
    private ScrollRect scrollRect;
    Action action=null;
    public void OnEndDrag(PointerEventData eventData)
    {
        if (bar.value >= 1)
        {
            if (action != null)
                action();
        }
    }
    public void setCall(Action func)
    {
        action = func;
    }
}
