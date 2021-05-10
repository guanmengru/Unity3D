using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController :Singleton<PlayerController>
{

    List<string> animlist = new List<string>(new string[] { "Fight", "Fight1", "Fight2", "Fight3" });//攻击动画

    public PlayerStates playerstates;

    private int combonum;//连招计数
    private float lastAttackTime;//cd
    private float time;
    private float jumpSpeed = 8f;
    private float gravity =20f;
    private  float speed = 6f;
    private float turnSmoothTime = 0.1f;
    private float jumpTime;
    private float turnSmoothVelocity;
    public float sightRadius;
    public int enemy;//追击玩家怪数量
    [HideInInspector]
    public bool isHit;//玩家处于受伤动画

    private Vector3 dir;
    private bool isJump;
    public bool isDead;

    private CharacterController controller;
    private Animator anim;
    public Transform cam;

    [HideInInspector]
    public CharacterStats characterStats;

    
    protected override void Awake()
    {      
        Cursor.visible = false;
        base.Awake();

        enemy = 0;
        isJump = false;
        playerstates = PlayerStates.isMove;

        cam = Camera.main.transform;

        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

    }
    private void OnEnable()
    {
        GameManager.Instance.RigisterPlayer(characterStats);
        
        SaveManager.Instance.LoadPlayerData();
    }

    void Update()
    {
        //判断升级
        characterStats.characterData.UpdateExp();
        if(characterStats.characterData.isLevel)
        {
            characterStats.attackData.minDamage += 3;
            characterStats.attackData.maxDamage += 3;
            characterStats.characterData.isLevel = false;
        }

        //玩家死亡播放死亡动画并广播
        if (characterStats.CurrentHealth == 0)
            isDead = true;
        anim.SetBool("Die", isDead);

        if(isDead)
        {
            GameManager.Instance.NotifyObservers();
        }

        //音乐切换
        if (enemy!=0)
            MusicController.Instance.mymusic = MusicController.musicStats.isFight;//播放战斗音乐
        else
            MusicController.Instance.mymusic = MusicController.musicStats.isIdel; //播放普通音乐

        PlayMove();
        FightTime();

        jumpTime -= Time.deltaTime;
        lastAttackTime -= Time.deltaTime;

        if (playerstates!=PlayerStates.isFight&& jumpTime < 0) { playerstates = PlayerStates.isMove; isJump = false; }
    }
    //移动，跳跃，发呆
    public void PlayMove()
    {
        StopAllCoroutines();

        if (isDead) return;

        //获取键盘wasd
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");   
        
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //跳跃
        if (Input.GetKeyDown(KeyCode.Space) && playerstates != PlayerStates.isJump)
        {
            if (horizontal != 0 || vertical != 0)
                dir = new Vector3(this.transform.forward.x, 3.5f, this.transform.forward.z);
            else
                dir = new Vector3(0, 3.5f, 0);

            jumpTime = 1.7f;
            playerstates = PlayerStates.isJump;
            isJump = true;
            anim.SetTrigger("Jump");          
        }
        if(isJump)
        {                    
            dir.y = dir.y - (gravity * Time.deltaTime);//重力作用
            controller.Move(dir.normalized*Time.deltaTime*jumpSpeed);
        }

        //移动
        if (direction.magnitude >= 0.1f&&playerstates==PlayerStates.isMove)
        {
            anim.SetFloat("Speed", speed);

            //疾跑
            if (Input.GetMouseButton(1))
            {
                speed = 10f;
                anim.speed = 1.5f;//动画播放加速
            }
            else
            {
                speed = 6f;
                anim.speed = 1f;
            }
            
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);//转向

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.SimpleMove(moveDir.normalized * speed);
        }

        //发呆
        else
        {
            anim.SetFloat("Speed", 0);
        }
    }

    //攻击动画
    public void FightTime()
    {
        if (Input.GetMouseButtonDown(0) && combonum < 4&&lastAttackTime<0.3&&!isHit)
        {
            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
            playerstates = PlayerStates.isFight;
            anim.SetTrigger(animlist[combonum]);
            combonum++;
            lastAttackTime = characterStats.attackData.coolDown;
            
        }
        if (combonum > 0)
        {
            //当超过时间间隔就回到初始状态
            if (lastAttackTime<0)
            {
                playerstates = PlayerStates.isMove;
                anim.SetTrigger("Reset");
                combonum = 0;
            }
        }
        //当到达最后一个动作的时候重置
        if (combonum == 4)
        {
            playerstates = PlayerStates.isMove;
            anim.SetTrigger("Reset");
            combonum = 0;
        }
    }

    //人物状态
    public enum PlayerStates
    {
        isAnim,//玩家处于不能动的动画状态
        isFight,//攻击状态
        isMove,//可以移动状态
        isJump//跳跃状态
    }

    //画图玩家的攻击范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
    //攻击范围内敌人掉血
    void SowdAttackEnemy()
    {
        List<GameObject> allEnemy = new List<GameObject>();
        if (isDead) return;
        StopAllCoroutines();
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        //统计攻击范围内所有敌人，石头
        foreach (var target in colliders)
        {
            if (target.CompareTag("Enemy"))
            {
                allEnemy.Add(target.gameObject);               
            }
            else if(target.CompareTag("Rock"))
            {
                allEnemy.Add(target.gameObject);                   
            }
         }
         //范围内敌人掉血，石头飞出
        foreach(var target in allEnemy)
        {
            if (target.CompareTag("Enemy"))
            {
                var targetStats = target.GetComponent<CharacterStats>();
                targetStats.TakeDamage(characterStats, targetStats);
                transform.LookAt(target.transform);
                
            }
            else if (target.CompareTag("Rock") )
            {
                target.gameObject.GetComponent<Rock>().rockStats = Rock.RockStats.HitEnemy;
                target.gameObject.GetComponent<Rigidbody>().velocity = Vector3.one * 3;
                target.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);

            }
        }
;
    }
}
