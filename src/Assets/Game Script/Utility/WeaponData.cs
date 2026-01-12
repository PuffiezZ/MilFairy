using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Inventory/New Weapon Data")]
public class WeaponData : ItemData
{
    [Header("Combat Stats")]
    public float damage;
    public float attackRange = 2f;
    public float attackSpeed = 1f; // คูลดาวน์ระหว่างการโจมตี

    [Header("Visuals & Animation")]
    public GameObject weaponPrefab; // ตัว Model ที่จะเอาไปติดที่มือ
    public int animationSetIndex; // ID ของท่าโจมตี (ส่งให้ Animator)
    public Sprite weaponSprite;

    public UtilityDev.WeaponType weaponType;
}
