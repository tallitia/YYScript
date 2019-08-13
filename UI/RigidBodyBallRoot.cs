using UnityEngine;

public class RigidBodyBallRoot : MonoBehaviour
{
	public Rigidbody2D[] balls;
	public Rigidbody2D endBall;
	public Transform bottom;
	public Transform[] tops;
	public GameObject boxs;

	public bool open = false;
	public int forces = 10000;
	public int lc = 30;

	private System.Action<GameObject> OnEndAction = null;

	private Vector3 endBallPos;
	private Transform endRoot;

	private void Awake ()
	{
		endBallPos = endBall.transform.localPosition;
		endRoot = endBall.transform.parent;
		endBall.gameObject.SetActive (false);
	}

	private void Update ()
	{
		if (!open) {
			return;
		}
		Run ();
	}

	private void Run ()
	{
		for (int i = 0; i < balls.Length; i++) 
		{
			if (balls [i].transform.localPosition.y < lc) 
			{
				Transform _t = tops [Random.Range (0, tops.Length)];
				Vector3 _v = _t.position - bottom.position;
				_v.Normalize ();
				balls [i].AddForce ((Vector2)_v * forces);
			}
			float _x = Mathf.Abs (balls[i].transform.localPosition.x);
			float _y = Mathf.Abs (balls[i].transform.localPosition.y);

			if (_x > 100 || _y > 200)
			{
				balls [i].transform.localPosition = Vector3.zero;
			}
		}
	}

	public void ReSet ()
	{
		open = false;
		endBall.transform.SetParent (endRoot);
		endBall.transform.localPosition = endBallPos;
		endBall.transform.localScale = Vector3.one;
		endBall.transform.localEulerAngles = Vector3.zero;
		endBall.gameObject.SetActive (false);

		for (int i = 0; i < balls.Length; i++) 
		{
			float _x = Mathf.Abs (balls[i].transform.localPosition.x);
			float _y = Mathf.Abs (balls[i].transform.localPosition.y);

			if (_x > 500 || _y > 500)
			{
				balls [i].transform.localPosition = Vector3.zero;
			}
		}
	}
	public void OnStart ()
	{
		ReSet ();
		open = true;
		endBall.bodyType = RigidbodyType2D.Dynamic;
	}
	public void OnEnd (System.Action<GameObject> _a)
	{
		endBall.gameObject.SetActive (true);
		OnEndAction = _a;
		endBall.AddForce (Vector2.right * 10000);
	}

	public void OnTriggerEnter2D (Collider2D _c)
	{
		open = false;
		endBall.bodyType = RigidbodyType2D.Static;
		if (OnEndAction != null) 
		{
			OnEndAction (endBall.gameObject);
			OnEndAction = null;
		}
	}
}