using UnityEngine;
using Photon.Pun;

public class PlayerPhotonSoundManager : MonoBehaviour
{
    public AudioSource gunShootSource;
    public AudioClip[] allGunShootSFX;

    // Public method to trigger the RPC
    public void PlayShootSFX(int index)
    {
        // Call the RPC on all clients
        GetComponent<PhotonView>().RPC("PlayShootSFX_RPC", RpcTarget.All, index);
    }

    [PunRPC]
    public void PlayShootSFX_RPC(int index)
    {
        if (index < 0 || index >= allGunShootSFX.Length)
        {
            Debug.LogError("Invalid sound index: " + index);
            return;
        }

        // Set up and play the audio
        gunShootSource.clip = allGunShootSFX[index];
        gunShootSource.pitch = Random.Range(0.7f, 1.2f);
        gunShootSource.volume = Random.Range(0.2f, 0.35f);
        gunShootSource.Play();
    }
}