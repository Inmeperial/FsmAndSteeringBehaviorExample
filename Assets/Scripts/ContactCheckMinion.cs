using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactCheckMinion : MonoBehaviour
{
    private MinionIA _minion;

    private void Start()
    {
        _minion = this.transform.parent.GetComponent<MinionIA>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.layer == LayerMask.NameToLayer("Level"))
        {
            _minion.IsObstacleAhead = true;
            return;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.gameObject.layer == LayerMask.NameToLayer("Level"))
        {
            _minion.IsObstacleAhead = false;
            return;
        }
    }
}
