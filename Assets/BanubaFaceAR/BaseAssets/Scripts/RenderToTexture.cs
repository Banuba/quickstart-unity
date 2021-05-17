using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNB
{
	[RequireComponent(typeof(Camera))]
	public class RenderToTexture : MonoBehaviour
	{

		//[HideInInspector]
		public RenderTexture texture;

		public Vector2 size;

		[SerializeField]
		public bool screenSize = true;

		public RenderTextureFormat format;

		public int depth;

		void Awake()
		{
			if (screenSize)
			{
				texture = new RenderTexture(Screen.width, Screen.height, depth, format);
			}
			else
			{
				texture = new RenderTexture((int)size.x, (int)size.y, depth, format);
			}
			GetComponent<Camera>().targetTexture = texture;
		}

        public void setRenderTargetSize(int w, int h)
        {
			UnityEngine.GameObject.Destroy(texture);
			texture = new RenderTexture(w, h, depth, format);
			GetComponent<Camera>().targetTexture = texture;
        }
	}
}

