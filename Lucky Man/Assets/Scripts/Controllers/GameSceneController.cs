using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using LuckyMan.Runtime;

/**
 * Script attached to the Controller object in the Game scene.
 */
public class GameSceneController : BaseSceneController
{
    //----------------------------------------------------------
    // Private properties
    //----------------------------------------------------------

    private SmartFox sfs;
    private bool runTimer;
    private float timer = 20;
    [SerializeField] private UIManager _uiManager;

    //----------------------------------------------------------
    // Unity callback methods
    //----------------------------------------------------------

    private void Start()
    {
        // Set a reference to the SmartFox client instance
        sfs = gm.GetSfsClient();

        // Hide additional and conditional panels
        HideModals();

        // Print system message
        PrintSystemMessage("Game joined as " + (sfs.MySelf.IsPlayer ? "player" : "spectator"));

        // Add event listeners
        AddSmartFoxListeners();

        // If user is the first player in the Room, set a timeout
        // Having a timeout is usefult to suggest the user to leave the game if not yet started within some time
        // For example this could mean that the invited buddy refused the invitation, or the server couldn't locate other players to invite
        if (sfs.MySelf.IsPlayer && sfs.LastJoinedRoom.PlayerList.Count == 1)
        {
            //runTimer = true;
        }

        SetupGame();
    }

    override protected void Update()
    {
        base.Update();

        // Check timeout
        if (runTimer)
        {
            timer -= Time.deltaTime;

            int timerInt = (int)Math.Round(timer, 0);

            if (timerInt == 0)
                StopTimeout(true);
        }
    }

    //----------------------------------------------------------
    // UI event listeners
    //----------------------------------------------------------
    #region

    private void OnEnable()
    {
        _uiManager.StartButton.onClick.AddListener(OnStartButtonClick);
    }

    private void OnDisable()
    {
        _uiManager.StartButton.onClick.RemoveListener(OnStartButtonClick);
    }

    public void OnStartButtonClick()
    {
        Debug.Log("Notify Server that player is ready");

        ISFSObject parameters = SFSObject.NewInstance();
        sfs.Send(new ExtensionRequest("ready", parameters, sfs.JoinedRooms[0]));

        // hide start panel:
        _uiManager.HideStartPanel();
    }

    /**
	 * On Leave button click, go back to Login scene.
	 */
    public void OnLeaveButtonClick()
    {
        // Leave current game room
        sfs.Send(new LeaveRoomRequest());

        // Return to lobby scene
        SceneManager.LoadScene("Lobby");
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
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
    }

    /**
	 * Remove all SmartFoxServer-related event listeners added by the scene.
	 * This method is called by the parent BaseSceneController.OnDestroy method when the scene is destroyed.
	 */
    override protected void RemoveSmartFoxListeners()
    {
        sfs.RemoveEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.RemoveEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
    }

    private void SetupGame()
    {
        // 1. get players names
        // 2. show start panel
        // 3. wait for start
    }

    /**
	 * Hide all modal panels.
	 */
    override protected void HideModals()
    {
        //leavePanel.Hide();
    }

    /**
	 * Disable timeout showing panel suggesting player to leave.
	 */
    private void StopTimeout(bool showPanel)
    {
        runTimer = false;

        //if (showPanel)
        //	leavePanel.Show();
        //else
        //	leavePanel.Hide();
    }

    /**
	 * Display a system message.
	 */
    private void PrintSystemMessage(string message)
    {
        //// Print message
        //chatTextArea.text += "<color=#ffffff><i>" + message + "</i></color>\n";

        //// Scroll view to bottom
        //ScrollChatToBottom();
    }

    #endregion

    //----------------------------------------------------------
    // SmartFoxServer event listeners
    //----------------------------------------------------------
    #region
    private void OnExtensionResponse(BaseEvent evt)
    {
        // Retrieve response object
        string cmd = (string)evt.Params["cmd"];
        Debug.Log($"--> Response command : {cmd}");

        if (cmd.Equals("start"))
        {
            Debug.Log("# Start playing !");
            // whose turn it is
            // enable dice
        }
        else if (cmd.Equals("update_turn"))
        {

        }
    }

    private void OnUserEnterRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];
        Room room = (Room)evt.Params["room"];

        // Display system message
        Debug.Log("User " + user.Name + " joined this game as " + (user.IsPlayerInRoom(room) ? "player" : "spectator"));

        // Stop timeout
        if (user.IsPlayerInRoom(room))
            StopTimeout(false);
    }

    private void OnUserExitRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];

        // Display system message
        if (user != sfs.MySelf)
            Debug.Log("User " + user.Name + " left the game");
    }
    #endregion
}
