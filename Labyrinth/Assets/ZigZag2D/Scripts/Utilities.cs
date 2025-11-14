using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TunnelGame
{
	public static class Utilities
	{
		#region Public Methods

		/// <summary>
		/// Returns the world width of the orthographic camera.
		/// </summary>
		public static float WorldWidth(Camera cam)
		{
			return 2f * cam.orthographicSize * cam.aspect;
		}

		/// <summary>
		/// Returns the world height of the orthographic camera.
		/// </summary>
		public static float WorldHeight(Camera cam)
		{
			return 2f * cam.orthographicSize;
		}

		/// <summary>
		/// Returns to mouse position
		/// </summary>
		public static Vector2 MousePosition(int index)
		{
			#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
			return (index == 0) ? (Vector2)Input.mousePosition : Vector2.zero;
			#else
			if (Input.touchCount > index)
			{
				return Input.touches[index].position;
			}

			return Vector2.zero;
			#endif
		}

		/// <summary>
		/// Returns true if a mouse down event happened, false otherwise
		/// </summary>
		public static bool MouseDown(int index)
		{
			return (index == 0 && Input.GetMouseButtonDown(0)) || (Input.touchCount > index && Input.touches[index].phase == TouchPhase.Began);
		}
		
		/// <summary>
		/// Returns true if a mouse up event happened, false otherwise
		/// </summary>
		public static bool MouseUp(int index)
		{
			return (index == 0 && Input.GetMouseButtonUp(0)) || (Input.touchCount > index && Input.touches[index].phase == TouchPhase.Ended);
		}

		#endregion
	}
}









