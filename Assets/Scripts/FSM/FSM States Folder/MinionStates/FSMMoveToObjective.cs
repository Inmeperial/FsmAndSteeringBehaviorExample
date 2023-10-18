
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FSMMoveToObjective<T> : FSMState<T>
{
    private readonly MinionIA _minionIA;
    private Transform objectivePos;
    private GameObject _heroAlly;
    private Tuple<bool, GameObject> _checkEnemyTuple;
    private readonly float _timeForEnemyOnVisionTotal = 0.3f;
    private float _timeForEnemyOnVision;
    
    public FSMMoveToObjective(MinionIA minionIA)
    {
        _minionIA = minionIA;
        _heroAlly = _minionIA.HeroAlly.gameObject;
        _timeForEnemyOnVision = _timeForEnemyOnVisionTotal;
    }

    public override void OnEnter()
    {
        _minionIA.SetSBehaviour(SteerBehaviourEnum.obstaculeAvoidance, _minionIA.ObjectiveToGo.transform);
        objectivePos = _minionIA.ObjectiveToGo.transform;
    }

    public override void OnUpdate()
    {
        CheckDistanceWithObjective();

        if (_minionIA.ObjectiveToGo != _heroAlly)
        {
            CheckFarFromHero();
        }

        if (!_minionIA.EnemyFound)
        {
            CheckIfEnemyOnVision();
        }

        CheckObstacles();

        _minionIA.MoveMinion();
    }

    public override void OnSleep()
    {
        
    }

    private void CheckFarFromHero()
    {
        if (Vector3.Distance(_heroAlly.transform.position, _minionIA.transform.position) >= _minionIA.RadiusForPatrol)
        {
            _minionIA.ResetMinionBehavior();
        }
    }

    private void CheckDistanceWithObjective()
    {
        if (objectivePos == null)
        {
            _minionIA.ResetMinionBehavior();

        }

        if (Vector3.Distance(new Vector3(objectivePos.position.x, _minionIA.gameObject.transform.position.y, objectivePos.position.z), _minionIA.transform.position) < 0.5f)
        {
            if (_minionIA.ObjectiveToGo == _heroAlly)
            {
                _minionIA.HealMinion();
            }
            if (!_minionIA.EnemyFound)
            {
                _minionIA.SetStateFSM(StateMinionEnum.patrol);
            }
            else
            {
                _minionIA.SetStateFSM(StateMinionEnum.attack);
            }
        }
    }

    private void CheckObstacles()
    {
        if (_minionIA.IsObstacleAhead && _minionIA.SBehaviour.GetType() != typeof(ObstaculeAvoidance))
        {
            _minionIA.SetSBehaviour(SteerBehaviourEnum.obstaculeAvoidance, _minionIA.ObjectiveToGo.transform);
            return;
        }

        if (!_minionIA.IsObstacleAhead && _heroAlly.gameObject != null)
        {
            if (_minionIA.ObjectiveToGo == _heroAlly.gameObject && _minionIA.SBehaviour.GetType() != typeof(Flocking))
            {
                _minionIA.SetSBehaviour(SteerBehaviourEnum.flocking, _minionIA.ObjectiveToGo.transform);
                return;
            }
            if (_minionIA.ObjectiveToGo != _heroAlly.gameObject && _minionIA.SBehaviour.GetType() != typeof(Seek) && _heroAlly.gameObject != null)
            {
                _minionIA.SetSBehaviour(SteerBehaviourEnum.seek, _minionIA.ObjectiveToGo.transform);
                return;
            }
        }

    }

    private void CheckIfEnemyOnVision()
    {
        if (_timeForEnemyOnVision > 0)
        {
            _timeForEnemyOnVision -= Time.deltaTime;
        }
        else
        {
            _timeForEnemyOnVision = _timeForEnemyOnVisionTotal;
            _checkEnemyTuple = _minionIA.IsTargetVisible();
            if (_checkEnemyTuple.Item1)
            {
               _minionIA.SetUpToFollowEnemy(_checkEnemyTuple.Item2);
            }
        }
    }

}


