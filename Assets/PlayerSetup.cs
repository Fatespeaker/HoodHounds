using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    public Movement movement;
    public GameObject camera;
    public string nickname;
    public TextMeshPro nicknameText;
    public Transform TPweaponHolder;

    void Start()
    {
        // Enable components only for the local player
        if (photonView.IsMine)
        {
            movement.enabled = true;
            camera.SetActive(true);
            TPweaponHolder.gameObject.SetActive(false);
        }
        else
        {
            movement.enabled = false;
            camera.SetActive(false);
        }
    }

    [PunRPC]
    public void SetTPWeapon(int weaponIndex)
    {
        // Ensure weapon index is within bounds
        if (weaponIndex < 0 || weaponIndex >= TPweaponHolder.childCount)
        {
            Debug.LogWarning("Weapon index out of bounds!");
            return;
        }

        // Disable all weapons
        foreach (Transform weapon in TPweaponHolder)
        {
            weapon.gameObject.SetActive(false); // Fixed: "GameObject" to "gameObject"
        }

        // Enable the selected weapon
        TPweaponHolder.GetChild(weaponIndex).gameObject.SetActive(true); // Fixed: Correct method chaining
    }

    [PunRPC]
    public void SetNickname(string name)
    {
        Debug.Log($"Setting nickname to '{name}' for player {photonView.Owner.NickName}");
        nickname = name;

        // Update nickname text if the component exists
        if (nicknameText != null)
        {
            nicknameText.text = nickname;
        }
        else
        {
            Debug.LogError("NicknameText is not assigned!");
        }
    }
}
