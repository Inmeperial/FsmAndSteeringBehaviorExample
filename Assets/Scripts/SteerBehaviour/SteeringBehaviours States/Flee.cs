using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : ISteeringBehaviour
{
    Transform _from;
    Transform _target;
    public Flee(Transform from, Transform target)
    {
        _from = from;
        _target = target;
    }
    public Vector3 GetDir()
    {

        return -(_target.position - _from.position).normalized;
    }
}
