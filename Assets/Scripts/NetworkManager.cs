using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection status")]
    public Text connectionStatusText;

    [Header("Login UI Panel")]
    public InputField playerNameInput;
    public GameObject Login_UI_Panel;

    [Header("Game Options UI Panel")]
    public GameObject GameOptions_UI_Panel;

    [Header("Create room UI Panel")]
    public InputField roomNameInputField;
    public InputField maxPlayerInputField;
    public GameObject CreateRoom_UI_Panel;

    [Header("Inside Room UI Panel")]
    public GameObject InsideRoom_UI_Panel;
    public Text roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerListContent;
    public GameObject startGameButton;

    [Header("Room List UI Panel")]
    public GameObject RoomList_UI_Panel;
    public GameObject roomListEntryPrefab;
    public GameObject roomListParentGameobject;

    [Header("Join Random Room UI Panel")]
    public GameObject JoinRandomRoom_UI_Panel;
    public Text joinRandomRoomText;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListGameObject;
    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Methods
        
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(Login_UI_Panel.name);
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObject = new Dictionary<string, GameObject>();

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        connectionStatusText.text = "Connection status: "+ PhotonNetwork.NetworkClientState;
    }

    #endregion

    #region UI Callbacks
    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        if(!string.IsNullOrEmpty(playerName)){
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }else{
            Debug.Log("Playername is invalid");
        }
    }   

    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room "+ Random.Range(1000,10000);
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(maxPlayerInputField.text);
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptions_UI_Panel.name);
    }

    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanel(RoomList_UI_Panel.name);
    }

    public void OnBackButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanel(GameOptions_UI_Panel.name);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
        // ActivatePanel(GameOptions_UI_Panel.name);
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        ActivatePanel(JoinRandomRoom_UI_Panel.name);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    #endregion

    #region Photon Callbacks
    public override void OnConnected() 
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster() 
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ " is connected to photon");
        ActivatePanel(GameOptions_UI_Panel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name+ " is created.");
    }

    public override void OnJoinedRoom()
    {

        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to "+PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(InsideRoom_UI_Panel.name);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }

        roomInfoText.text = "Room name: "+PhotonNetwork.CurrentRoom.Name+ " "+
                            "Players/Max.players: "+
                            PhotonNetwork.CurrentRoom.PlayerCount+" / "+
                            PhotonNetwork.CurrentRoom.MaxPlayers;

        if (playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }
        // INSTANTIATE PLAYER LIST GAMEOBJECTS
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListGameObject = Instantiate(playerListPrefab);
            playerListGameObject.transform.SetParent(playerListContent.transform);
            playerListGameObject.transform.localScale = Vector3.one;

            playerListGameObject.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
            }else{
                playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
            }

            playerListGameObjects.Add(player.ActorNumber,playerListGameObject);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {       
        //Update room info text
        roomInfoText.text = "Room name: "+PhotonNetwork.CurrentRoom.Name+ " "+
                        "Players/Max.players: "+
                        PhotonNetwork.CurrentRoom.PlayerCount+" / "+
                        PhotonNetwork.CurrentRoom.MaxPlayers;

        GameObject playerListGameObject = Instantiate(playerListPrefab);
        playerListGameObject.transform.SetParent(playerListContent.transform);
        playerListGameObject.transform.localScale = Vector3.one;

        playerListGameObject.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;
        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
        }else{
            playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
        }

        playerListGameObjects.Add(newPlayer.ActorNumber,playerListGameObject);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Update room info text
        roomInfoText.text = "Room name: "+PhotonNetwork.CurrentRoom.Name+ " "+
                        "Players/Max.players: "+
                        PhotonNetwork.CurrentRoom.PlayerCount+" / "+
                        PhotonNetwork.CurrentRoom.MaxPlayers;

        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptions_UI_Panel.name);

        foreach (GameObject playerListGameObject in playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
        
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room);
            if(!room.IsOpen || !room.IsVisible || room.RemovedFromList){
                if(cachedRoomList.ContainsKey(room.Name)){
                    cachedRoomList.Remove(room.Name);
                }
            }
            else
            {
                if(cachedRoomList.ContainsKey(room.Name)){
                    cachedRoomList[room.Name] = room;
                }
                else
                {
                    cachedRoomList.Add(room.Name,room);
                }
            }
        }

        foreach (RoomInfo room in cachedRoomList.Values)
        {
            GameObject roomListEntryGameobject = Instantiate(roomListEntryPrefab);
            roomListEntryGameobject.transform.SetParent(roomListParentGameobject.transform);
            roomListEntryGameobject.transform.localScale = Vector3.one;

            roomListEntryGameobject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
            roomListEntryGameobject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount+ " / "+room.MaxPlayers;
            roomListEntryGameobject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(room.Name) );

            roomListGameObject.Add(room.Name, roomListEntryGameobject);
            
        }
    }
    
    public override void OnLeftLobby()
    {
        ClearRoomListView();
        cachedRoomList.Clear();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // joinRandomRoomText.text = message;
        Debug.Log(message);
        string roomName = "Room "+ Random.Range(1000,10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    #endregion

    #region Public Methods
    public void ActivatePanel(string panelToBeActivated)
    {
        Login_UI_Panel.SetActive(panelToBeActivated.Equals(Login_UI_Panel.name));
        GameOptions_UI_Panel.SetActive(panelToBeActivated.Equals(GameOptions_UI_Panel.name));
        CreateRoom_UI_Panel.SetActive(panelToBeActivated.Equals(CreateRoom_UI_Panel.name));
        InsideRoom_UI_Panel.SetActive(panelToBeActivated.Equals(InsideRoom_UI_Panel.name));
        RoomList_UI_Panel.SetActive(panelToBeActivated.Equals(RoomList_UI_Panel.name));
        JoinRandomRoom_UI_Panel.SetActive(panelToBeActivated.Equals(JoinRandomRoom_UI_Panel.name));
    }
    #endregion

    #region Private Methods
    void OnJoinRoomButtonClicked(string _roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(_roomName);
    }

    void ClearRoomListView()
    {
        foreach (var gameObject in roomListGameObject.Values)
        {
            Destroy(gameObject);
        }
        roomListGameObject.Clear();
    }
    #endregion
}
