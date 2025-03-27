using UnityEngine;

public class RoomItemButton : MonoBehaviour
{
    public string RoomName; // The name of the room this button represents

    public void OnButtonPressed()
    {
        if (RoomList.Instance != null) // Use RoomList.Instance (uppercase 'I')
        {
            RoomList.Instance.JoinRoomByName(RoomName);
        }
        else
        {
            Debug.LogError("RoomList.Instance is not set or is null!");
        }
    }
}