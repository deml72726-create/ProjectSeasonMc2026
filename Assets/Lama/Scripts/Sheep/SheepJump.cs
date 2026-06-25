using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SheepMovement : MonoBehaviour
{
    public enum SheepTier 
    { 
        Sluggish, 
        Standard, 
        Hyper 
    }

    [Header("Movement Settings")]
    [Tooltip("How fast the sheep walks forward.")]
    public float walkSpeed = 3f;
    [Tooltip("How high the sheep jumps over the gap.")]
    public float jumpForce = 7f;

    [Header("Floor Checks")]
    [Tooltip("Drag the GroundCheck object (at the feet) here.")]
    public Transform groundCheck;
    [Tooltip("Drag the new EdgeCheck object (in front of the sheep) here.")]
    public Transform edgeCheck;
    
    [Tooltip("How wide the circle checking for the floor is.")]
    public float groundCheckRadius = 0.2f;
    [Tooltip("How far down the sheep looks to detect a gap.")]
    public float edgeLookDistance = 1f;
    
    [Tooltip("Select the 'floor' layer here.")]
    public LayerMask whatIsFloor;

    private Rigidbody2D rb;
private Animator anim; // 1. Added Animator variable
    private bool isGrounded;
    public SheepTier currentMode = SheepTier.Standard;
public enum GameState { Playing, Won, Lost }

[Header("End Game Settings")]
public GameState currentState = GameState.Playing;
public float loseSpinSpeedZ = 360f; // Degrees per second
public float winSpinSpeedY = 360f;
public GameObject shadow; // Assign your win effect prefab in the Inspector

//SOUNDDECLARATION----
[Range(0f, 1f)]
public float chancesound;
public AudioSource sheepJumpSound1;
public AudioSource sheepJumpSound2;
public AudioSource sheepJumpSound3;
public AudioSource OIIAI;
public AudioSource scream;
bool firsttime = true;


    void Start()
    {
        // Pick a random number between 0 and 2
        int randomTier = Random.Range(0, 3); 
        
        // Cast that number into our enum (0 = Sluggish, 1 = Standard, 2 = Hyper)
        currentMode = (SheepTier)randomTier;

        ApplyTierStats();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

void Update()
{
    if (currentState == GameState.Playing)
    {
        // --- YOUR EXISTING CODE STAYS EXACTLY THE SAME HERE ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsFloor);
        anim.SetBool("IsGrounded", isGrounded);

        RaycastHit2D hitFloorAhead = Physics2D.Raycast(edgeCheck.position, Vector2.down, edgeLookDistance, whatIsFloor);

        if (isGrounded && hitFloorAhead.collider == null)
        {
            Jump();
        }
    }
    else if (currentState == GameState.Lost)
    {
        // If lost, spin wildly on the Z axis (like a tumble)
        rb.linearVelocity = new Vector2(walkSpeed, rb.linearVelocity.y);
        GetComponent<Collider2D>().enabled = false; // Optional: Disable the collider so it falls through the floor
        scream.Play();
    }
    else if (currentState == GameState.Won)
    {
        OIIAI.Play();
        // We still need to check the ground so it knows when it landed
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsFloor);
        anim.SetBool("IsGrounded", isGrounded);

        // Only start the Y-axis celebration spin if it has touched the floor
        if (isGrounded)
        {
            transform.Rotate(0, winSpinSpeedY * Time.deltaTime, 0);
        }
        
    }
    if (isGrounded)
        {
            shadow.SetActive(true);
        }
        else
        {
            shadow.SetActive(false);
        }
}

void FixedUpdate()
{
    if (currentState == GameState.Playing)
    {
        // Normal walking
        rb.linearVelocity = new Vector2(walkSpeed, rb.linearVelocity.y);
    }
    else
    {
        // Stop forward movement instantly, but let gravity keep pulling them down
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
}
    public void ApplyTierStats()
        {
            // 2. Use a switch statement to set the values based on the current mode
            switch (currentMode)
            {
                case SheepTier.Sluggish:
                    walkSpeed = 5.0f;
                    jumpForce = 9.0f;
                    break;

                case SheepTier.Standard:
                    walkSpeed = 7.0f;
                    jumpForce = 7.0f;
                    break;

                case SheepTier.Hyper:
                    walkSpeed = 9.0f;
                    jumpForce = 5.0f;
                    break;
            }
            
            Debug.Log("Sheep spawned as " + currentMode + " with speed: " + walkSpeed);
        }
    

    private void Jump()
    {
        // Apply the jump force upwards
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        chancesound = Random.Range(0f, 1f); // Random number between 0 and 1
// Play a random jump sound
    if (firsttime)
    {
        if (chancesound > 0.5f) // 30% chance to play a sound
        {
            int randomSound = Random.Range(1, 4); // Random number between 1 and 3
            switch (randomSound)
            {
                case 1:
                    sheepJumpSound1.Play();
                    break;
                case 2:
                    sheepJumpSound2.Play();
                    break;
                case 3:
                    sheepJumpSound3.Play();
                    break;
            }
        }
        firsttime = false; // Set firsttime to false after the first jump
    }
    }

    // This draws helpful lines in the Unity Editor so you can see the edge detection
    private void OnDrawGizmosSelected()
    {
        // Draw the Ground Check at the feet
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Draw the Edge Check line in front of the sheep
        if (edgeCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + (Vector3.down * edgeLookDistance));
        }
    }

public void TriggerWin()
{
    currentState = GameState.Won;
}

public void TriggerLose()
{
    currentState = GameState.Lost;
    // Optional: Turn off the collider so it falls completely through the floor and off-screen!
    // GetComponent<Collider2D>().enabled = false; 
}
}