using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection status")]
    public Text connectionStatusText;

    [Header("Login UI Panel")]
    public InputField playerNameInput;
    #region Unity Methods
        
    // Start is called before the first frame update
    void Start()
    {
        
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
    #endregion

    #region Photon Callbacks
    public override void OnConnected() 
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster() 
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ " is connected to photon");
    }
    #endregion
}
