using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WalkTo : MonoBehaviour
{
    [SerializeField] private List<Transform> TargetPos;
    private NavMeshAgent agent;
    private Transform currentPos;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentPos = TargetPos[0];
        
        agent.destination = currentPos.position;
    }

    private void FixedUpdate()
    {
        var dist = Vector3.Distance(agent.transform.position, currentPos.position);
        if (dist < 3f)
        {
            var indexOf = TargetPos.IndexOf(currentPos);
            currentPos = TargetPos[(indexOf + 1) % TargetPos.Count];
            agent.destination = currentPos.position;
        }

    }
}
