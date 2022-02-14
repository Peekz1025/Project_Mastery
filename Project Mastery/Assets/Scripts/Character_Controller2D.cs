using UnityEngine;
using UnityEngine.Events;

public class Character_Controller2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private float runSpeed = 40f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [Range(0f, 2f)] [SerializeField] private float jumpCutMultiplier = 0.5f;
    
    private const float k_GroundedRadius = .2f;                                 // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;                                                    // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;                                          // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
    private float horizontalMove = 0;
    private bool isJumping = false;


    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}

    void Update()
    {
        m_Grounded = Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

        Jump();
    }

    private void FixedUpdate()
	{
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        Move(horizontalMove * Time.fixedDeltaTime);
	}

    public void Move(float move)
    {
		// Move the character by finding the target velocity
		Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
		// And then smoothing it out and applying it to the character
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        // If the input is moving the player right and the player is facing left...
        if (move > 0 && !m_FacingRight)
		{
			Flip();
		}
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (move < 0 && m_FacingRight)
		{
			Flip();
		}
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    public void Jump()
    {
        // If the player should jump...
        if (m_Grounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            
            float force = m_JumpForce;
            if (m_Rigidbody2D.velocity.y < 0)
                force -= m_Rigidbody2D.velocity.y;
            m_Rigidbody2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }
        
        if (isJumping && m_Rigidbody2D.velocity.y > 0 && Input.GetButtonUp("Jump"))
        {
            m_Rigidbody2D.AddForce(Vector2.down * m_Rigidbody2D.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }
    }
}
//Debug.Log("I AM HERE");
