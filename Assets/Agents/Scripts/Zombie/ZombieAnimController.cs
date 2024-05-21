using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAnimController : MonoBehaviour
{
    #region//SerializedField
    [SerializeField] Transform frontPerception, backPerception, leftPerception, rightPerception, arrivePoint;
    [SerializeField] float arriveRadious, frontPerceptionRadious, backPerceptionRadoius, sidesPerceptionRadious, wheelRadious, wheelDisplacement,wanderCooldown,maxSpeed,maxForce,rotationSpeed ;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] SphereCollider attackColliderRight, attackColliderLeft;
    [SerializeField] GameObject leftArm, rightArm;
    #endregion
    #region//Member
    bool isPlayerInPerception;
    Animator animator;
    Rigidbody rigidBody;
    private Vector3 desired_Vel, steering, wheel,randPos,test;
    private NavMeshAgent navMeshAgent;
    #endregion
    #region//Public
    public float wanderNewPos, attackRange;
    public Graph<Vector3> pathFindGraph;
    [HideInInspector] Transform target;
    #endregion
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        
    }
    private void FixedUpdate()
    {
        if(PlayerInVision())
        {
            animator.SetBool("isSeeking", true);
        }
        else
        {
            animator.SetBool("isSeeking", false);
        }
    }
    void Update()
    {

        getLocalCoordinates(attackColliderLeft, leftArm.transform);
        getLocalCoordinates(attackColliderRight, rightArm.transform);
    }
    void getLocalCoordinates(SphereCollider collider,Transform localPos)
    {
        Vector3 colliderToLocalPos = transform.InverseTransformPoint(localPos.position);
        collider.center = colliderToLocalPos;
    }
    void LookAtFixed(Vector3 target)
    {
        if (target != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(target);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
       
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(frontPerception.position, frontPerceptionRadious);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(backPerception.position, backPerceptionRadoius);
        Gizmos.DrawWireSphere(rightPerception.position, sidesPerceptionRadious);
        Gizmos.DrawWireSphere(leftPerception.position, sidesPerceptionRadious);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(wheel, wheelRadious);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(test, 1);
    }
    public bool PlayerInVision()
    {
        isPlayerInPerception = Physics.CheckSphere(frontPerception.position, frontPerceptionRadious, playerLayer);
        return isPlayerInPerception;
    }
    public GameObject getPlayerInVision()
    {
        Collider[] colliders = Physics.OverlapSphere(frontPerception.position, frontPerceptionRadious, playerLayer);
        Transform playerTransform;
        foreach (Collider hit in colliders)
        {
            playerTransform = hit.gameObject.transform;
            target = playerTransform;

        }
        return target.gameObject;
    }
    public void Seek(Vector3 target)
    {
        //LookAtFixed(desired_Vel);
        desired_Vel.x = target.x - transform.position.x;
        desired_Vel.z = target.z - transform.position.z;
        desired_Vel = (desired_Vel.normalized * maxSpeed);
        steering = desired_Vel - rigidBody.velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering = steering / rigidBody.mass;
        rigidBody.velocity = Vector3.ClampMagnitude((steering + rigidBody.velocity), maxSpeed);
        //transform.position += rigidBody.velocity;
        navMeshAgent.destination = target;
        Debug.DrawRay(transform.position, rigidBody.velocity.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, desired_Vel.normalized * 2, Color.magenta);
    }
    public void Wander()
    {
            wheel = (rigidBody.velocity.normalized * wheelDisplacement) + transform.position;
        if (wanderNewPos <= 0 || (transform.position - randPos).magnitude <= 0.5f)
        {
            randPos = new Vector3(Random.Range(-1f, 1f), transform.position.y, Random.Range(-1f, 1f));
            randPos = randPos.normalized * wheelRadious;
            randPos += wheel;
            randPos.y = transform.position.y;
            test = randPos;
            wanderNewPos = wanderCooldown; 
        }
        else
        {
            //Seek(test);
            wanderNewPos -= Time.deltaTime;
        }
        Seek(test);
    }
    public void ActivateLeftCollider()
    {
        attackColliderLeft.enabled = true;
    }
    public void ActivateRightCollider()
    {
        attackColliderRight.enabled = true;
    }
    public void DeactivateLeftCollider()
    {
        attackColliderLeft.enabled = false;
    }
    public void DeactivateRightCollider()
    {
        attackColliderRight.enabled = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider == attackColliderLeft)
        {
            
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other == attackColliderLeft)
        {
            Debug.Log("ataque");
        }
    }

}
