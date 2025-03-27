using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro; // Required for TextMeshProUGUI
using System.Collections;
using System.Collections.Generic;

public class RoomList : MonoBehaviourPunCallbacks
{
    public static RoomList Instance; // Singleton instance for easy access
    public GameObject roomManagerGameobject;
    public RoomManager roomManager;
    public Transform roomListParent; // Parent object to hold room list items
    public GameObject roomListItemPrefab; // Prefab for a room list item

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    public void ChangeRoomToCreateName(string roomName) {
        roomManager.roomNameToJoin = roomName;
    }

    private void Awake()
    {
        // Ensure only one instance of RoomList exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated");

        foreach (var room in roomList)
        {
            int index = cachedRoomList.FindIndex(r => r.Name == room.Name);

            if (room.RemovedFromList)
            {
                // Remove room if it has been removed from the server
                if (index != -1)
                {
                    cachedRoomList.RemoveAt(index);
                }
            }
            else
            {
                if (index == -1)
                {
                    // Add new room
                    cachedRoomList.Add(room);
                }
                else
                {
                    // Update existing room
                    cachedRoomList[index] = room;
                }
            }
        }

        // Update the UI after modifying the cached room list
        UpdateUI();
    }

    void UpdateUI()
    {
        // Clear existing room list items
        foreach (Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }

        // Populate the UI with updated room list
        foreach (var room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);

            // Set room details in the UI
            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{room.PlayerCount}/16";

            // Assign room name to the button
            RoomItemButton roomItemButton = roomItem.GetComponent<RoomItemButton>();
            if (roomItemButton != null)
            {
                roomItemButton.RoomName = room.Name;
            }
            else
            {
                Debug.LogError("RoomItemPrefab is missing the RoomItemButton component.");
            }
        }
    }

    public void JoinRoomByName(string name)
    {
        roomManager.roomNameToJoin = name;
        roomManagerGameobject.SetActive(true);
        gameObject.SetActive(false);
    }
}