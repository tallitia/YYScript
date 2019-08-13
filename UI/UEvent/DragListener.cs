using System;
using LuaInterface;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragListener : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
	public LuaFunction onBeginDrag;
	public LuaFunction onDrag;
	public LuaFunction onEndDrag;
    public LuaFunction onDrop;
    public LuaFunction onPointerDown;
    private RectTransform parent;
    private Vector2 pos = new Vector2();//用来接收转换后的拖动坐标

    void OnDestroy() 
	{
        if (onPointerDown != null)
        {
            onPointerDown.Dispose();
            onPointerDown = null;
        }
		if (onBeginDrag != null) 
		{
            onBeginDrag.Dispose();
            onBeginDrag = null;
		}
        if (onDrag != null)
        {
            onDrag.Dispose();
            onDrag = null;
        }
        if (onEndDrag != null)
        {
            onEndDrag.Dispose();
            onEndDrag = null;
        }
        if (onDrop != null)
        {
            onDrop.Dispose();
            onDrop = null;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
	{
		if (onBeginDrag != null) 
			onBeginDrag.Call(gameObject, eventData);
	}

	public void OnDrag (PointerEventData eventData)
	{
        if (onDrag != null)
        {
            if(parent == null) parent = transform.parent as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, eventData.position, eventData.enterEventCamera, out pos);
            onDrag.Call(gameObject, eventData , pos);
        }
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (onEndDrag != null)
			onEndDrag.Call(gameObject, eventData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (onDrop != null)
            onDrop.Call(gameObject, eventData);
    }

    static public DragListener Get(GameObject go)
	{
		DragListener listener = go.GetComponent<DragListener>();
		if (listener == null) listener = go.AddComponent<DragListener>();
		return listener;
	}

	static public DragListener Get(Transform trans)
	{
		DragListener listener = trans.GetComponent<DragListener>();
		if (listener == null) listener = trans.gameObject.AddComponent<DragListener>();
		return listener;
	}
    public void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null)
            onPointerDown.Call(gameObject, eventData);
	}

	static public DragListener Get(Component com)
	{
		DragListener listener = com.GetComponent<DragListener>();
		if (listener == null) listener = com.gameObject.AddComponent<DragListener>();
		return listener;
	}
}
