using System.Collections;
using System.Collections.Generic;
using UnityEngine;
   
[RequireComponent(typeof(CharacterStats))]

//蝙蝠移动模式，与EnemyController差不多
public class Bat : MonoBehaviour,IEndGameObserver
{
    bool index;
    private EnemyStates enemyStates;
    private Animator anim;
    protected CharacterStats characterStats;
    [Header("Basic Setting")]
    public float sightRadius;//发现范围
    protected GameObject attackTarget;//攻击对象
    public bool isGuard;//站桩
    private float speed;
    bool isWalk, isChase, isFollow;
    bool isDead;
    bool playerDead;
    public float lookAtTime;//寻找时间
    private float remainLookAtTime;
    private float lastAttackTime;//攻击cd
    private Quaternion guardRotation;

    [Header("Patrol State")]
    public float patrolRange;//巡逻范围
    private Vector3 wayPoint;
    private Vector3 guardPos;//初始坐标
    private float stoppingDistance;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        speed =5f;
        stoppingDistance=2f;
        guardRotation = transform.rotation;
        guardPos = transform.position;
        remainLookAtTime = lookAtTime;
        index = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(isGuard)
        {
            enemyStates = EnemyStates.Guard;
        }
        else
        {
            enemyStates = EnemyStates.Patrol;
            GetNewWayPoint();
        }
        GameManager.Instance.AddObserver(this);
    }

    private void OnDisable()
    {
        if (!GameManager.IsInitialized)
            return;
        GameManager.Instance.RemoveObserver(this);
    }
    // Update is called once per frame
    void Update()
    {

        if (characterStats.CurrentHealth == 0)
            isDead = true;

        if(!playerDead)
        {
            lastAttackTime -= Time.deltaTime;

            SwitchStates();
            SwitchAnimator();
        }
       


    }
    void SwitchStates()
    {
        //怪物死亡
        if (isDead)
            enemyStates = EnemyStates.Dead;

        //发现玩家进入追击状态
        else if(FindPlayer())
        {
            enemyStates = EnemyStates.Chase;
        }


        switch(enemyStates)
        {
            case EnemyStates.Guard:
                if (index)
                {
                    PlayerController.Instance.enemy--;
                    index = false;
                }

                isChase = false;
                if(transform.position!=guardPos)
                {
                    if (Vector3.Distance(guardPos , transform.position) <= stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                    else
                    {
                        isWalk = true;
                        transform.LookAt(guardPos);
                        transform.position=Vector3.MoveTowards(transform.position,guardPos,Time.deltaTime*5);
                    }
                }

                break;
            case EnemyStates.Patrol:
                if (index)
                {
                    PlayerController.Instance.enemy--;
                    index = false;
                }

                isChase = false;

                if(Vector3.Distance(wayPoint,transform.position)<=stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime >0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    transform.LookAt(wayPoint);
                    transform.position=Vector3.MoveTowards(transform.position,wayPoint,Time.deltaTime*5);
                }
                break;
            case EnemyStates.Chase:
    
                isWalk = false;
                isChase = true;

 
                if (!FindPlayer())
                {
                    
                    isFollow = false;
                    if(remainLookAtTime>0)
                    {
                        transform.position=transform.position;

                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if(isGuard)
                    {
                        enemyStates = EnemyStates.Guard;
                    }
                    else
                    {
                        enemyStates = EnemyStates.Patrol;
                    }
                }
                
                else
                {
                    if(!index)
                    {
                        PlayerController.Instance.enemy++;
                        index = true;
                    }
                    isFollow = true;
                    transform.LookAt(attackTarget.transform.position);
                    if(Vector3.Distance(transform.position,attackTarget.transform.position)>2)
                        transform.position=Vector3.MoveTowards(transform.position,attackTarget.transform.position,Time.deltaTime*speed);
                }
                //玩家是否在攻击范围内
                if(TargetInAttackRange()||TargetInSkillRange())
                {
                    transform.LookAt(attackTarget.transform.position);
                    isFollow = false;


                    if(lastAttackTime<0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;
                        
                        //判断暴击
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;

                        //攻击
                        Attack();
                    }
                }
                break;
            case EnemyStates.Dead:

                if(index)
                {
                    PlayerController.Instance.enemy--;
                    index = false;

                }
                Destroy(gameObject, 2f);
                break;
        }

    }

    //攻击
    void Attack()
    {
        transform.LookAt(attackTarget.transform);

        //近战动画
        if(TargetInAttackRange())
        {
            anim.SetTrigger("Attack");
        }
        //远程动画
        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill");
        }
    }

    //判断是否发现玩家
    bool FindPlayer()
    {

        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach(var target in colliders)
        {
            if(target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    //动画控制
    void SwitchAnimator()
    {
        anim.SetBool("Die", isDead);
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
    }

    //站桩，巡逻，追击，死亡
    public enum EnemyStates 
    { 
        Guard, 
        Patrol,
        Chase, 
        Dead 
    };
    //画出攻击范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //获取新巡逻点
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        wayPoint=randomPoint;


    }
    
    //判断玩家在近战攻击范围内
    bool TargetInAttackRange()
    {
        if (attackTarget)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }

    //判断玩家在远程攻击范围内
    bool TargetInSkillRange()
    {
        if (attackTarget)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }

    //攻击玩家
    void Hit()
    {
        if(attackTarget!=null&&transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats,targetStats);
        }
    }

    //玩家死亡怪物庆祝
    public void EndNotify()
    {
        playerDead = true;
        anim.SetBool("Win",true);
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}

