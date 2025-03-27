using UnityEngine;
using System.Linq;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;

public class Leaderboard : MonoBehaviour
{
    public GameObject playersHolder;         // The UI container for the leaderboard
    public float refreshRate = 1f;          // Refresh rate for leaderboard updates
    public GameObject[] slots;              // Array of slot GameObjects
    public TextMeshProUGUI[] scoreTexts;    // Score text objects
    public TextMeshProUGUI[] nameTexts;     // Name text objects

    private void Start()
    {
        // Periodically refresh the leaderboard
        InvokeRepeating(nameof(Refresh), 1f, refreshRate);
    }

    public void Refresh()
    {
        // Disable all leaderboard slots initially
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        // Get the sorted list of players based on their score
        var sortedPlayerList = PhotonNetwork.PlayerList
            .OrderByDescending(player => player.GetScore())
            .ToList();

        // Populate the leaderboard slots
        for (int i = 0; i < sortedPlayerList.Count && i < slots.Length; i++)
        {
            var player = sortedPlayerList[i];
            slots[i].SetActive(true);

            // Set player name and score
            string playerName = string.IsNullOrEmpty(player.NickName) ? "Diggle" : player.NickName;
            nameTexts[i].text = playerName;
            scoreTexts[i].text = player.GetScore().ToString();
        }
    }

    private void Update()
    {
        // Show/hide leaderboard UI when the Tab key is pressed
        playersHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }
}