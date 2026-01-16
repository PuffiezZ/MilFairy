using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponScript
{
    [Header("Melee Weapon Raycast")]
    [SerializeField] private Transform bladeBase;
    [SerializeField] private Transform bladeTip;
    [SerializeField] private float swordRadius = 0.3f;
}
