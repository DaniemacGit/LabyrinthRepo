using UnityEngine;
using System.Collections;

namespace TunnelGame
{
	public class Player : MonoBehaviour
	{
		#region Inspector Variables

		[SerializeField] private float collisionSize;

		#endregion

		#region Properties

		public float CollisionSize { get { return collisionSize; } }

		#endregion
	}
}
