using UnityEngine;

public class Sway : MonoBehaviour
{
    public float swayClamp = 0.09f;
    public float smoothing = 3f;
    private Vector3 origin;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse input
        Vector2 input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Clamp the input to avoid excessive sway
        input.x = Mathf.Clamp(input.x, -swayClamp, swayClamp);
        input.y = Mathf.Clamp(input.y, -swayClamp, swayClamp);

        // Calculate target position
        Vector3 target = new Vector3(input.x, -input.y, 0);

        // Smoothly move to the target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, target + origin, Time.deltaTime * smoothing);
    }
}
