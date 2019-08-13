using UnityEngine;
using System.Collections;

namespace GameFramework
{
	public class GFMonoBehaviour : MonoBehaviour
	{
		public Transform parent
		{
			get
			{
				return this.transform.parent;
			}
		}
		public Vector3 position
		{
			get
			{
				return this.transform.position;
			}
			set
			{
				this.transform.position = value;
			}
		}
		public Quaternion rotation
		{
			get
			{
				return this.transform.rotation;
			}
			set
			{
				this.transform.rotation = value;
			}
		}
		public Vector3 eulerAngles
		{
			get
			{
				return this.transform.eulerAngles;
			}
			set
			{
				this.transform.eulerAngles = value;
			}
		}
		public Vector3 lossyScale
		{
			get
			{
				return this.transform.lossyScale;
			}
		}
		public Vector3 localPosition
		{
			get
			{
				return this.transform.localPosition;
			}
			set
			{
				this.transform.localPosition = value;
			}
		}
		public Quaternion localRotation
		{
			get
			{
				return this.transform.localRotation;
			}
			set
			{
				this.transform.localRotation = value;
			}
		}
		public Vector3 localEulerAngles
		{
			get
			{
				return this.transform.localEulerAngles;
			}
			set
			{
				this.transform.localEulerAngles = value;
			}
		}
		public Vector3 localScale
		{
			get
			{
				return this.transform.localScale;
			}
			set
			{
				this.transform.localScale = value;
			}
		}
	}
}
