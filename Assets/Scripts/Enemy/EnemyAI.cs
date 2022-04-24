using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // References
    public NavMeshAgent agent;
    public Transform player;
    //public Transform playerAlly;
    public LayerMask whatIsGround, whatIsPlayer;

    //public GameObject projectile;

    //private Health playerHealth;
    private Animator anim;


    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    float distanceToPlayer;
    float distanceToPlayerAlly;
    bool targetLocked;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // // Who will enemy attack first? Player or player allies
        // if (playerInSightRange && !targetLocked && playerAlly != null)
        // {
        //     distanceToPlayer = Mathf.Abs(Vector3.Distance(transform.position, player.position));
        //     distanceToPlayerAlly = Mathf.Abs(Vector3.Distance(transform.position, playerAlly.position));
        //
        //     if (distanceToPlayer > distanceToPlayerAlly)
        //     {
        //         player = GameObject.Find("PlayerAlly").transform;
        //         targetLocked = true;
        //     }
        //     else
        //     {
        //         player = GameObject.Find("Viking_ragnar_v2").transform;
        //         targetLocked = true;
        //     }
        //
        //
        //     //float distanceToEnemy = Mathf.Max(distanceToPlayer, distanceToPlayerAlly);
        //
        // }
        if (player == null)
        {
            player = GameObject.Find("Player").transform;
        }


        //float velocity = agent.velocity.magnitude / agent.speed;
        //anim.SetFloat("Speed", agent.velocity.magnitude);
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }

        if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
            Debug.Log("Attack player");
        }
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void Patroling()
    {
        //vel -= 0.1f;
        //anim.SetBool("Walk",true);
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }


    private void ChasePlayer()
    {
        //anim.SetFloat("Speed", vel, 0.1f, Time.deltaTime);
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
    
        //Make sure enemy doesnt move
        agent.SetDestination(transform.position);
        transform.LookAt(player);
    
        if (!alreadyAttacked)
        {
            //Attack code here
            anim.SetTrigger("Attack");

            // end of attack code
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    // void PlayFootstepsEvent(string path)
    // {
    //     FMOD.Studio.EventInstance Footsteps = FMODUnity.RuntimeManager.CreateInstance(path);
    //     //Footsteps.setParameterByName("Material", material);
    //     Footsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform));
    //     Footsteps.start();
    //     Footsteps.release();
    // }
}