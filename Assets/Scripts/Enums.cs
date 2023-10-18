using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TagsForObjectsEnum
{
    Floor
}

public enum SteerBehaviourEnum
{
    seek,
    flee,
    obstaculeAvoidance,
    flocking
}

public enum StateMinionEnum
{
    move,
    attack,
    flee,
    patrol
}

public enum StateHeroEnum
{
    hyperHeal,
    moveHero,
    focusToMinion,
    allBlocking,
    random
}
