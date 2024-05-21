using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekBehavior : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public Transform target;
    private float attackRange;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /*maxSpeed = animator.GetComponent<ZombieAnimController>().maxSpeed;
        rigidB = animator.GetComponent<Rigidbody>();
        maxForce = animator.GetComponent<ZombieAnimController>().maxForce;*/
        target = animator.GetComponent<ZombieAnimController>().getPlayerInVision().transform;
        attackRange = animator.GetComponent<ZombieAnimController>().attackRange;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        animator.GetComponent<ZombieAnimController>().Seek(target.position);
        if((target.position - animator.transform.position).magnitude <= attackRange)
        {
            animator.SetBool("isAttacking", true);
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
