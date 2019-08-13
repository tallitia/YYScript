using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

[ExecuteInEditMode]
public class Disk : MonoBehaviour {

	public Transform[] childs;
	public RigidBodyBallRoot[] childController;

	public Transform f;
	public Transform b;

	// Update is called once per frame
	void Update () 
	{

		float _d = f.position.z - b.position.z;
		int _minCount = 0;
		float _min = float.MaxValue;
		for (int i = 0; i < childs.Length; i++) 
		{
			float _d1 = f.position.z - childs [i].position.z;
			float _bl = _d1 / _d;
			_bl = Mathf.Min (_bl, 1);
			childs [i].localScale = Vector3.one * (1 - _bl * 0.4f);

			//保持抽奖文字不变
			//childs[i].Find("Text").localScale=Vector3.one*(1 + _bl*0.6f);


			if (_bl < _min) 
			{
				_minCount = i;
				_min = _bl;
			}
		}
		for (int i = 0; i < childs.Length; i++) 
		{
			Canvas _c = childs [i].GetComponent<Canvas> ();
			_c.overrideSorting = true;
			if (i == _minCount) 
			{
				_c.sortingOrder = 3;
				/*for (int j = 0; j < childController[i].balls.Length; j++)
				{
					childController [i].balls [j].bodyType = RigidbodyType2D.Dynamic;
				}
				childController [i].boxs.SetActive (true);*/
			} 
			else 
			{
				_c.sortingOrder = 1;
				/*for (int j = 0; j < childController[i].balls.Length; j++)
				{
					childController [i].balls [j].bodyType = RigidbodyType2D.Static;
				}
				childController [i].boxs.SetActive (false); */
			}
		}
	}

	private int angle = 0;
	private float current = 0;
	public int Left ()
	{
		angle += 120;
		DOTween.To (() => current, _v => OnMove (_v), angle, 0.5f);
		return angle;
	}
	public int Right ()
	{
		angle -= 120;
		DOTween.To (() => current, _v => OnMove (_v), angle, 0.5f);
		return angle;
	}

	private void OnMove (float _f)
	{
		current = _f;
		transform.localEulerAngles = new Vector3 (20, 0, current);
		for (int i = 0; i < childs.Length; i++) 
		{
			childs [i].eulerAngles = Vector3.zero;
		}
	}
}
