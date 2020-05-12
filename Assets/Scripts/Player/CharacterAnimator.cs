using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;
    private CharacterController cc;

    // Update is called once per frame
    private void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        animator.SetBool("Grounded", cc.isGrounded);

        if(Input.GetKeyDown(KeyBinds.Jump))
        {
            animator.SetTrigger("Jump");
        }

        animator.SetBool("Running", Input.GetKey(KeyBinds.Run));

        animator.SetBool("Forward", Input.GetKey(KeyBinds.Forward));
        animator.SetBool("Backward", Input.GetKey(KeyBinds.Backward));
        animator.SetBool("Left", Input.GetKey(KeyBinds.Left));
        animator.SetBool("Right", Input.GetKey(KeyBinds.Right));
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
}
