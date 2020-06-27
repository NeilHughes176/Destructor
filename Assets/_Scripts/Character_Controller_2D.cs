// Basic Controller Adapted from Brackeys 2D Movement Tutorial

using UnityEngine;
using UnityEngine.Events;
using System.Collections;


public class Character_Controller_2D : MonoBehaviour
{
    // script refs
    private Character_Controller_2D character_cont;
    private Player_Anim_Controller anim_cont;

    [Header("Collide Settings")]
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Button Names")]
    [SerializeField] private string horizontal_axis_name = "Horizontal";
    [SerializeField] private string jump_button_name = "Jump";
    [SerializeField] private string dash_button_name = "Dash";
    [SerializeField] private string restart_button = "Restart";

    [Header("Movement Settings")]
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [SerializeField] private float m_Enemy_bounce_force = 400f;
    [SerializeField] private int move_speed_mult = 1;
    [SerializeField] private int rb_gravity_value = 1;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    private float horizontal_move = 0;
    private bool is_jumping = false;

    // dashing variables
    [Header("Dashing Settings")]
    [SerializeField] private float m_DashForce = 400f;
    [SerializeField] private float dash_speed;
    [SerializeField] private float dash_distance;
    private bool dash_pressed;
    private bool is_dashing; public bool GetIsDashing() { return is_dashing; }
    private bool can_dash = true;

    // death variables
    [Header("Death Settings")]
    [SerializeField] private float death_hang_time_seconds = 2f; // the amount of time between the player dying, and the game automaticlaly reseting
    private bool is_dead = false;


    [Header("Sound effects")]
    public AudioClip Jump;
    public AudioClip Dash;
    public AudioClip Death;

    private Vector2 orig_position;  // original start position

    private void ResetGame()
    {
        can_dash = true;
        dash_pressed = false;
        is_jumping = true;
        is_dead = false;
        ResetDashVariables();
        gameObject.transform.position = orig_position;
    }



    private void Awake()
    {
        // get script refs
        character_cont = gameObject.GetComponent<Character_Controller_2D>();
        anim_cont = gameObject.GetComponent<Player_Anim_Controller>();

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        SetGrav();
        orig_position = gameObject.transform.position;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        horizontal_move = Input.GetAxisRaw(horizontal_axis_name) * move_speed_mult;

        if (Input.GetButtonDown(jump_button_name))
        {
            is_jumping = true;
        }
        if (Input.GetButtonDown(dash_button_name))
        {
            dash_pressed = true;
        }
        else
            dash_pressed = false;

        if (Input.GetButtonDown(restart_button))
        {
            ResetGame();
        }

        m_Rigidbody2D.gravityScale = rb_gravity_value;
        SetGrav();
        SetCanDash();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        foreach (var collider in colliders)
        {
            m_Grounded = collider.gameObject != gameObject;
            if (m_Grounded)
                break;
        }

        // move charcater
        character_cont.Move(horizontal_move * Time.fixedDeltaTime, is_jumping, dash_pressed);

        SetAnim();

        is_jumping = false;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy_0"))
        {
            //ResetGame();

            // if dashing
            // go up and get dash back

            // else if not dashing
            // D I E

            if (is_dashing)
            {
                m_Rigidbody2D.AddForce(new Vector2(0, m_Enemy_bounce_force * 2));
                ResetDashVariables();
            }
            else if (!is_dashing)
            {
                is_dead = true;
                is_dashing = false;
                // start death coroutine
                StartCoroutine(DeathCoRout());
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        float x_speed = m_Rigidbody2D.velocity.x;
        if (!is_dashing && x_speed > 0.1f)
            return;
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Door"))
            ResetDashVariables();
    }


    void ResetDashVariables()
    {
        // unfreeze all restraints (in this case Y) then refreeze rotation as set
        m_Rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        // no longer dashing
        is_dashing = false;
        can_dash = true;
    }

    public void Move(float move, bool jump = false, bool dash_input = false)
    {


        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {



            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            AudioSource JumpSource = GetComponent<AudioSource>();
            JumpSource.clip = Jump;
            JumpSource.Play();
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
        // if player can dash, and isnt already dashing, and should dash based on input
        if (can_dash && !is_dashing && dash_input)
        {
            /*
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY;

            // if facing right
            if(m_FacingRight)
            // add horizontal force to player
                m_Rigidbody2D.AddForce(new Vector2(m_DashForce, 0.0f));
            else if(!m_FacingRight)
                m_Rigidbody2D.AddForce(new Vector2(-m_DashForce, 0.0f));
            */
            is_dashing = true;
            can_dash = false;
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            AudioSource DashSource = GetComponent<AudioSource>();
            DashSource.clip = Dash;
            DashSource.Play();
            StartCoroutine(DashCoRout(gameObject.transform.position.x));
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

    private void SetGrav()
    {
        m_Rigidbody2D.gravityScale = rb_gravity_value;
    }

    private void SetCanDash()
    {
        if (m_Grounded)
            can_dash = true;
    }

    private IEnumerator DeathCoRout()
    {
        AudioSource DeathSource = GetComponent<AudioSource>();
        DeathSource.clip = Death;
        DeathSource.Play();
        yield return new WaitForSeconds(death_hang_time_seconds);

        ResetGame();
    }

    private IEnumerator DashCoRout(float dash_start_x)
    {
        while (is_dashing)
        {
            if (m_FacingRight)
            {
                if (gameObject.transform.position.x < dash_start_x + dash_distance)
                {
                    // keep dashing
                    m_Rigidbody2D.AddForce(new Vector2(m_DashForce * dash_speed, 0.0f));
                    is_dashing = true;
                }
                else
                {
                    ResetDashVariables();
                }
            }
            else
            {
                if (gameObject.transform.position.x > dash_start_x - dash_distance)
                {
                    // keep dashing
                    m_Rigidbody2D.AddForce(new Vector2(-m_DashForce * dash_speed, 0.0f));
                    is_dashing = true;
                }
                else
                {
                    ResetDashVariables();
                }
            }
            yield return new WaitForFixedUpdate();
        }

    }

    private void SetAnim()
    {
        bool is_left_pressed = horizontal_move < 0.0f;
        bool is_right_pressed = horizontal_move > 0.0f;
        anim_cont.SetAnimStates(m_FacingRight, !m_Grounded, is_dashing, is_left_pressed, is_right_pressed, is_dead);

    }

    
}