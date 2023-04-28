using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;

/**
 * Script attached to the Controller object in the Lobby scene.
 */
public class LobbySceneController : BaseSceneController
{
	public static string GAME_ROOMS_GROUP_NAME = "games";

	//----------------------------------------------------------
	// UI elements
	//----------------------------------------------------------

	public Text loggedInAsLabel;
	public UserProfilePanel userProfilePanel;
	public WarningPanel warningPanel;	

	//----------------------------------------------------------
	// Private properties
	//----------------------------------------------------------

	private SmartFox sfs;
	private Dictionary<int, GameListItem> gameListItems;

	//----------------------------------------------------------
	// Unity calback methods
	//----------------------------------------------------------

	private void Start()
	{
		// Set a reference to the SmartFox client instance
		sfs = gm.GetSfsClient();

		// Hide modal panels
		HideModals();

		// Display username in footer and user profile panel
		//loggedInAsLabel.text = "Logged in as <b>" + sfs.MySelf.Name + "</b>";
		//userProfilePanel.InitUserProfile(sfs.MySelf.Name);

		// Add event listeners
		AddSmartFoxListeners();
	}

	//----------------------------------------------------------
	// UI event listeners
	//----------------------------------------------------------
	#region
	/**
	 * On Logout button click, disconnect from SmartFoxServer.
	 * This causes the SmartFox listeners added by this scene to be removed (see BaseSceneController.OnDestroy method)
	 * and the Login scene to be loaded (see GlobalManager.OnConnectionLost method).
	 */
	public void OnLogoutButtonClick()
	{
		// Disconnect from SmartFoxServer
		sfs.Disconnect();
	}

	/**
	 * On Start game button click, create and join a new game Room.
	 */
	public void OnStartGameButtonClick()
	{
		Debug.Log("Joining game ...");
		// Configure Room
		RoomSettings settings = new RoomSettings(sfs.MySelf.Name + "'s game");
		settings.GroupId = GAME_ROOMS_GROUP_NAME;
		settings.IsGame = true;
		settings.MaxUsers = 2;
		settings.MaxSpectators = 0;

		// Request Room creation to server
		sfs.Send(new CreateRoomRequest(settings, true));

        SceneManager.LoadScene("Game");
    }

	/**
	 * On Play game button click in Game List Item prefab instance, join an existing game Room as a player.
	 */
	public void OnGameItemPlayClick(int roomId)
	{
		// Join game Room as player
		sfs.Send(new Sfs2X.Requests.JoinRoomRequest(roomId));
	}

	/**
	 * On Watch game button click in Game List Item prefab instance, join an existing game Room as a spectator.
	 */
	public void OnGameItemWatchClick(int roomId)
	{
		// Join game Room as spectator
		sfs.Send(new Sfs2X.Requests.JoinRoomRequest(roomId, null, null, true));
	}

	/**
	 * On User icon click, show User Profile Panel prefab instance.
	 */
	public void OnUserIconClick()
	{
		userProfilePanel.Show();
	}
	#endregion

	//----------------------------------------------------------
	// Helper methods
	//----------------------------------------------------------
	#region
	/**
	 * Add all SmartFoxServer-related event listeners required by the scene.
	 */
	private void AddSmartFoxListeners()
	{
		sfs.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError);
		sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
		sfs.AddEventListener(SFSEvent.ROOM_REMOVE, OnRoomRemoved);
		sfs.AddEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChanged);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
	}

	/**
	 * Remove all SmartFoxServer-related event listeners added by the scene.
	 * This method is called by the parent BaseSceneController.OnDestroy method when the scene is destroyed.
	 */
	override protected void RemoveSmartFoxListeners()
	{
		sfs.RemoveEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError);
		sfs.RemoveEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
		sfs.RemoveEventListener(SFSEvent.ROOM_REMOVE, OnRoomRemoved);
		sfs.RemoveEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChanged);
		sfs.RemoveEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		sfs.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
	}

	/**
	 * Hide all modal panels.
	 */
	override protected void HideModals()
	{
		//userProfilePanel.Hide();
		//warningPanel.Hide();
	}

	#endregion

	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------
	#region
	private void OnRoomCreationError(BaseEvent evt)
	{
		// Show Warning Panel prefab instance
		warningPanel.Show("Room creation failed: " + (string)evt.Params["errorMessage"]);
	}

	private void OnRoomAdded(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];
	}

	public void OnRoomRemoved(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];

		// Get reference to game list item corresponding to Room
		gameListItems.TryGetValue(room.Id, out GameListItem gameListItem);

		// Remove game list item
		if (gameListItem != null)
		{
			// Remove listeners
			gameListItem.playButton.onClick.RemoveAllListeners();

			// Remove game list item from dictionary
			gameListItems.Remove(room.Id);

			// Destroy game object
			GameObject.Destroy(gameListItem.gameObject);
		}
	}

	public void OnUserCountChanged(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];

		// Get reference to game list item corresponding to Room
		gameListItems.TryGetValue(room.Id, out GameListItem gameListItem);

		// Update game list item
		if (gameListItem != null)
			gameListItem.SetState(room);
	}

	private void OnRoomJoin(BaseEvent evt)
	{
		// Load game scene
		SceneManager.LoadScene("Game");
	}

	private void OnRoomJoinError(BaseEvent evt)
	{
		// Show Warning Panel prefab instance
		warningPanel.Show("Room join failed: " + (string)evt.Params["errorMessage"]);
	}
	#endregion
}
