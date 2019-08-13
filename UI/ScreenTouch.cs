using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

public class ScreenTouch : MonoBehaviour {

	List<LuaFunction> EventList;

	// Use this for initialization
	void Start () {

		EventList = new List<LuaFunction> ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	#if UNITY_EDITOR
	    
		if (Input.GetMouseButtonDown(0))
		{
			
			OnTouchEvent(Input.mousePosition);
		}
	#else
		if(Input.touchCount==1)
		{
			if (Input.touches [0].phase == TouchPhase.Began) {
				// 手指按下时，要触发的代码
				OnTouchEvent (Input.touches [0].position);
			}
		}
	#endif
	}

	void OnTouchEvent(Vector2 pos)
	{
		//Debug.Log(EventList);
		for (int i=0;i<EventList.Count;i++)
		{
			if (EventList [i] != null)
			{
				EventList [i].Call (pos);
			}
		}
	}

	//添加事件
	public void AddTouchDownEvent(LuaFunction func)
	{
		EventList.Add (func);
	}

	public void RemoveTouchDownEvent(LuaFunction func)
	{
		EventList.Remove (func);
	}
}
