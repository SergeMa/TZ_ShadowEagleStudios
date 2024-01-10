using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;
using UnityEngine.TextCore.Text;

public class SplitEnemyAbility : Enemie
{
    public int SplitCount;
    public GameObject SplitCharacter;
    public override void Die()
    {
        for (int i = 0; i < SplitCount; i++)
        {
            Vector3 SplitPosition = transform.position + new Vector3(Random.Range(0, 3), 0, Random.Range(0, 3));
            SplitCharacter.GetComponent<Enemie>().Hp = 1;
            SplitCharacter.GetComponent<Enemie>().Damage = 1;
            Instantiate(SplitCharacter, SplitPosition, Quaternion.identity);
        }
        base.Die();
    }
}
