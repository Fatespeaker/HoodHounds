using UnityEngine;
using Photon.Pun;

public class AudioPlayer : MonoBehaviourPun
{
    public AudioSource audioSource; // Assign this in the Inspector
    public AudioClip[] audioClips;  // Assign your list of audio clips in the Inspector

    private void OnEnable()
    {
        if (!audioSource || audioClips == null || audioClips.Length == 0)
        {
            Debug.LogError("AudioSource is not assigned or the AudioClip array is empty!");
            return;
        }

        // Select a random clip
        AudioClip randomClip = GetRandomClip();

        // Play audio locally
        PlayAudio(randomClip);

        // Synchronize audio playback for all players in the lobby
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_PlayAudio", RpcTarget.OthersBuffered, randomClip.name);
        }
    }

    private AudioClip GetRandomClip()
    {
        int randomIndex = Random.Range(0, audioClips.Length);
        return audioClips[randomIndex];
    }

    private void PlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.spatialBlend = 1.0f; // Ensure it's 3D audio if needed
        audioSource.Play();
    }

    [PunRPC]
    private void RPC_PlayAudio(string clipName)
    {
        // Find the clip by name
        AudioClip clipToPlay = System.Array.Find(audioClips, clip => clip.name == clipName);
        if (clipToPlay != null)
        {
            PlayAudio(clipToPlay);
        }
        else
        {
            Debug.LogError($"AudioClip with name {clipName} not found in the list!");
        }
    }
}