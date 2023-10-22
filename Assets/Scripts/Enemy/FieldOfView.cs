using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[HideScriptField]
public class FieldOfView : MonoBehaviour
{

    public float radius = 5f;
    [Range(0, 360)]
    public float angle = 100f;
    
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private GameEvent onPlayerEnterFieldOfView;
    [BeginReadOnlyGroup] // we should not be able to add remove visible targets in the inspector
    public List<Transform> visibleTargets = new List<Transform>();
    //[EndReadOnlyGroup]
    
    
    
    private void Start()
    {
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }
    
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    
    void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, radius, targetMask);
        var foundTarget = new List<Transform>();
        
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform; //get the transform of the target
            Vector3 dirToTarget = (target.position - transform.position).normalized; //get the direction to the target

            if (Vector3.Angle(transform.forward, dirToTarget) >= angle / 2) continue; //if target is not in the field of view, continue
            
            float dstToTarget = Vector3.Distance(transform.position, target.position);

            if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) continue;
            
            foundTarget.Add(target); //add target to foundTarget so we can compare it to visibleTargets later
            
            if (visibleTargets.Contains(target)) continue;
            visibleTargets.Add(target); //add target to visibleTargets
            onPlayerEnterFieldOfView?.RaiseEvent(this, target.gameObject);
        }
        
        // if an item is is in visibleTargets but not in foundTarget, remove it from visibleTargets
        // we do this so we can remove targets that are no longer visible
        // so when a player enters the field of view they can potentially have an event triggered
        for (int i = 0; i < visibleTargets.Count; i++)
        {
            if (foundTarget.Contains(visibleTargets[i])) continue;
            visibleTargets.Remove(visibleTargets[i]);
        }
    }
    
    public void SetAngle(float angle)
    {
        
        this.angle = Mathf.Clamp(angle, 0, 360);
    }
    
    public void SetRadius(float radius)
    {
        this.radius = radius;
    }
}
