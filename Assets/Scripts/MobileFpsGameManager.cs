using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MobileFpsGameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            int randomPoint = Random.Range(-10,10);
            PhotonNetwork.Instantiate(playerPrefab.name,new Vector3(randomPoint,0f,randomPoint), Quaternion.identity);
        }
    }

}
