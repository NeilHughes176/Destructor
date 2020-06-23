using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour {

    public string horizontal_axis_name = "Horizontal";
    public string jump_button_name = "Jump";
    public string dash_button_name = "Dash";
    public int move_speed_mult = 1;
    public int rb_gravity_value = 1;

    private Character_Controller_2D character_cont;
    private Rigidbody2D rb;
    private float horizontal_move = 0;
    private bool is_jumping = false;

    public float dash_distance;
    public float dash_speed;
    private bool is_dashing = false;
    private bool can_dash = true;

    // Use this for initialization
    void Start ()
    {
        character_cont = gameObject.GetComponent<Character_Controller_2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
	}

	
	// Update is called once per frame
	void Update ()
    {
        horizontal_move = Input.GetAxisRaw(horizontal_axis_name) * move_speed_mult;
        
        if(Input.GetButtonDown(jump_button_name))
        {
            is_jumping = true;
        }
        if(Input.GetButtonDown(dash_button_name))
        {
            is_dashing = true;
            can_dash = false;
        }
        rb.gravityScale = rb_gravity_value;
	}

    private void FixedUpdate()
    {
        bool is_moving_left;
        if (horizontal_move > 0)
            is_moving_left = true;
        else
            is_moving_left = false;
        // move charcater
        character_cont.Move(horizontal_move * Time.fixedDeltaTime, is_jumping, is_dashing);
        is_jumping = false;
        is_dashing = false;
    }

    
}
