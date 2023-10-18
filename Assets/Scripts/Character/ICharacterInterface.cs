using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ICharacterInterface
{
    void GetDamage(int num);
    void Die();
    void AttackedFocus(GameObject obj);
}
