using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TunnelGame
{
	public class GameManager : SingletonComponent<GameManager>
	{
		#region Classes

		[System.Serializable]
		public class PlayerInfo
		{
			public Player	playerPrefab;
			public bool		locked;
			public float	unlockAmount;
			public Sprite	icon;
			public Color	iconColor;
		}

		#endregion

		#region Enums

		public enum Direction
		{
			Left,
			Right
		}

		public enum GameState
		{
			Idle,
			PlayerSelect,
			Playing,
			Over
		}

		#endregion

		#region Inspector Variables

		[SerializeField] private bool		showCollisionBoxesInScene;
		[SerializeField] private Camera		gameCamera;
		[SerializeField] private GameObject	startUI;
		[SerializeField] private GameObject	playerSelectUI;
		[SerializeField] private GameObject	gameUI;
		[SerializeField] private GameObject	overUI;
		
		[SerializeField] private PlayerInfo[]	playerInfos;
		[SerializeField] private float			speed;
		[SerializeField] private float			scoreMultiplier;

		[Range(0f, 1f)]
		[SerializeField] private float		dropChance;
		[SerializeField] private float		dropCollectAmount;
		[SerializeField] private float		dropEdgePadding;
		[SerializeField] private Drop		dropPrefab;

		[SerializeField] private Material	bkgMaterial;
		[SerializeField] private float		bkgMoveSpeed;
		[SerializeField] private float		bkgRepeatSize;

		[SerializeField] private Material	tunnelMaterial;
		[SerializeField] private bool		textureFillsTunnel;
		[SerializeField] private bool		alignTextureWithTunnel;
		[SerializeField] private float		unitSize;
		[SerializeField] private float		textureSize;
		[SerializeField] private float		maxTunnelSizeInUnits;
		[SerializeField] private float		minTunnelSizeInUnits;
		[SerializeField] private float		minTunnelLengthInUnits;
		[SerializeField] private float		maxTunnelLengthInUnits;
		
		#endregion
		
		#region Member Variables
		
		private const float offScreenPadding	= 1000f;	// Extra padding to make sure the player cannot see the "edge" of the tunnel.
		private const float tunnelAngle			= 45f;		// Setting this to anything other than 45 will cause unexpected results, it should only even be 45.

		private GameObject		tunnelGameObject;
		private Mesh			tunnelMesh;
		private List<Vector3> 	tunnelVertices;
		private List<Vector2> 	tunnelUVs;
		private List<Vector3> 	tunnelNormals;
		private List<int>		tunnelTriangles;
		private List<float>		tunnelSizes;

		private Player		player;
		private ObjectPool	dropObjectPool;
		private List<Drop>	drops;
		private Direction	tunnelDirection;
		private Vector2		leftDirection;
		private Vector2		rightDirection;
		private GameState	gameState;
		private Direction	playerDirection;
		private Vector2 	uvStartPoint;

		#endregion
		
		#region Properties

		public PlayerInfo[]	PlayerInfos		{ get { return playerInfos; } }
		public int 			CurrentScore 	{ get; set; }

		// These values are saved across app loads
		public int		CurrentPlayerIndex	{ get { return PlayerPrefs.GetInt("TunnelGame_CurrentPlayerIndex"); }		set { PlayerPrefs.SetInt("TunnelGame_CurrentPlayerIndex", value); } }
		public int 		HighScore			{ get { return PlayerPrefs.GetInt("TunnelGame_HighScore"); }				set { PlayerPrefs.SetInt("TunnelGame_HighScore", value); } }
		public int 		AverageScore		{ get { return PlayerPrefs.GetInt("TunnelGame_AverageScore"); }				set { PlayerPrefs.SetInt("TunnelGame_AverageScore", value); } }
		public int 		TimesPlayed			{ get { return PlayerPrefs.GetInt("TunnelGame_TimesPlayed"); }				set { PlayerPrefs.SetInt("TunnelGame_TimesPlayed", value); } }
		public float	DropsCollected		{ get { return PlayerPrefs.GetFloat("TunnelGame_DropsCollected"); }			set { PlayerPrefs.SetFloat("TunnelGame_DropsCollected", value); } }
		public string	UnlockedPlayerInfos	{ get { return PlayerPrefs.GetString("TunnelGame_UnlockedPlayerInfos"); }	set { PlayerPrefs.SetString("TunnelGame_UnlockedPlayerInfos", value); } }

		// These values return the Y position of the mesh GameObject for the high score and average score
		public float 	HighScoreYPos		{ get { return tunnelGameObject == null ? 0 : tunnelGameObject.transform.position.y + (float)HighScore / scoreMultiplier; } }
		public float 	AverageScoreYPos	{ get { return tunnelGameObject == null ? 0 : tunnelGameObject.transform.position.y + (float)AverageScore / scoreMultiplier; } }

		private float MaxTunnelSizeInPixels		{ get { return maxTunnelSizeInUnits * unitSize; } }
		private float MinTunnelSizeInPixels		{ get { return minTunnelSizeInUnits * unitSize; } }
		private float MaxTunnelLengthInPixels	{ get { return maxTunnelLengthInUnits * unitSize; } }
		private float MinTunnelLengthInPixels	{ get { return minTunnelLengthInUnits * unitSize; } }
		private float OffScreenVertDistance 	{ get { return (MaxTunnelLengthInPixels + MaxTunnelSizeInPixels + Utilities.WorldWidth(gameCamera)) * 2f; } }

		#endregion
		
		#region Unity Methods
		
		private void Start()
		{
			if (!gameCamera.orthographic)
			{
				Debug.LogError("GameCamera must be an orthographic camera!");
				return;
			}

			if (playerInfos.Length == 0)
			{
				Debug.LogError("There are no player infos!");
				return;
			}
			
			if (playerInfos[0].locked)
			{
				Debug.LogWarning("The first player info cannot be locked!");
				playerInfos[0].locked = false;
			}

			// Sets the locked variable to false on all playerInfos whos index appears in the UnlockedPlayerInfos string
			SetSavedUnlockedPlayerInfos();

			tunnelVertices	= new List<Vector3>();
			tunnelUVs		= new List<Vector2>();
			tunnelNormals	= new List<Vector3>();
			tunnelTriangles	= new List<int>();
			tunnelSizes		= new List<float>();

			dropObjectPool	= new ObjectPool(dropPrefab.gameObject, 5, new GameObject("drop_pool").transform);
			drops			= new List<Drop>();

			// Calculate the radians of the tunnel angle
			float rad = Mathf.Deg2Rad * tunnelAngle;

			// Get the direction vectors for left and right based on the angle of the tunnels
			leftDirection.x		= -1f * Mathf.Sin(rad);
			leftDirection.y		= 1f * Mathf.Cos(rad);
			rightDirection.x	= -1f * Mathf.Sin(-rad);
			rightDirection.y	= 1f * Mathf.Cos(-rad);
			
			// Create the player using the CurrentPlayerIndex
			CreatePlayer();

			// Create the background mesh and add it to the main camera
			SetupBackground();

			// Setup the game to be played
			Reset();
		}
		
		private void Update()
		{
			// If the game state is not playing then don't do anything in the update loop
			if (gameState != GameState.Playing)
			{
				// Keep updating the camera position on game over so it smoothly moves to center the player
				if (gameState == GameState.Over)
				{
					UpdateCameraPosition();
				}

				return;
			}

			// If the player tapped the screen switch directions
			if (Utilities.MouseDown(0))
			{
				playerDirection = (playerDirection == Direction.Left) ? Direction.Right : Direction.Left;
			}

			// Calculate how fast the tunnel is falling and the player is moving
			Vector2 dir		= (playerDirection == Direction.Left) ? leftDirection : rightDirection;
			Vector2 move	= speed * Time.deltaTime * dir;

			UpdateBkgMaterialOffsets(dir);
			UpdateBallPosition(move.x);
			UpdateTunnelPosition(move.y);
			UpdateCameraPosition(false);
			CheckCollisions();
			CheckDropsOffScreen();
			
			// The score is determined by the amount moved in the y direction multiplied by the scoreMultiplier rounded to an integer
			CurrentScore = Mathf.RoundToInt(-tunnelGameObject.transform.position.y * scoreMultiplier);
		}
		
		#endregion
		
		#region Public Methods

		public void Play()
		{
			ChangeGameState(GameState.Playing);
		}

		public void Reset()
		{
			// Set the current score to 0
			CurrentScore = 0;

			// Clear the drop info
			drops.Clear();
			dropObjectPool.ReturnAllObjectsToPool();

			// If there exists a tunnel object form a previous game then destroy it
			if (tunnelGameObject != null)
			{
				Destroy(tunnelGameObject);
			}

			// Null out the old mesh
			tunnelMesh = null;

			// Clear all the tunnel mesh information
			tunnelVertices.Clear();
			tunnelUVs.Clear();
			tunnelNormals.Clear();
			tunnelTriangles.Clear();
			tunnelSizes.Clear();

			// Set the player back to origin and call update camera position so the camera centers on the player
			player.transform.position = Vector3.zero;
			UpdateCameraPosition(true);

			// Re-set the directions to the starting directions
			tunnelDirection = Direction.Left;
			playerDirection	= Direction.Left;
			
			// Set game state to Idle
			ChangeGameState(GameState.Idle);
			
			// Create the mesh and the starting vertices
			SetupMesh();
		}

		/// <summary>
		/// Swaps the player prefab that is being used
		/// </summary>
		public void SwapPlayer(int index)
		{
			if (index < 0 || index >= playerInfos.Length)
			{
				Debug.LogError("Player index out of range.");
				return;
			}

			CurrentPlayerIndex = index;

			CreatePlayer();
		}

		/// <summary>
		/// Changes the state of the game and updates the UI
		/// </summary>
		public void ChangeGameState(GameState newGameState)
		{
			startUI.gameObject.SetActive(newGameState == GameState.Idle);
			playerSelectUI.gameObject.SetActive(newGameState == GameState.PlayerSelect);
			gameUI.gameObject.SetActive(newGameState == GameState.Playing);
			overUI.gameObject.SetActive(newGameState == GameState.Over);
			
			gameState = newGameState;
		}

		/// <summary>
		/// Shows the player select UI screen
		/// </summary>
		public void ShowPlayerSelectScreen()
		{
			// Simply change the state to PlayerSelect
			ChangeGameState(GameState.PlayerSelect);
		}

		#endregion
		
		#region Private Methods

		private void UpdateBkgMaterialOffsets(Vector2 dir)
		{
			Vector2 moveAmount = bkgMoveSpeed * Time.deltaTime * dir;

			bkgMaterial.mainTextureOffset += moveAmount;
		}

		/// <summary>
		/// Updates the position of the ball by moving it by the x amount
		/// </summary>
		private void UpdateBallPosition(float xAmount)
		{
			// Move the ball but only in the x direction
			player.transform.position = new Vector3(player.transform.position.x + xAmount, player.transform.position.y);
		}

		/// <summary>
		/// Updates the position of the tunnel by moving it down yAmount
		/// </summary>
		private void UpdateTunnelPosition(float yAmount)
		{
			// Move the tunnel down
			tunnelGameObject.transform.position = new Vector3(0f, tunnelGameObject.transform.position.y - yAmount);
			
			// Check if the tunnels mesh has moved down enough that we can remove the bottom 4 vertices 
			if (tunnelVertices[5].y + tunnelGameObject.transform.position.y <= -Utilities.WorldHeight(gameCamera) / 2f &&
			    tunnelVertices[6].y + tunnelGameObject.transform.position.y <= -Utilities.WorldHeight(gameCamera) / 2f)
			{
				tunnelVertices.RemoveRange(0, 4);
				tunnelUVs.RemoveRange(0, 4);
				tunnelNormals.RemoveRange(0, 4);
				tunnelTriangles.RemoveRange(0, textureFillsTunnel ? 6 : 12);
				
				// Since we removed vertices we now need to go update all the indexes in triangles so they are pointing to the correct vert index
				for (int i = 0; i < tunnelTriangles.Count; i++)
				{
					tunnelTriangles[i] -= 4;
				}
			}
			
			// Check if the tunnel mesh has moved down enough that we need to add to the top
			if (tunnelVertices[tunnelVertices.Count - 2].y + tunnelGameObject.transform.position.y <= Utilities.WorldHeight(gameCamera) / 2f ||
			    tunnelVertices[tunnelVertices.Count - 3].y + tunnelGameObject.transform.position.y <= Utilities.WorldHeight(gameCamera) / 2f)
			{
				// Add another section to the tunnel
				AddTunnel();
				
				// Now refresh the mesh
				RefreshMesh();
			}
		}

		/// <summary>
		/// Updates the x position of the camera so it's centered on the ball
		/// </summary>
		private void UpdateCameraPosition(bool force = false)
		{
			float xDistance = player.transform.position.x - gameCamera.transform.position.x;
			gameCamera.transform.position = new Vector3(gameCamera.transform.position.x + xDistance / (force ? 1f : 10f), gameCamera.transform.position.y, gameCamera.transform.position.z);
		}

		/// <summary>
		/// Checks if the ball is colliding with the tunnel walls
		/// </summary>
		private void CheckCollisions()
		{
			// Check if the player has collided with the walls of the tunnel
			for (int i = 7; i < tunnelVertices.Count; i += 4)
			{
				if (tunnelVertices[i - 1].x > tunnelVertices[i - 5].x)
				{
					// Check the right wall
					if (CheckWallCollision(tunnelVertices[i - 2], tunnelVertices[i - 6]))
					{
						OnBallCollision();
						return;
					}
				}
				else
				{
					// Check the left wall
					if (CheckWallCollision(tunnelVertices[i - 1], tunnelVertices[i - 5]))
					{
						OnBallCollision();
						return;
					}
				}
			}

			// Check if the player has collided with any of the drops
			for (int i = 0; i < drops.Count; i++)
			{
				// If the distance between the player and the drop and greater than or equal to the size of both their collision sizes
				if (Vector2.Distance((Vector2)player.transform.position, (Vector2)drops[i].transform.position) <= player.CollisionSize + drops[i].CollisionSize)
				{
					// Increment the drops collected
					DropsCollected += dropCollectAmount;

					// Set it to de-active, this will return it ot the pool
					drops[i].gameObject.SetActive(false);

					// Remove it from the list of drops
					drops.RemoveAt(i);
					i--;
				}
			}
		}

		private bool CheckWallCollision(Vector2 wallVert1, Vector2 wallVert2)
		{
			wallVert1 += (Vector2)tunnelGameObject.transform.position;
			wallVert2 += (Vector2)tunnelGameObject.transform.position;

			Vector2 playerPos = (Vector2)player.transform.position;

			float debugRotate = 0;

			if (wallVert1.x > wallVert2.x)
			{
				wallVert1 = RotateVector2(wallVert1, -tunnelAngle);
				wallVert2 = RotateVector2(wallVert2, -tunnelAngle);
				playerPos = RotateVector2(playerPos, -tunnelAngle);

				debugRotate = tunnelAngle;

				Vector2 temp = wallVert1;
				wallVert1 = wallVert2;
				wallVert2 = temp;
			}
			else
			{
				wallVert1 = RotateVector2(wallVert1, tunnelAngle);
				wallVert2 = RotateVector2(wallVert2, tunnelAngle);
				playerPos = RotateVector2(playerPos, tunnelAngle);

				debugRotate = -tunnelAngle;
			}

			float left		= wallVert1.x;
			float right		= wallVert2.x;
			float bottom	= wallVert1.y;
			float top		= wallVert1.y + 100;

			float playerLeft	= playerPos.x - player.CollisionSize;
			float playerRight	= playerPos.x + player.CollisionSize;
			float playerBottom	= playerPos.y - player.CollisionSize;
			float playerTop		= playerPos.y + player.CollisionSize;
			
			// Only compile this code in the editor
			#if UNITY_EDITOR
			if (showCollisionBoxesInScene)
			{
				Debug.DrawLine(RotateVector3(new Vector3(left, top, tunnelGameObject.transform.position.z - 1f), debugRotate), RotateVector3(new Vector3(right, top, tunnelGameObject.transform.position.z - 1f), debugRotate), Color.green);
				Debug.DrawLine(RotateVector3(new Vector3(left, bottom, tunnelGameObject.transform.position.z - 1f), debugRotate), RotateVector3(new Vector3(right, bottom, tunnelGameObject.transform.position.z - 1f), debugRotate), Color.green);
				Debug.DrawLine(RotateVector3(new Vector3(left, top, tunnelGameObject.transform.position.z - 1f), debugRotate), RotateVector3(new Vector3(left, bottom, tunnelGameObject.transform.position.z - 1f), debugRotate), Color.green);
				Debug.DrawLine(RotateVector3(new Vector3(right, top, tunnelGameObject.transform.position.z - 1f), debugRotate), RotateVector3(new Vector3(right, bottom, tunnelGameObject.transform.position.z - 1f), debugRotate), Color.green);
				
				Debug.DrawLine(RotateVector3(new Vector3(playerLeft, playerTop, tunnelGameObject.transform.position.z - 1f), debugRotate), RotateVector3(new Vector3(playerRight, playerTop, tunnelGameObject.transform.position.z - 1f), debugRotate), Color.green);
				Debug.DrawLine(RotateVector3(new Vector3(playerLeft, playerBottom, tunnelGameObject.transform.position.z - 1f), debugRotate), RotateVector3(new Vector3(playerRight, playerBottom, tunnelGameObject.transform.position.z - 1f), debugRotate), Color.green);
				Debug.DrawLine(RotateVector3(new Vector3(playerLeft, playerTop, tunnelGameObject.transform.position.z - 1f), debugRotate), RotateVector3(new Vector3(playerLeft, playerBottom, tunnelGameObject.transform.position.z - 1f), debugRotate), Color.green);
				Debug.DrawLine(RotateVector3(new Vector3(playerRight, playerTop, tunnelGameObject.transform.position.z - 1f), debugRotate), RotateVector3(new Vector3(playerRight, playerBottom, tunnelGameObject.transform.position.z - 1f), debugRotate), Color.green);
			}
			#endif

			if (playerRight < left || playerLeft > right || playerBottom > top || playerTop < bottom)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Checks if any drop objects have moved off screen and if so returns them to the pool
		/// </summary>
		private void CheckDropsOffScreen()
		{
			for (int i = 0; i < drops.Count; i++)
			{
				if (drops[i].transform.position.y < -Utilities.WorldHeight(gameCamera) / 2f)
				{
					// Set it to de-active, this will return it ot the pool
					drops[i].gameObject.SetActive(false);
					
					// Remove it from the list of drops
					drops.RemoveAt(i);
					i--;
				}
			}
		}

		/// <summary>
		/// Called when the ball comes in contact with the tunnel mesh
		/// </summary>
		private void OnBallCollision()
		{
			// Check if the game state is playing, if it wasn't then just ignore the collision event
			if (gameState != GameState.Playing)
			{
				return;
			}

			TimesPlayed++;

			// Update the high score
			if (CurrentScore > HighScore)
			{
				HighScore = CurrentScore;
			}

			// Update the average score
			AverageScore = (AverageScore * (TimesPlayed - 1) + CurrentScore) / TimesPlayed;
			
			ChangeGameState(GameState.Over);
		}

		/// <summary>
		/// Creates the background mesh and places it in the scene.
		/// </summary>
		private void SetupBackground()
		{
			// Create the GameObject for the game mesh
			GameObject bkgObject = new GameObject("backgound_mesh");
			bkgObject.transform.SetParent(gameCamera.transform, false);
			bkgObject.transform.localPosition = new Vector3(0f, 0f, 20f);
			
			// Create and mesh
			Mesh bkgMesh = new Mesh();

			Vector3[]	vertices	= new Vector3[4];
			Vector2[]	uvs			= new Vector2[4];
			Vector3[]	normals		= new Vector3[4];
			int[]		triangles	= new int[6];

			vertices[0] = new Vector3(-Utilities.WorldWidth(gameCamera) / 2f, Utilities.WorldHeight(gameCamera) / 2f, 0f);
			vertices[1] = new Vector3(Utilities.WorldWidth(gameCamera) / 2f, Utilities.WorldHeight(gameCamera) / 2f, 0f);
			vertices[2] = new Vector3(-Utilities.WorldWidth(gameCamera) / 2f, -Utilities.WorldHeight(gameCamera) / 2f, 0f);
			vertices[3] = new Vector3(Utilities.WorldWidth(gameCamera) / 2f, -Utilities.WorldHeight(gameCamera) / 2f, 0f);

			float screenScale = Utilities.WorldWidth(gameCamera) / Utilities.WorldHeight(gameCamera);

			uvs[0] = new Vector2(0f, bkgRepeatSize);
			uvs[1] = new Vector2(bkgRepeatSize * screenScale, bkgRepeatSize);
			uvs[2] = new Vector2(0f, 0f);
			uvs[3] = new Vector2(bkgRepeatSize * screenScale, 0f);

			normals[0] = new Vector3(0, 0, -1);
			normals[1] = new Vector3(0, 0, -1);
			normals[2] = new Vector3(0, 0, -1);
			normals[3] = new Vector3(0, 0, -1);

			triangles[0] = 0;
			triangles[1] = 1;
			triangles[2] = 2;
			triangles[3] = 1;
			triangles[4] = 3;
			triangles[5] = 2;

			// Update the vertices, normals, and triangles of the mesh
			bkgMesh.vertices	= vertices;
			bkgMesh.uv			= uvs;
			bkgMesh.normals		= normals;
			bkgMesh.triangles	= triangles;
			
			// Create the required mesh components
			bkgObject.AddComponent<MeshFilter>().mesh		= bkgMesh;
			bkgObject.AddComponent<MeshRenderer>().material	= bkgMaterial;
		}
		
		/// <summary>
		/// Setups the mesh.
		/// </summary>
		private void SetupMesh()
		{
			Vector2 rightVert1	= rightDirection * (MaxTunnelSizeInPixels / 2f);					// top left corner
			Vector2 rightVert2	= rightVert1 - leftDirection * OffScreenVertDistance;				// bottom right corner
			Vector2 rightVert3	= new Vector2(rightVert2.x + OffScreenVertDistance, rightVert1.y);	// top right corner

			Vector2 leftVert1	= -rightDirection * (MaxTunnelSizeInPixels / 2f);		// top right
			Vector2 leftVert2	= leftVert1 - leftDirection * OffScreenVertDistance;;	// bottom right
			Vector2 leftVert3	= new Vector2(-OffScreenVertDistance, leftVert1.y);		// top left
			Vector2 leftVert4	= new Vector2(leftVert3.x, leftVert2.y);				// bottom left

			uvStartPoint = alignTextureWithTunnel ? rightVert1 : Vector2.zero;

			AddVertice(leftVert4);
			AddVertice(leftVert2);
			AddVertice(rightVert2);
			AddVertice(0f, 0f);		// Empty vertex so the indices of the List are correct
			AddVertice(leftVert3);
			AddVertice(leftVert1);
			AddVertice(rightVert1);
			AddVertice(rightVert3);

			if (textureFillsTunnel)
			{
				AddTriangle(6, 2, 5);
				AddTriangle(5, 2, 1);
			}
			else
			{
				AddTriangle(0, 0, 0);
				AddTriangle(4, 5, 0);
				AddTriangle(5, 1, 0);
				AddTriangle(6, 7, 2);
			}
			
			// We started the tunnel using the max tunnel size (need to add 2 since the alogrithm needs
			// to check the previous tunnel size which will normally be 2 indexes from the end of the list)
			tunnelSizes.Add(0);
			tunnelSizes.Add(MaxTunnelSizeInPixels);

			// Now add a tunnel which will create the rest of the starting tunnel
			AddTunnel(Mathf.Ceil(Utilities.WorldWidth(gameCamera) / unitSize) * unitSize, false);
			
			// Create the GameObject for the game mesh
			tunnelGameObject = new GameObject("mesh");
			tunnelGameObject.transform.position = new Vector3(0f, 0f);
			
			// Create and mesh
			tunnelMesh = new Mesh();
			RefreshMesh();
			
			// Create the required mesh components
			tunnelGameObject.AddComponent<MeshFilter>().mesh = tunnelMesh;
			tunnelGameObject.AddComponent<MeshRenderer>().material = tunnelMaterial;
		}
		
		public void AddTunnel(float nextTunnelLength = 0f, bool canAddDrop = true)
		{
			Vector2 lastPoint1		= tunnelVertices[tunnelVertices.Count - (tunnelDirection == Direction.Left ? 3 : 2)];
			Vector2 lastPoint2		= tunnelVertices[tunnelVertices.Count - (tunnelDirection == Direction.Left ? 2 : 3)];
			float	lastTunnelSize	= tunnelSizes[tunnelSizes.Count - 2];

			// Cacluate the new tunnels size
			float nextTunnelSize = Random.Range((int)minTunnelSizeInUnits, (int)(maxTunnelSizeInUnits + 1)) * unitSize;

			// If the passed in tunnel length is less than the min tunnel length then get a random tunnel lenght using the min/max
			if (nextTunnelLength <= MinTunnelLengthInPixels)
			{
				nextTunnelLength = Random.Range((int)minTunnelLengthInUnits, (int)(maxTunnelLengthInUnits + 1)) * unitSize;
			}

			// Get the positions of the next 2 points
			Vector2 nextPoint1 = lastPoint1 + (tunnelDirection == Direction.Left ? leftDirection : rightDirection) * (nextTunnelLength + nextTunnelSize);
			Vector2 nextPoint2 = lastPoint2 + (tunnelDirection == Direction.Left ? leftDirection : rightDirection) * (lastTunnelSize + nextTunnelLength);

			// If we are going right we need to swap the nextPoint1 and nextPoint2 so when we make calls to AddVertices they are in the correct order
			if (tunnelDirection != Direction.Left)
			{
				Vector2 temp = nextPoint1;
				nextPoint1 = nextPoint2;
				nextPoint2 = temp;
			}

			// Add the new vertices to the list of vertices
			AddVertice(nextPoint1.x - OffScreenVertDistance, nextPoint1.y);
			AddVertice(nextPoint1);
			AddVertice(nextPoint2);
			AddVertice(nextPoint1.x + OffScreenVertDistance, nextPoint2.y);

			// Add the triangles for the mesh to draw
			int vIndex = tunnelVertices.Count - 8;

			if (textureFillsTunnel)
			{
				AddTriangle(vIndex + 1, vIndex + 5, vIndex + 6);
				AddTriangle(vIndex + 1, vIndex + 6, vIndex + 2);
			}
			else
			{
				AddTriangle(vIndex + 0, vIndex + 5, vIndex + 1);
				AddTriangle(vIndex + 0, vIndex + 4, vIndex + 5);
				AddTriangle(vIndex + 2, vIndex + 6, vIndex + 7);
				AddTriangle(vIndex + 2, vIndex + 7, vIndex + 3);
			}

			// Create and place a drop
			if (canAddDrop && Random.Range(0.01f, 1f) <= dropChance)
			{
				Vector2 topLeft		= nextPoint1;
				Vector2 topRight	= nextPoint2;
				Vector2 bottomLeft	= tunnelVertices[tunnelVertices.Count - 7];
				Vector2 bottomRight	= tunnelVertices[tunnelVertices.Count - 6];
				
				topLeft		+= (Vector2)tunnelGameObject.transform.position;
				topRight	+= (Vector2)tunnelGameObject.transform.position;
				bottomLeft	+= (Vector2)tunnelGameObject.transform.position;
				bottomRight	+= (Vector2)tunnelGameObject.transform.position;

				float angleToRotate = (topLeft.x < bottomLeft.x) ? -tunnelAngle : tunnelAngle;

				topLeft		= RotateVector2(topLeft, angleToRotate);
				topRight	= RotateVector2(topRight, angleToRotate);
				bottomLeft	= RotateVector2(bottomLeft, angleToRotate);
				bottomRight	= RotateVector2(bottomRight, angleToRotate);

				Drop dropObject = dropObjectPool.GetObject().GetComponent<Drop>();

				float	randomX			= Random.Range(topLeft.x + dropObject.CollisionSize / 2f + dropEdgePadding, topRight.x - dropObject.CollisionSize / 2f - dropEdgePadding);
				float	randomY			= Random.Range(Mathf.Min(topLeft.y, topRight.y), Mathf.Max(bottomLeft.y, bottomRight.y));
				Vector2	dropPosition	= RotateVector2(new Vector2(randomX, randomY), -angleToRotate);

				dropObject.transform.SetParent(tunnelGameObject.transform, false);
				dropObject.transform.position = dropPosition;
				dropObject.gameObject.SetActive(true);

				drops.Add(dropObject);
			}

			// Switch the tunnel dirction
			tunnelDirection = (tunnelDirection == Direction.Left) ? Direction.Right : Direction.Left;
			tunnelSizes.Add(nextTunnelSize);
		}

		private void AddVertice(Vector2 vert)
		{
			AddVertice(vert.x, vert.y);
		}
		
		private void AddVertice(float x, float y)
		{
			// Add the vertice and normal
			tunnelVertices.Add(new Vector3(x, y, 0f));
			tunnelNormals.Add(new Vector3(0f, 0f, -1f));

			// Rotate the uvs so the texture runs parallel to the walls of the tunnel
			tunnelUVs.Add(RotateVector2(new Vector2((x + uvStartPoint.x) / textureSize, (y + uvStartPoint.y) / textureSize), tunnelAngle));
		}
		
		private void AddTriangle(int v1, int v2, int v3)
		{
			tunnelTriangles.Add(v1);
			tunnelTriangles.Add(v2);
			tunnelTriangles.Add(v3);
		}
		
		public void RefreshMesh()
		{
			// Clear the mesh
			tunnelMesh.Clear();

			// Update the vertices, normals, and triangles of the mesh
			tunnelMesh.vertices		= tunnelVertices.ToArray();
			tunnelMesh.uv			= tunnelUVs.ToArray();
			tunnelMesh.normals		= tunnelNormals.ToArray();
			tunnelMesh.triangles	= tunnelTriangles.ToArray();

			// Need to call RecalculateBounds so the mesh doesn't disappear after a while
			tunnelMesh.RecalculateBounds();
		}

		private void CreatePlayer()
		{
			Vector3 playerPosition = Vector3.zero;

			// If there is already a player then save it position and destroy it
			if (player != null)
			{
				playerPosition = player.transform.position;
				Destroy(player.gameObject);
			}

			// Create a new player using the saved index
			player = Instantiate(PlayerInfos[CurrentPlayerIndex].playerPrefab);
			player.transform.position = playerPosition;
		}

		private void SetSavedUnlockedPlayerInfos()
		{
			if (string.IsNullOrEmpty(UnlockedPlayerInfos))
			{
				for (int i = 0; i < playerInfos.Length; i++)
				{
					if (playerInfos[i].locked)
					{
						continue;
					}

					if (i != 0)
					{
						UnlockedPlayerInfos += ";";
					}

					UnlockedPlayerInfos += i.ToString();
				}

				return;
			}

			string[] unlockedPlayerInfos = UnlockedPlayerInfos.Split(';');

			for (int i = 0; i < unlockedPlayerInfos.Length; i++)
			{
				int index = System.Convert.ToInt32(unlockedPlayerInfos[i]);

				if (index < playerInfos.Length)
				{
					playerInfos[index].locked = false;
				}
			}
		}

		private Vector3 RotateVector3(Vector3 v, float angle)
		{
			Vector2 vect2 = RotateVector2((Vector2)v, angle);
			return new Vector3(vect2.x, vect2.y, v.z);
		}

		private Vector2 RotateVector2(Vector2 v, float angle)
		{
			float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
			float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
			
			float tx = v.x;
			float ty = v.y;

			v.x = (cos * tx) - (sin * ty);
			v.y = (sin * tx) + (cos * ty);

			return v;
		}
		
		#endregion
	}
}
