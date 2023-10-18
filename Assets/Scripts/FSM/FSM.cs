using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T>
{
    FSMState<T> _current;

    public FSM(FSMState<T> initialState)
    {
        _current = initialState;
        _current.OnEnter();
    }

    public void OnUpdate()
    {
        _current.OnUpdate();
    }

    public void Transition(T input)
    {
        var newState = _current.GetTransition(input);
        if (newState == null) return;

        _current.OnSleep();
        _current = newState;
        _current.OnEnter();
    }

}
