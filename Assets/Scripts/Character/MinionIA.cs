using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MinionIA : MonoBehaviour, ICharacterInterface
{
    private HeroScript _heroAlly;

    private float _speedOriginal;
    private float _speed;
    private float _radiusBehavior = 2;
    private float _radiusVision = 3;
    private float _radiusForPatrol = 5;
    private Vector3 _dir;
    private ISteeringBehaviour _sBehaviour;
    private bool _isObstacleAhead;
    private bool _enemyFound;
    public GameObject _objetiveToGo;
    private GameObject _objectState;
    private readonly int _angle = 40;
    private bool _IsBlockingDamage = false;
    private float _timeBlocking;
    private readonly float _timeBlockingTotal = 2;
    private LayerMask _layerEnemy;
    private int _life;
    private int _lifeTotalMinion;
    private int _lifeTrigerBlock;
    private int _damage;

    // FSMMinion
    private FSM<StateMinionEnum> _fsm;
    private FSMMoveToObjective<StateMinionEnum> _moveToObjFSM;
    private FSMAttackMinion<StateMinionEnum> _attackFSM;
    private FSMPatrol<StateMinionEnum> _patrolFSM;

    // ObstaculeAvoidance Stuff
    private float _avoidWeigth = 2;

    // Flocking stuff
    private LayerMask _maskFlock;
    private readonly float _leader = 2;
    private readonly float _separation = 5;
    private readonly float _alineation = 2;
    private readonly float _cohesion = 2;

    void Start()
    {
        if (HeroAlly != null)
        {
            SetUpMinionAttributes();
            CreateAndSetFSM();
            SetColorForState(StateMinionEnum.move);
        }
        else
        {
            Debug.Log("heroAlly not set.");
            this.gameObject.SetActive(false);
        }

    }

    void Update()
    {
        if (_fsm != null)
        {
            _fsm.OnUpdate();
        }

        if (ObjectiveToGo == null)
        {
            ResetMinionBehavior();
        }

        if (IsBlockingDamage)
        {
            BlockDamage();
        }
    }

    #region "Get/Set"

    public bool IsBlockingDamage
    {
        get { return _IsBlockingDamage; }
        set { _IsBlockingDamage = value; }
    }

    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    public int Life
    {
        get { return _life; }
        set { _life = value; }
    }

    public bool EnemyFound
    {
        get { return _enemyFound; }
        set { _enemyFound = value; }
    }

    public LayerMask LayerEnemy
    {
        get { return _layerEnemy; }
        set { _layerEnemy = value; }
    }

    public GameObject ObjectiveToGo
    {
        get { return _objetiveToGo; }
        set { _objetiveToGo = value; }
    }

    public ISteeringBehaviour SBehaviour
    {
        get { return _sBehaviour; }
    }

    public HeroScript HeroAlly
    {
        get { return _heroAlly; }
        set { _heroAlly = value; }
    }

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public bool IsObstacleAhead
    {
        get { return _isObstacleAhead; }
        set { _isObstacleAhead = value; }
    }

    public float RadiusForPatrol
    {
        get { return _radiusForPatrol; }
        set { _radiusForPatrol = value; }
    }

    public float RadiusBehavior
    {
        get { return _radiusBehavior; }
        set { _radiusBehavior = value; }
    }

    #endregion

    #region "Functions"

    public void SetUpToFollowEnemy(GameObject obj)
    {
        EnemyFound = true;
        Speed = Speed + 1f;
        ObjectiveToGo = obj;
        SetStateFSM(StateMinionEnum.move);
    }

    public void HealMinion()
    {
        if (_life < _lifeTotalMinion)
        {
            _life = _life + 1;
        }
    }

    public void ResetMinionBehavior()
    {
        if (EnemyFound) EnemyFound = false;
        SetOriginalSpeed();
        ObjectiveToGo = HeroAlly.gameObject;
        SetStateFSM(StateMinionEnum.move);
    }

    public void GetDamage(int num)
    {
        if (!IsBlockingDamage)
        {
            _life = _life - num;

            if (_life <= _lifeTrigerBlock)
            {
                IsBlockingDamage = true;
            }

            if (_life <= 0)
            {
                this.Die();
            }
        }
    }

    private void BlockDamage()
    {
        if (_timeBlocking > 0)
        {
            _timeBlocking -= Time.deltaTime;
        }
        else
        {
            _timeBlocking = _timeBlockingTotal;
            IsBlockingDamage = false;
        }
    }

    public void Die()
    {
        HeroAlly.RemoveMinion(this.gameObject);
        Destroy(this.gameObject);
    }

    public void DieMinion(GameObject caller)
    {
        if (caller.GetComponent<HeroScript>() != null)
        {
            this.Die();
        }
    }

    public void AttackedFocus(GameObject obj)
    {
        ObjectiveToGo = obj;
    }

    public Tuple<bool, GameObject> IsTargetVisible()
    {
        Collider[] collEnemiesList = Physics.OverlapSphere(this.transform.position, _radiusVision, LayerEnemy);
        Tuple<bool, GameObject> tupleReturn = new Tuple<bool, GameObject>(false, null);

        if (collEnemiesList.Count() > 0)
        {
            foreach (var item in collEnemiesList)
            {
                if (Vector3.Distance(item.transform.position, this.transform.position) <= _radiusVision)
                {
                    var direction = item.transform.position - transform.position;
                    if (Vector3.Angle(transform.forward, direction) <= _angle)
                    {
                        RaycastHit rch;
                        LayerMask mask = LayerEnemy;
                        if (Physics.Raycast(transform.position, direction, out rch, _radiusVision, mask))
                        {
                            tupleReturn = new Tuple<bool, GameObject>(true, item.gameObject);
                            return tupleReturn;
                        }
                    }
                }
            }

        }

        return tupleReturn;
    }

    public void SetOriginalSpeed()
    {
        Speed = _speedOriginal;
    }

    public void SetSBehaviour(SteerBehaviourEnum index, Transform target)
    {
        switch (index)
        {
            case SteerBehaviourEnum.flee:
                _sBehaviour = new Flee(transform, target.transform);
                break;
            case SteerBehaviourEnum.seek:
                _sBehaviour = new Seek(transform, target.transform);
                break;
            case SteerBehaviourEnum.obstaculeAvoidance:
                _sBehaviour = new ObstaculeAvoidance(transform, target.transform, _radiusBehavior, _avoidWeigth);
                break;
            case SteerBehaviourEnum.flocking:
                _sBehaviour = new Flocking(transform, target.transform, _radiusBehavior, _maskFlock, _leader, _separation, _alineation, _cohesion);
                break;
            default:
                _sBehaviour = new Flocking(transform, target.transform, _radiusBehavior, _maskFlock, _leader, _separation, _alineation, _cohesion);
                break;
        }
    }

    public void MoveMinion()
    {
        _dir = _sBehaviour.GetDir();
        this.transform.position += _dir * _speed * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, _dir, 0.2f);
    }

    public void SetStateFSM(StateMinionEnum index)
    {
        SetColorForState(index);
        _fsm.Transition(index);
    }

    public void SetColorForState(StateMinionEnum index)
    {
        switch (index)
        {
            case StateMinionEnum.attack:
                _objectState.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                break;
            case StateMinionEnum.move:
                _objectState.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                break;
            case StateMinionEnum.patrol:
                _objectState.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                break;
            default:
                _objectState.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                break;
        }
    }

    private void CreateAndSetFSM()
    {
        _moveToObjFSM = new FSMMoveToObjective<StateMinionEnum>(this);
        _attackFSM = new FSMAttackMinion<StateMinionEnum>(this);
        _patrolFSM = new FSMPatrol<StateMinionEnum>(this);

        _moveToObjFSM.SetTransition(StateMinionEnum.patrol, _patrolFSM);
        _moveToObjFSM.SetTransition(StateMinionEnum.attack, _attackFSM);
        _moveToObjFSM.SetTransition(StateMinionEnum.move, _moveToObjFSM); // needed to move if minion is too far from hero

        _patrolFSM.SetTransition(StateMinionEnum.move, _moveToObjFSM);
        _attackFSM.SetTransition(StateMinionEnum.move, _moveToObjFSM);

        _fsm = new FSM<StateMinionEnum>(_moveToObjFSM);

    }

    private void SetUpMinionAttributes()
    {
        Damage = 1;
        ObjectiveToGo = HeroAlly.gameObject;
        _speedOriginal = HeroAlly.Speed + 2f;
        Speed = _speedOriginal;
        _lifeTotalMinion = HeroAlly.LifeTotalMinion;
        _life = _lifeTotalMinion;
        LayerEnemy = HeroAlly.LayerMinionEnemy;
        _maskFlock = HeroAlly.gameObject.layer;
        _objectState = this.gameObject.transform.GetChild(1).gameObject;
        _lifeTrigerBlock = 40 * _lifeTotalMinion / 100; // 20 * 10 / 100
        _timeBlocking = _timeBlockingTotal;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radiusBehavior);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radiusVision);

        Vector3 rightLimit = Quaternion.AngleAxis(_angle, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * _radiusVision));

        Vector3 leftLimit = Quaternion.AngleAxis(-_angle, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * _radiusVision));


        Gizmos.color = Color.blue;

        if (ObjectiveToGo != null)
        {
            Gizmos.DrawLine(this.transform.position, new Vector3(ObjectiveToGo.transform.position.x, this.transform.position.y, ObjectiveToGo.transform.position.z));
        }

        if (RadiusForPatrol > 0)
        {
            Gizmos.DrawWireSphere(transform.position, _radiusForPatrol);
        }
    }

}
