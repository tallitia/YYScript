using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using GameFramework;

namespace PK_Game
{
	[ExecuteInEditMode]
	public class PKSpriteAnimation : GFMonoBehaviour
	{
		public enum Action
		{
			Up = 0,
			Right = 1,
			Left = 2,
			Down = 3,
		}

		public Texture2D texture;
		public int hCount = 4;
		public int VCount = 4;

		//new 6  old 10
		public int frame = 6;
		public int speed = 1;
		public float onceTime
		{
			get
			{
				return space * hCount;
			}
		}
		public int limit = 0;
		public bool isNative = true;
		public bool ui = false;
		public bool loop = true;
		private bool stop = false;
        public Action aniTriget = Action.Down;
		private Action lastAniTriget = Action.Down;

		private Sprite[] sprites;
		private float space
		{
			get
			{
				return 1f / frame / speed;
			}
		}
		private Image image;
		private SpriteRenderer srenderer;

		private float lastTime;

		private int startC;
		private int endC;
		private int currentC;

	
		private void Awake ()
		{
						
			if (texture == null) return;
			int _wc = texture.width / hCount;
			int _hc = texture.height / VCount;
			sprites = new Sprite[hCount * VCount];
			Sprite _temp;
			int _count = 0;
			for (int j = 0; j < VCount; j++)
			{
				for (int i = 0; i < hCount; i++)
				{
					_temp = Sprite.Create(texture, new Rect(_wc * i, _hc * j, _wc, _hc), new Vector2(0.5f, 0.5f));
					sprites[_count] = _temp;
					_temp.name = _count.ToString();
					_count++;
				}
			}
			SetComponent (_wc, _hc);
			Play ();			

		}


		public void SetComponent ()
		{
			SetComponent (0, 0);
		}
		private void SetComponent (int _width, int _heigth)
		{
			if (ui)
			{
				if (srenderer) Object.Destroy (srenderer);
				image = this.gameObject.GetComponent<Image> ();
				if (image == null)
				{
					image = this.gameObject.AddComponent<Image> (); 
				}
				image.raycastTarget = false;
				image.sprite = sprites[0];
				if (_width > 0 && _heigth > 0)
				{
					image.rectTransform.sizeDelta = new Vector2 (_width, _heigth);
				}
			}
			else
			{
				if (image) Object.Destroy (image);
				srenderer = this.gameObject.GetComponent<SpriteRenderer> ();
				if (srenderer == null)
				{
					srenderer = this.gameObject.AddComponent<SpriteRenderer> ();
				}
				srenderer.sprite = sprites[0];
			}
		}

		public void Play ()
		{
			Calc (true);
			this.gameObject.SetActive (true);
		}
        public void SetStop(bool _flag)
        {
			stop = _flag;
        }
        public void SetAction(int act)
        {
			if (aniTriget != (Action)act)
			{
				aniTriget = (Action)act;
				Calc (true);
				ChangeFrame (currentC);
			}
        }
		private void CheckLimit ()
		{
			if (limit <= 0) 
			{
				return;
			}
			if (image != null) 
			{
				float _w = image.rectTransform.sizeDelta.x;
				float _h = image.rectTransform.sizeDelta.y;
				if (_w > limit) 
				{
					float _bl = limit / _w;
					_w *= _bl;
					_h *= _bl;
				}
				if (_h > limit) 
				{
					float _bl = limit / _h;
					_w *= _bl;
					_h *= _bl;
				}
				image.rectTransform.sizeDelta = new Vector2 (_w, _h);
				return;
			}
		}
        private void Calc (bool _force = false)
		{
			if (lastAniTriget != aniTriget || _force)
			{
				lastAniTriget = aniTriget;
				startC = (int)aniTriget * hCount;
				endC = startC + hCount;
				if (startC > sprites.Length)
				{
					startC = 0;
					currentC = startC;
					endC = startC + hCount;
					return;
				}

				currentC = startC;
			}
		}
		private void End ()
		{
			this.gameObject.SetActive (false);
		}

		private void ChangeFrame(int frame)
		{
			Sprite _s = sprites[frame];
			if (ui)
			{
				if (image == null)
				{
					SetComponent ();
				}
				image.sprite = _s;
				if (isNative) image.SetNativeSize ();
				CheckLimit ();
			}
			else
			{
				if (srenderer == null)
				{
					SetComponent ();
				}
				srenderer.sprite = _s;
			}
		}

		private void Update ()
		{
            if (stop)
            {
                image.sprite = sprites[(int)aniTriget * hCount];
				if (isNative) image.SetNativeSize();
				CheckLimit ();
                return;
            }
			Calc ();
			if (Time.time - lastTime > space)
			{
				lastTime = Time.time;
				currentC++;
				if (currentC >= endC)
				{
					if (loop)
					{
						currentC = startC;
					}
					else
					{
						End ();
						return;
					}
				}
				Sprite _s = sprites[currentC];
				if (ui)
				{
					if (image == null)
					{
						SetComponent ();
					}
					image.sprite = _s;
					if (isNative) image.SetNativeSize ();
					CheckLimit ();
				}
				else
				{
					if (srenderer == null)
					{
						SetComponent ();
					}
					srenderer.sprite = _s;
				}
			}
		}
		
	}
}