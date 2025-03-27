using UnityEngine;
using Photon.Pun;

public class EscPause : MonoBehaviour
{
     void Update()
    {        if (!PhotonNetwork.IsConnected || PhotonNetwork.LocalPlayer.IsLocal)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
               Application.Quit(); 
            }
        }
    }
}
