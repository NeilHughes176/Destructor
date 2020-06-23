using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Anim_Script : MonoBehaviour {

    [SerializeField] private Animator[] local_animators;
    private Collider2D physical_collider;
    private Collider2D trigger_collider;

    public string restart_button_name = "Restart";

    void Start()
    {
        // get animators from children
        GetAnimators();
        GetColliders();
    }

    private void Update()
    {
        if(Input.GetButtonDown(restart_button_name))
        {
            RestartAnims();
        }
    }


    private void GetAnimators()
    {
        local_animators = gameObject.GetComponentsInChildren<Animator>();
    }

    private void GetColliders()
    {
        Collider2D[] colls = gameObject.GetComponents<Collider2D>();
        foreach (Collider2D coll in colls)
        {
            if (coll.isTrigger)
                trigger_collider = coll;
            else
                physical_collider = coll;
        }
    }

    private void RestartAnims()
    {
        foreach (Animator anim in local_animators)
        {
            anim.SetBool("is_broken", false);
        }
        physical_collider.enabled = true;

    }

    private void PlayAnims()
    {
        foreach  (Animator anim in local_animators)
        {
            anim.SetBool("is_broken", true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<Character_Controller_2D>().GetIsDashing())
            {
                physical_collider.enabled = false;
                PlayAnims();
            }
        }
    }
    
}
