using UnityEngine.AI;
using UnityEngine;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 15;
    public GameObject rockPrefab;
    public Transform handpos;

    //击飞玩家
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;

            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            targetStats.TakeDamage(characterStats, targetStats);
            attackTarget.GetComponent<Animator>().SetTrigger("Cry");
        }
    }
    //丢石头
    public void ThrowRock()
    {
        if(attackTarget!=null)
        {
            //在石头人手上生成石头
            var rock = Instantiate(rockPrefab, handpos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }
}
