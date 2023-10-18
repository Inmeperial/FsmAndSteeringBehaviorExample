using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculeAvoidance : ISteeringBehaviour
{
    Transform _from;
    Transform _target;
    float _radius;
    LayerMask _mask;
    float _avoidWeight;

    public ObstaculeAvoidance(Transform from, Transform target, float radius, float avoidWeight, LayerMask mask)
    {
        _from = from;
        _target = target;
        _avoidWeight = avoidWeight;
        _mask = mask;
        _radius = radius;
    }

    public Vector3 GetDir()
    {
        Vector3 dir = (_target.position - _from.position).normalized;
        Transform obs = null;
        Collider[] obstacles = Physics.OverlapSphere(_from.position, _radius, _mask);
        if (obstacles.Length > 0)
        {
            foreach (var item in obstacles)
            {
                if (!obs)
                    obs = item.transform;
                else if (Vector3.Distance(item.transform.position, _from.position) < Vector3.Distance(obs.position, _from.position))
                    obs = item.transform;
            }
        }
        if (obs)
        {
            Vector3 dirToObs = (_from.position - obs.position).normalized * _avoidWeight;
            dir += dirToObs;
        }
        
        return new Vector3(dir.x, 0, dir.z).normalized;
    }
}
