using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] FPS_Hands_ChildGameobjects;
    public GameObject[] Soldier_ChildGameobjects;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            //Activate FPS Hands
            foreach (GameObject gameObject in FPS_Hands_ChildGameobjects)
            {
                gameObject.SetActive(true);
            }
            //Deactivate Soldier
            foreach (GameObject gameObject in Soldier_ChildGameobjects)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            //Deactivate FPS Hands
            foreach (GameObject gameObject in FPS_Hands_ChildGameobjects)
            {
                gameObject.SetActive(false);
            }
            //Activate Soldier
            foreach (GameObject gameObject in Soldier_ChildGameobjects)
            {
                gameObject.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
