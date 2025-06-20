using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class HitscanGunData : ScriptableObject
{

    [Header("Info")]
    public new string name;
    public Boolean hitscan;

    [Header("Shooting")]
    public float damage;
    public float maxDistance;
    public float fireRate;
    public int numOfProjectiles;
    public float bloom;
    public float bloomRatioX;
    public float bloomRatioY;
    public bool useAmmo;
    public int maxAmmo;

    [Header("Projectile")]
    public float launchForce;
    public float spinSpeed;
    [Tooltip("This variable is only used for weapons like grenade launchers (NOT for normal guns)")]
    public float upwardLaunchForce;

    [Header("Unique Properties")]
    public bool trackReturnProjectiles;
}
