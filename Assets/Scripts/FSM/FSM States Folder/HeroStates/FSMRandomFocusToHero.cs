
using System.Collections.Generic;
using UnityEngine;

public class FSMRandomFocusToHero<T> : FSMState<T>
{
    private HeroScript _heroScript;

    public FSMRandomFocusToHero(HeroScript heroScript)
    {
        _heroScript = heroScript;
    }

    public override void OnEnter()
    {
        FocusToHeroEnemy();
    }

    public override void OnUpdate()
    {

    }

    public override void OnSleep()
    {

    }

    private void FocusToHeroEnemy()
    {
        List<GameObject> listMinions = _heroScript.MinionsList;
        if (_heroScript.HeroEnemy != null)
        {
            foreach (GameObject itemObj in listMinions)
            {
                itemObj.GetComponent<MinionIA>().SetUpToFollowEnemy(_heroScript.HeroEnemy);
                GameManager.gameManagerStatic.CreateParticle(itemObj, StateHeroEnum.random);
            }
        }
    }

}
