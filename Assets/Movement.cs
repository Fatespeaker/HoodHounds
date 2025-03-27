using UnityEngine;
using Photon.Pun;

public class Movement : MonoBehaviourPun
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float jumpForce = 7f;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private bool isDancing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Disable control for remote players
        if (!photonView.IsMine)
        {
            rb.isKinematic = true; // Prevent physics interference
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return; // Only allow local player control

        // Handle Dance Logic
        if (Input.GetKeyDown(KeyCode.E) && !isDancing)
        {
            StartDancing();
        }
        else if (isDancing && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetButtonDown("Jump")))
        {
            StopDancing();
        }

        // Skip movement logic if dancing
        if (isDancing) return;

        // Movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        bool isMoving = direction.magnitude >= 0.1f;

        // Handle walking and sprinting
        if (isMoving)
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            Move(direction, speed);

            // Update animator parameters
            UpdateAnimatorParameters(Input.GetKey(KeyCode.LeftShift), false);
        }
        else
        {
            // Stop walking/sprinting
            UpdateAnimatorParameters(false, false);
        }

        // Jump logic
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isGrounded = false;

            // Update animator parameters for jumping
            UpdateAnimatorParameters(false, true);
        }
    }

    private void StartDancing()
    {
        isDancing = true;
        animator.SetBool("IsDancing", true);

        // Notify other players to play the dance animation
        photonView.RPC("SyncDancingState", RpcTarget.Others, true);
    }

    private void StopDancing()
    {
        isDancing = false;
        animator.SetBool("IsDancing", false);

        // Notify other players to stop the dance animation
        photonView.RPC("SyncDancingState", RpcTarget.Others, false);
    }

    private void Move(Vector3 direction, float speed)
    {
        Vector3 move = transform.right * direction.x + transform.forward * direction.z;
        rb.linearVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            UpdateAnimatorParameters(false, false); // Update grounded state
        }
    }

    [PunRPC]
    private void SyncDancingState(bool isDancing)
    {
        this.isDancing = isDancing;
        animator.SetBool("IsDancing", isDancing);
    }

    private void UpdateAnimatorParameters(bool isSprinting, bool isJumping)
    {
        if (!photonView.IsMine) return;

        // Local animation updates
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        bool isWalking = direction.magnitude >= 0.1f && !isSprinting && !isJumping;

        // Update local animator
        animator.SetBool("IsSprinting", isSprinting);
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsJumping", isJumping);
    }
}