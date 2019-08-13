using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventListener : MonoBehaviour, IPointerClickHandler
{
    public Action<GameObject> onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick.Invoke(gameObject);
    }

    static public EventListener Get(GameObject go)
    {
        EventListener listener = go.GetComponent<EventListener>();
        if (listener == null) listener = go.AddComponent<EventListener>();
        return listener;
    }

    static public EventListener Get(Transform trans)
    {
        EventListener listener = trans.GetComponent<EventListener>();
        if (listener == null) listener = trans.gameObject.AddComponent<EventListener>();
        return listener;
    }

    static public EventListener Get(Component com)
    {
        EventListener listener = com.GetComponent<EventListener>();
        if (listener == null) listener = com.gameObject.AddComponent<EventListener>();
        return listener;
    }
}
