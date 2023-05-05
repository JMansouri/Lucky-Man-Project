using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Util;
using Sfs2X.Entities.Data;

/**
 * Script attached to the Controller object in the Login scene.
 */
public class LoginSceneController : BaseSceneController
{
    //----------------------------------------------------------
    // Editor public properties
    //----------------------------------------------------------

    [Tooltip("IP address or domain name of the SmartFoxServer instance")]
    public string host = "127.0.0.1";

    [Tooltip("TCP listening port of the SmartFoxServer instance, used for TCP socket connection in all builds except WebGL")]
    public int tcpPort = 9933;

    [Tooltip("HTTP listening port of the SmartFoxServer instance, used for WebSocket (WS) connections in WebGL build")]
    public int httpPort = 8080;

    [Tooltip("Name of the SmartFoxServer Zone to join")]
    public string zone = "LuckyMan";

    [Tooltip("Display SmartFoxServer client debug messages")]
    public bool debug = false;

    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------

    public TMP_InputField nameInput;
    public TMP_InputField passInput;
    public TMP_InputField signupNameInput;
    public TMP_InputField signupPassInput;
    public Button loginButton;
    public Button showSignupButton;
    public Text loginInfo;
    public BasePanel signupPanel;
    public TextMeshProUGUI signupInfo;

    //----------------------------------------------------------
    // Private properties
    //----------------------------------------------------------

    private SmartFox _sfs;
    private bool _isSignup = false;

    //----------------------------------------------------------
    // Unity calback methods
    //----------------------------------------------------------

    private void Start()
    {
        // Focus on username input
        nameInput.Select();
        nameInput.ActivateInputField();

        // Show connection lost message, in case the disconnection occurred in another scene
        string connLostMsg = gm.ConnectionLostMessage;
        if (connLostMsg != null)
            loginInfo.text = connLostMsg;
    }

    //----------------------------------------------------------
    // UI event listeners
    //----------------------------------------------------------
    #region
    /**
	 * On username input edit end, if the Enter key was pressed, connect to SmartFoxServer.
	 */
    public void OnNameInputEndEdit()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            Connect();
    }

    /**
	 * On Login button click, connect to SmartFoxServer.
	 */
    public void OnLoginButtonClick()
    {
        Connect();
    }

    public void OnShowSignupButtonClick()
    {
        // show signup panel
        signupPanel.Show();
    }

    public void OnSignupButtonClick()
    {
        _isSignup = true;
        Connect();
    }
    #endregion

    //----------------------------------------------------------
    // Helper methods
    //----------------------------------------------------------
    #region
    /**
	 * Enable/disable username input interaction.
	 */
    private void EnableUI(bool enable)
    {
        nameInput.interactable = enable;
        passInput.interactable = enable;
        loginButton.interactable = enable;
        showSignupButton.interactable = enable;
        signupNameInput.interactable = enable;
        signupPassInput.interactable = enable;
    }

    /**
	 * Connect to SmartFoxServer.
	 */
    private void Connect()
    {
        // Disable user interface
        EnableUI(false);

        // Clear any previour error message
        loginInfo.text = "";

        // Set connection parameters
        ConfigData cfg = new ConfigData();

#if UNITY_EDITOR
        cfg.Host = host;
#endif
#if UNITY_ANDROID
        cfg.Host = "192.168.1.2";
#endif

        cfg.Port = tcpPort;
        cfg.Zone = zone;
        cfg.Debug = debug;

#if UNITY_WEBGL
		cfg.Port = httpPort;
#endif

        // Initialize SmartFox client
        // The singleton class GlobalManager holds a reference to the SmartFox class instance,
        // so that it can be shared among all the scenes
#if !UNITY_WEBGL
        _sfs = gm.CreateSfsClient();
#else
		sfs = gm.CreateSfsClient(UseWebSocket.WS_BIN);
#endif

        // Configure SmartFox internal logger
        _sfs.Logger.EnableConsoleTrace = debug;

        // Add event listeners
        AddSmartFoxListeners();

        // Connect to SmartFoxServer
        _sfs.Connect(cfg);
    }

    /**
	 * Add all SmartFoxServer-related event listeners required by the scene.
	 */
    private void AddSmartFoxListeners()
    {
        _sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        _sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        _sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        _sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
    }

    /**
	 * Remove all SmartFoxServer-related event listeners added by the scene.
	 * This method is called by the parent BaseSceneController.OnDestroy method when the scene is destroyed.
	 */
    override protected void RemoveSmartFoxListeners()
    {
        // NOTE
        // If this scene is stopped before a connection is established, the SmartFox client instance
        // could still be null, causing an error when trying to remove its listeners

        if (_sfs != null)
        {
            _sfs.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
            _sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            _sfs.RemoveEventListener(SFSEvent.LOGIN, OnLogin);
            _sfs.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        }
    }

    /**
	 * Hide all modal panels.
	 */
    override protected void HideModals()
    {
        // No modals used by this scene
    }
    #endregion

    //----------------------------------------------------------
    // SmartFoxServer event listeners
    //----------------------------------------------------------
    #region
    private void OnConnection(BaseEvent evt)
    {
        // Check if the conenction was established or not
        if ((bool)evt.Params["success"])
        {
            Debug.Log("SFS2X API version: " + _sfs.Version);
            Debug.Log("Connection mode is: " + _sfs.ConnectionMode);

            ISFSObject parameters = SFSObject.NewInstance();
            parameters.PutBool("sign_up", _isSignup);

            if (!_isSignup)
            {
                // Login
                _sfs.Send(new LoginRequest
                    (nameInput.text, passInput.text, zone, parameters));
            }
            else
            {
                // Signup
                parameters.PutUtfString("password", signupPassInput.text);
                _sfs.Send(new LoginRequest
                    (signupNameInput.text, signupPassInput.text, zone, parameters));
            }
        }
        else
        {
            // Show error message
            loginInfo.text = "Connection failed; is the server running at all?";

            // Enable user interface
            EnableUI(true);
        }
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        _isSignup = false;
        // Remove SFS listeners
        RemoveSmartFoxListeners();

        // Show error message
        string reason = (string)evt.Params["reason"];

        if (reason != ClientDisconnectionReason.MANUAL)
            loginInfo.text = "Connection lost; reason is: " + reason;

        // Enable user interface
        EnableUI(true);
    }

    private void OnLogin(BaseEvent evt)
    {
        if (_isSignup)
        {
            _sfs.Disconnect();
            _isSignup = false;
            signupPanel.Hide();
            loginInfo.text = "Successful Sign Up! Now Login with your account";
        }
        else
        {
            // Load lobby scene
            SceneManager.LoadScene("Lobby");
        }
    }

    private void OnLoginError(BaseEvent evt)
    {
        _isSignup = false;
        // Disconnect
        // NOTE: this causes a CONNECTION_LOST event with reason "manual", which in turn removes all SFS listeners
        _sfs.Disconnect();

        // Show error message
        loginInfo.text = "Login failed due to the following error:\n" + (string)evt.Params["errorMessage"];

        // Enable user interface
        EnableUI(true);
    }
    #endregion
}
