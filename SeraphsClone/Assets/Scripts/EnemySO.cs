using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName ="Enemy", menuName = "ScriptableObject/Enemy")]
public class EnemySO : ScriptableObject
{
    public GameObject bullet_prefab;
    public GameObject EnemyAppearence;
    public float height;
    public float distance;
    public float healthMax;
    public float cooldown;

}
