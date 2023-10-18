
using UnityEngine;

public class FSMAttackMinion<T> : FSMState<T>
{
    private MinionIA _minionIA;
    private float _time;
    private readonly float timeTotal = 0.3f;

    public FSMAttackMinion(MinionIA minionIA)
    {
        _minionIA = minionIA;
    }

    public override void OnEnter()
    {
        if (_minionIA.ObjectiveToGo.GetComponent<ICharacterInterface>() != null)
        {
            _minionIA.ObjectiveToGo.GetComponent<ICharacterInterface>().AttackedFocus(_minionIA.gameObject);
        }
    }

    public override void OnUpdate()
    {
        if (_minionIA.ObjectiveToGo != null)
        {
            Attack();
           CheckDistanceWithObjective();
        }
        else
        {
            _minionIA.ResetMinionBehavior();
        }
    }

    public override void OnSleep()
    {
        
    }

    private void Attack()
    {
        if (_time > 0)
        {
            _time -= Time.deltaTime;
        }
        else
        {
            _minionIA.ObjectiveToGo.GetComponent<ICharacterInterface>().GetDamage(_minionIA.Damage);
            _time = timeTotal;
        }
    }

    private void CheckDistanceWithObjective()
    {
        if (Vector3.Distance(_minionIA.ObjectiveToGo.transform.position, _minionIA.gameObject.transform.position) > _minionIA.RadiusBehavior)
        {
            _minionIA.ResetMinionBehavior();
        }
    }
}
