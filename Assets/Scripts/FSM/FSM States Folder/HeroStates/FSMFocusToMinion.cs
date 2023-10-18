
using System.Collections.Generic;
using UnityEngine;

public class FSMFocusToMinion<T> : FSMState<T>
{
    private HeroScript _heroScript;

    public FSMFocusToMinion(HeroScript heroScript)
    {
        _heroScript = heroScript;
    }

    public override void OnEnter()
    {
        FocusToMinion();
    }

    public override void OnUpdate()
    {

    }

    public override void OnSleep()
    {
        
    }

    private void FocusToMinion()
    {
        List<GameObject> listMinions = _heroScript.MinionsList;
        if (_heroScript.ObjectiveToGo != null)
        {
            foreach (GameObject itemObj in listMinions)
            {
                itemObj.GetComponent<MinionIA>().SetUpToFollowEnemy(_heroScript.ObjectiveToGo);
                GameManager.gameManagerStatic.CreateParticle(itemObj, StateHeroEnum.focusToMinion);
            }
        }
    }

}
