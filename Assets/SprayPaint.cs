using UnityEngine;

public class SprayPaint : MonoBehaviour
{
    public GameObject sprayPaintPrefab; // Assign your sprite prefab here
    public float sprayDistance = 5f;   // Maximum distance for spray paint
    public LayerMask sprayableLayer;   // Specify layers that can be spray painted

    void Update()
    {
        // Check for the "G" key press
        if (Input.GetKeyDown(KeyCode.G))
        {
            Spray();
        }
    }

    void Spray()
{
    // Cast a ray from the player's position forward
    Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);


    // Visualize the raycast in the Scene view
    Debug.DrawRay(ray.origin, ray.direction * sprayDistance, Color.red, 2f);

    // Perform the raycast and log debugging information
    if (Physics.Raycast(ray, out RaycastHit hit, sprayDistance))
    {
        Debug.Log($"Hit {hit.collider.gameObject.name} at {hit.point}");

        // Instantiate the spray paint sprite at the hit point
        GameObject spray = Instantiate(sprayPaintPrefab, hit.point, Quaternion.LookRotation(hit.normal));

        // Optionally scale or rotate the spray paint for variety
        spray.transform.localScale *= Random.Range(0.8f, 1.2f); // Randomize size slightly
        spray.transform.Rotate(0, Random.Range(0, 360), 0);      // Randomize rotation

        // Destroy the spray paint after a few seconds (optional)
        Destroy(spray, 10f); // Adjust lifetime as needed
    }
    else
    {
        Debug.Log("No valid surface detected for spray paint.");
        Debug.Log($"Ray origin: {ray.origin}, direction: {ray.direction}, distance: {sprayDistance}");
    }
}

}