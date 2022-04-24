using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private Transform target;
    private void Start()
    {
        GetReferences();
    }

    private void Update()
    {
        MoveToTarget();
    }

    private void GetReferences()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void MoveToTarget()
    {
        _agent.SetDestination(target.position);
        RotateToTarget();
      
    }

    private void RotateToTarget()
    {
        transform.LookAt(target);

        //Vector3 direction = target.position - transform.position;
        //Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        //transform.rotation = rotation;
    }
}
