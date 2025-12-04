using UnityEngine;
using System.Collections;

namespace TunnelGame
{
	public class Drop : MonoBehaviour
	{
		#region Inspector Variables

		[SerializeField] private float collisionSize;

		#endregion
		
		#region Properties
		
		public float CollisionSize { get { return collisionSize; } }
		
		#endregion
	}
}
