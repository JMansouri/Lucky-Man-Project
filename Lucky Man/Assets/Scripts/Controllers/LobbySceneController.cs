using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Requests.Game;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Managers;
using TMPro;

/**
 * Script attached to the Controller object in the Lobby scene.
 */
public class LobbySceneController : BaseSceneController
{
    public static string GAME_ROOMS_GROUP_NAME = "games";

    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------

    public TextMeshProUGUI profileText;
    public Text loggedInAsLabel;
    public UserProfilePanel userProfilePanel;
    public WarningPanel warningPanel;

    //----------------------------------------------------------
    // Private properties
    //----------------------------------------------------------

    private SmartFox sfs;
    private bool searchForMatch = false;

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

        // Load name and score of the player:
        LoadProfile();
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
        sfs.Send(new JoinRoomRequest("Lobby"));
        // deactive find match button        
    }

    public void OnLeaderBoardButtonClick()
    {
        ISFSObject parameters = SFSObject.NewInstance();
        sfs.Send(new ExtensionRequest("leader_board", parameters));
        // show leader board panel
        // wait for list of players
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
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
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
        sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
    }

    /**
	 * Hide all modal panels.
	 */
    override protected void HideModals()
    {
        //userProfilePanel.Hide();
        //warningPanel.Hide();
    }

    private void LoadProfile()
    {
        int points = sfs.MySelf.GetVariable("rank_points").GetIntValue();
        profileText.text = $"{sfs.MySelf.Name} عزیز _ امتیاز شما : {points}";
    }

    #endregion

    //----------------------------------------------------------
    // SmartFoxServer event listeners
    //----------------------------------------------------------
    #region
    private void OnRoomCreationError(BaseEvent evt)
    {
        Debug.Log("Room creation failed: " + (string)evt.Params["errorMessage"]);
        // Show Warning Panel prefab instance
        //warningPanel.Show("Room creation failed: " + (string)evt.Params["errorMessage"]);
    }

    private void OnRoomAdded(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        Debug.Log($"{room.Name} room created.");
    }

    public void OnRoomRemoved(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
    }

    public void OnUserCountChanged(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        Debug.Log($"User count changed in {room.Name}! current number : {room.UserCount}");
    }

    private void OnRoomJoin(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        Debug.Log($"Joined {room.Name} room");

        if (room.UserCount == 1)
        {
            Debug.Log($"Only 1 player in {room.Name}, waiting for another player.");
            searchForMatch = true;
        }
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = (string)evt.Params["cmd"];
        if (cmd.Equals("found_match"))
        {
            ISFSObject responseParams = (SFSObject)evt.Params["params"];
            int id = responseParams.GetInt("room_id");
            Debug.Log(" Room ID : " + id + " ____ Name : " + sfs.GetRoomById(id));
            SceneManager.LoadScene("Game");
        }
        else if (cmd.Equals("leader_board"))
        {
            ISFSObject responseParams = (SFSObject)evt.Params["params"];
            ISFSArray players = responseParams.GetSFSArray("board");

            string s = "PEOPLE LIST RECEIVED:\n\n";

            for (int i= 0; i < players.Count; i++)
            {
                ISFSObject player = players.GetSFSObject(i);
                s += " > " + (i+1) +". "+ player.GetUtfString("name") +
                    ", " + player.GetInt("points") + "Point \n";
            }

            Debug.Log(s);
        }
    }

    private void JoinOrCreateGameRoom()
    {
        // Configure Room
        string roomName = sfs.MySelf.Name + "'s game";
        SFSGameSettings settings = new SFSGameSettings(roomName);
        settings.GroupId = GAME_ROOMS_GROUP_NAME;
        settings.MaxUsers = 2;
        settings.MaxSpectators = 0;
        settings.MinPlayersToStartGame = 2;
        settings.IsPublic = true;
        settings.LeaveLastJoinedRoom = true;
        settings.NotifyGameStarted = false;
        settings.IsGame = true;
        // Join a game room
        sfs.Send(new QuickJoinOrCreateRoomRequest(null,
            new List<string>() { GAME_ROOMS_GROUP_NAME }, settings));
        searchForMatch = false;
        //sfs.Send(new QuickJoinGameRequest(null,
        //new List<string>() { GAME_ROOMS_GROUP_NAME }, sfs.LastJoinedRoom));
    }

    private void OnRoomJoinError(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        Debug.Log($"Sorry ,Couldn't join to room {room.Name}");
    }

    private void OnPublicMessage(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        User sender = (User)evt.Params["sender"];
        string message = (string)evt.Params["message"];
        Debug.Log($"Public message in {room.Name} room:");
        Debug.Log($" {sender.Name} : {message}");
    }
    #endregion
}
