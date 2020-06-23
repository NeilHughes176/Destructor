using UnityEngine;

public class Player_Anim_Controller : MonoBehaviour {

    public Animator player_animator;

    private void Awake()
    {
        //player_animator = gameObject.GetComponent<Animator>();
    }


    public void SetAnimStates(bool is_facing_right, bool is_jumping, bool is_dashing, bool is_left_pressed, bool is_right_pressed, bool is_dead)
    {

        player_animator.SetBool("is_facing_right", is_facing_right);
        player_animator.SetBool("is_jumping", is_jumping);
        player_animator.SetBool("is_dashing", is_dashing);
        player_animator.SetBool("is_left_pressed", is_left_pressed );
        player_animator.SetBool("is_right_pressed", is_right_pressed);
        player_animator.SetBool("is_dead", is_dead);

        bool is_moving = false;
        if(is_left_pressed || is_right_pressed)
            is_moving = true;

        player_animator.SetBool("is_moving", is_moving);
    }

}
