
using System.Collections.Generic;
using UnityEngine;

public class FSMAllBlocking<T> : FSMState<T>
{
    private HeroScript _heroScript;

    public FSMAllBlocking(HeroScript heroScript)
    {
        _heroScript = heroScript;
    }

    public override void OnEnter()
    {
        GetMinionWithLessLife();
    }

    public override void OnUpdate()
    {

    }

    public override void OnSleep()
    {

    }

    private void GetMinionWithLessLife()
    {
        List<GameObject> listMinions = _heroScript.GetMinionsList;

        foreach (GameObject itemObj in listMinions)
        {
            itemObj.GetComponent<MinionIA>().IsBlockingDamage = true;
            GameManager.gameManagerStatic.CreateParticle(itemObj, StateHeroEnum.allBlocking);
        }
    }
}
