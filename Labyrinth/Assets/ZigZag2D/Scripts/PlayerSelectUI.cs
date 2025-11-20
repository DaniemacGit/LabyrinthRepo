using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace TunnelGame
{
	public class PlayerSelectUI : MonoBehaviour
	{
		#region Inspector Variables

		[SerializeField] private Text					coinsAmountText;
		[SerializeField] private PlayerSelectListItem	itemPrefab;
		[SerializeField] private Transform				listContent;

		#endregion

		#region Member Variables

		private List<PlayerSelectListItem> listItems;

		#endregion

		#region Unity Methods

		private void OnEnable()
		{
			GameManager.PlayerInfo[] playerInfos = GameManager.Instance.PlayerInfos;

			// Create the PlayerSelectListItems if they haven't already been created
			if (listItems == null)
			{
				listItems = new List<PlayerSelectListItem>();

				for (int i = 0; i < playerInfos.Length; i++)
				{
					listItems.Add(Instantiate(itemPrefab));
					listItems[i].transform.SetParent(listContent, false);
				}
			}

			// Setup all the PlayerSelectListItem
			for (int i = 0; i < playerInfos.Length; i++)
			{
				listItems[i].Setup(i);
			}
		}

		private void Update()
		{
			// Set the total amount of coins the player has to spend
			coinsAmountText.text = GameManager.Instance.DropsCollected.ToString();
		}

		#endregion

		#region Public Methods

		public void BackClicked()
		{
			// Set it back to Over so the over screen shows
			GameManager.Instance.ChangeGameState(GameManager.GameState.Over);
		}

		#endregion
	}
}
