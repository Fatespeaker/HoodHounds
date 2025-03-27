using UnityEngine;
using Photon.Pun; // Required for PhotonView and RPCs

public class WeaponSwitcher : MonoBehaviour
{
    public PhotonView playerSetupView; // Ensure this is properly assigned
    private int selectedWeapon = 0;

    void Start()
    {
        // Select the initial weapon
        SelectWeapon();
    }

    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        // Check for weapon selection inputs
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
        }

        // Switch weapons if the selected weapon has changed
        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        // Notify all players about the weapon change
        if (playerSetupView != null)
        {
            playerSetupView.RPC("SetTPWeapon", RpcTarget.All, selectedWeapon);
        }
        else
        {
            Debug.LogError("PhotonView is not assigned to WeaponSwitcher!");
        }

        // Enable the selected weapon and disable others
        int i = 0;
        foreach (Transform weapon in transform)
        {
            weapon.gameObject.SetActive(i == selectedWeapon);
            i++;
        }
    }
}