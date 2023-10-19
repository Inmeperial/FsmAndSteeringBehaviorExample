
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FSMHyperHeal<T> : FSMState<T>
{
    private HeroScript _heroScript;
    private MinionIA _minion;

    public FSMHyperHeal(HeroScript heroScript)
    {
        _heroScript = heroScript;
    }

    public override void OnEnter()
    {

        _minion = GetMinionWithLessLife();
        if (_minion != null)
        {
            GameManager.gameManagerStatic.CreateParticle(_minion.gameObject, StateHeroEnum.hyperHeal);
            _minion.Life = _heroScript.LifeTotalMinion;
        }
        else
        {
            Debug.Log("Not minion found to Heal");
        }
    }

    public override void OnUpdate()
    {

    }

    public override void OnSleep()
    {

    }

    private MinionIA GetMinionWithLessLife()
    {
        MinionIA minionForReturn;

        List<GameObject> listMinions = _heroScript.GetMinionsList;


        if (listMinions.Count > 0)
        {
            minionForReturn = listMinions.FirstOrDefault(x => x.GetComponent<MinionIA>()).GetComponent<MinionIA>();

            if (minionForReturn != null)
            {
                foreach (GameObject objItem in listMinions)
                {
                    if (objItem.GetComponent<MinionIA>())
                    {
                        if (objItem.gameObject.GetComponent<MinionIA>().Life <= minionForReturn.Life)
                        {
                            minionForReturn = objItem.gameObject.GetComponent<MinionIA>();
                        };
                    }
                }
            }
        }
        else
        {
            minionForReturn = null;
        }


        return minionForReturn;
    }

}
