using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : ISteeringBehaviour
{
    Transform _from;
    float _radius;
    LayerMask _mask;
    Transform _leader;
    float _cohesionWeight;
    float _alineationWeight;
    float _separationWeight;
    float _leaderWeight;
    float _allWeight;

    public Flocking(Transform from, Transform leader, float radius, LayerMask mask,
        float leaderWeight = 1, float separationWeight = 1, float alineationWeight = 1, float cohesionWeight = 1)
    {
        _from = from;
        _radius = radius;
        _mask = mask;
        _leader = leader;
        _leaderWeight = leaderWeight;
        _separationWeight = separationWeight;
        _alineationWeight = alineationWeight;
        _cohesionWeight = cohesionWeight;
        _allWeight = cohesionWeight + alineationWeight + separationWeight + leaderWeight;
    }

    public Vector3 GetDir()
    {
        Collider[] coll = Physics.OverlapSphere(_from.transform.position, _radius, _mask);
        Transform[] collTransform = new Transform[coll.Length];

        for (int i = 0; i < coll.Length; i++)
        {
            collTransform[i] = coll[i].transform;
        }

        Vector3 separation = Separation(collTransform, _from, _radius) * (_separationWeight / _allWeight);
        Vector3 cohesion = Cohesion(collTransform, _from) * (_cohesionWeight / _allWeight);
        Vector3 alination = Alineation(collTransform, _from) * (_alineationWeight / _allWeight);
        Vector3 leader = Leader(_from, _leader) * (_leaderWeight / _allWeight);

        Vector3 dir = separation + cohesion + alination + leader;

        Vector3 vec = new Vector3(dir.x, 0, dir.z);

        return vec;
    }

    Vector3 Leader(Transform from, Transform leader)
    {
        return (leader.position - from.position).normalized;
    }

    Vector3 Cohesion(Transform[] coll, Transform from)
    {
        Vector3 cohesion = Vector3.zero;
        for (int i = 0; i < coll.Length; i++)
        {
            cohesion += coll[i].position;
        }
        Vector3 average = cohesion / coll.Length;
        return (average - from.position).normalized;
    }

    Vector3 Alineation(Transform[] coll, Transform from)
    {
        Vector3 alineation = Vector3.zero;
        for (int i = 0; i < coll.Length; i++)
        {
            alineation += coll[i].forward;
        }
        return alineation.normalized;
    }

    Vector3 Separation(Transform[] coll, Transform from, float radius)
    {
        Vector3 separation = Vector3.zero;
        for (int i = 0; i < coll.Length; i++)
        {
            Vector3 dirToColl = from.position - coll[i].position;
            float dirMultiplier = radius - dirToColl.magnitude;
            dirToColl *= dirMultiplier;
            separation += dirToColl;
        }
        return separation.normalized;
    }
}
