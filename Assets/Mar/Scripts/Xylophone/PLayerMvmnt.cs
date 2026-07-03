using UnityEngine;

public class PlayerMvmnt : MonoBehaviour 
{ 
    public float moveSpeed = 8f; 
    private Rigidbody2D rb; 
    private Animator anim; 
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update() 
    { 
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Logic to switch between 3 states: Idle (0), Right (1), Left (2)
        if (horizontalInput > 0)
        {
            anim.SetInteger("state", 1); // Walk Right
        }
        else if (horizontalInput < 0)
        {
            anim.SetInteger("state", 2); // Walk Left
        }
        else
        {
            anim.SetInteger("state", 0); // Idle
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }
}