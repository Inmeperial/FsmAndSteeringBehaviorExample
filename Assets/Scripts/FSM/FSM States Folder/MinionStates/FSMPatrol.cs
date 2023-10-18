using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FSMPatrol<T> : FSMState<T>
{
    private readonly MinionIA _minionIA;
    private LayerMask _layerNode;

    public FSMPatrol(MinionIA minionIA)
    {
        _minionIA = minionIA;
        _layerNode = _minionIA.HeroAlly.LayerNodes;
    }

    public override void OnEnter()
    {
        Patrol();
        _minionIA.SetStateFSM(StateMinionEnum.move);
    }

    public override void OnUpdate()
    {

    }

    public override void OnSleep()
    {

    }

    private void Patrol()
    {

        Collider[] collEnemiesList = Physics.OverlapSphere(_minionIA.gameObject.transform.position, _minionIA.RadiusForPatrol, _layerNode);
        System.Random rd = new System.Random();
        int rInt = rd.Next(0, collEnemiesList.Length);

        _minionIA.ObjectiveToGo = collEnemiesList[rInt].gameObject;
    }

}

