using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviour
{
    public int damage = 10;
    public float fireRate = 1f;
    public Camera camera; // Assign the player's camera in the Inspector
    private float nextFire;
    public int shootSFXIndex = 0;
    public PlayerPhotonSoundManager playerPhotonSoundManager;
    public GameObject hitVFX;

    void Update()
    {
        if (nextFire > 0)
        {
            nextFire -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && nextFire <= 0)
        {
            nextFire = 1 / fireRate;
            Fire();
        }
    }

    void Fire()
    {
        playerPhotonSoundManager.PlayShootSFX(shootSFXIndex);
        if (camera == null)
        {
            Debug.LogError("Weapon: Camera is not assigned!");
            return;
        }

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);
            PhotonView targetPhotonView = hit.transform.gameObject.GetComponent<PhotonView>();
            Health targetHealth = hit.transform.gameObject.GetComponent<Health>();

            if (targetPhotonView != null && targetHealth != null)
            {
                // Call the TakeDamage RPC on all clients
                if (damage >= hit.transform.gameObject.GetComponent<Health>().health) {
                    
                }
                targetPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
    }
}