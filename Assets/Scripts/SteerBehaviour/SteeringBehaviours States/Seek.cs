using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : ISteeringBehaviour
{
    private Transform _from;
    private Transform _target;

    public Seek(Transform from, Transform target)
    {
        _from = from;
        _target = target;
    }

    public Vector3 GetDir()
    {
        Vector3 dir = new Vector3(0, 0, 0);

        if (_target != null)
        {
            dir = (_target.position - _from.position).normalized;
        }

        return new Vector3(dir.x, 0, dir.z);
    }

}
