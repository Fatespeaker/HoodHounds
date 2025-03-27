using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab; // The current player prefab
    public Transform[] spawnPoints;

    public GameObject roomCam;
    public static RoomManager instance;
    public GameObject nameUI;
    public GameObject connectingUI;

    private string nickname = "Diggle";
    public string roomNameToJoin = "test";

    void Awake()
    {
        // Singleton pattern to ensure one instance of RoomManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeNickname(string name)
    {
        nickname = name;
    }

    public void ChangePlayerPrefab(string prefabName)
    {
        // Load the new prefab from the Resources folder
        GameObject loadedPrefab = Resources.Load<GameObject>(prefabName);
        if (loadedPrefab != null)
        {
            playerPrefab = loadedPrefab;
            Debug.Log($"Player prefab changed to: {prefabName}");
        }
        else
        {
            Debug.LogError($"Failed to load prefab: {prefabName}. Ensure it exists in the Resources folder.");
        }
    }

    public void JoinRoomButtonPressed()
    {
        Debug.Log("Connecting...");
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);
        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room Successfully");
        roomCam.SetActive(false);

        if (playerPrefab != null && spawnPoints.Length > 0)
        {
            // Select a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate the player object
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
            Debug.Log($"Player instantiated at {spawnPoint.position}: {player.name}");

            // Set the player's nickname
            PhotonView playerPhotonView = player.GetComponent<PhotonView>();
            if (playerPhotonView != null)
            {
                playerPhotonView.RPC("SetNickname", RpcTarget.AllBuffered, nickname);
            }
            else
            {
                Debug.LogError("Player prefab does not have a PhotonView component!");
            }
        }
        else
        {
            Debug.LogError("Player prefab not assigned or no spawn points available!");
        }
    }

    public void RespawnPlayer()
    {
        if (playerPrefab != null && spawnPoints.Length > 0)
        {
            // Select a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate the player object
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
            Debug.Log($"Player respawned at {spawnPoint.position}: {player.name}");

            // Set the player's nickname
            PhotonView playerPhotonView = player.GetComponent<PhotonView>();
            if (playerPhotonView != null)
            {
                playerPhotonView.RPC("SetNickname", RpcTarget.AllBuffered, nickname);
                PhotonNetwork.LocalPlayer.NickName = nickname;
            }
            else
            {
                Debug.LogError("Player prefab does not have a PhotonView component!");
            }
        }
        else
        {
            Debug.LogError("Player prefab not assigned or no spawn points available!");
        }
    }
}