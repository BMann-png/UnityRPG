using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int hitpoints = 0;
    public int HitPoints { get { return hitpoints; } set { } }

    public int Damage(int damageDealt)
    {
        hitpoints -= damageDealt;

        return hitpoints;
    }
}
