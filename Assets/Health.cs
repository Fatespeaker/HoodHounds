using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviourPun
{
    public int health = 100;
    public int damagePerSecond = 10; // Damage taken per second when on unsafe objects
    public TextMeshProUGUI healthText;

    private bool isOnUnsafeObject = false; // Tracks if the player is on an "unsafe" object
    private Coroutine damageCoroutine; // Reference to the damage coroutine
    public RectTransform healthBar;
    private float originalHealthBarSize;
    private void Start() {
        originalHealthBarSize = healthBar.sizeDelta.x;
    }
    

    [PunRPC]
    public void TakeDamage(int damage)
    {
        // Ensure only the owner modifies health
        if (!photonView.IsMine) return;

        float previousHealth = health;
    health -= damage;
    health = Mathf.Clamp(health, 0, 100); // Ensure health doesn't go below 0

    // Calculate the width reduction
    float newWidth = originalHealthBarSize * health / 100;
    float widthLost = originalHealthBarSize * (previousHealth - health) / 100;

    // Update the size of the health bar
    healthBar.sizeDelta = new Vector2(newWidth, healthBar.sizeDelta.y);

    // Move the health bar to the left by half the width lost
    healthBar.anchoredPosition -= new Vector2(widthLost / 2, 0);

    // Update the health text
    healthText.text = health.ToString();
        if (health <= 0)
        {
            RoomManager.instance.RespawnPlayer();
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        // Use PhotonNetwork to synchronize destruction
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player stepped on an "unsafe" object
        if (collision.gameObject.CompareTag("unsafe"))
        {
            isOnUnsafeObject = true;

            // Start the damage coroutine if it's not already running
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ApplyDamageOverTime());
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if the player left the "unsafe" object
        if (collision.gameObject.CompareTag("unsafe"))
        {
            isOnUnsafeObject = false;

            // Stop the damage coroutine
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator ApplyDamageOverTime()
    {
        while (isOnUnsafeObject)
        {
            TakeDamage(damagePerSecond);

            // Wait for 1 second before applying damage again
            yield return new WaitForSeconds(1f);
        }
    }
}