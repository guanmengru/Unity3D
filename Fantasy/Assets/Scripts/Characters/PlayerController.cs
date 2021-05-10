using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController :Singleton<PlayerController>
{

    List<string> animlist = new List<string>(new string[] { "Fight", "Fight1", "Fight2", "Fight3" });//��������

    public PlayerStates playerstates;

    private int combonum;//���м���
    private float lastAttackTime;//cd
    private float time;
    private float jumpSpeed = 8f;
    private float gravity =20f;
    private  float speed = 6f;
    private float turnSmoothTime = 0.1f;
    private float jumpTime;
    private float turnSmoothVelocity;
    public float sightRadius;
    public int enemy;//׷����ҹ�����
    [HideInInspector]
    public bool isHit;//��Ҵ������˶���

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
        //�ж�����
        characterStats.characterData.UpdateExp();
        if(characterStats.characterData.isLevel)
        {
            characterStats.attackData.minDamage += 3;
            characterStats.attackData.maxDamage += 3;
            characterStats.characterData.isLevel = false;
        }

        //����������������������㲥
        if (characterStats.CurrentHealth == 0)
            isDead = true;
        anim.SetBool("Die", isDead);

        if(isDead)
        {
            GameManager.Instance.NotifyObservers();
        }

        //�����л�
        if (enemy!=0)
            MusicController.Instance.mymusic = MusicController.musicStats.isFight;//����ս������
        else
            MusicController.Instance.mymusic = MusicController.musicStats.isIdel; //������ͨ����

        PlayMove();
        FightTime();

        jumpTime -= Time.deltaTime;
        lastAttackTime -= Time.deltaTime;

        if (playerstates!=PlayerStates.isFight&& jumpTime < 0) { playerstates = PlayerStates.isMove; isJump = false; }
    }
    //�ƶ�����Ծ������
    public void PlayMove()
    {
        StopAllCoroutines();

        if (isDead) return;

        //��ȡ����wasd
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");   
        
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //��Ծ
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
            dir.y = dir.y - (gravity * Time.deltaTime);//��������
            controller.Move(dir.normalized*Time.deltaTime*jumpSpeed);
        }

        //�ƶ�
        if (direction.magnitude >= 0.1f&&playerstates==PlayerStates.isMove)
        {
            anim.SetFloat("Speed", speed);

            //����
            if (Input.GetMouseButton(1))
            {
                speed = 10f;
                anim.speed = 1.5f;//�������ż���
            }
            else
            {
                speed = 6f;
                anim.speed = 1f;
            }
            
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);//ת��

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.SimpleMove(moveDir.normalized * speed);
        }

        //����
        else
        {
            anim.SetFloat("Speed", 0);
        }
    }

    //��������
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
            //������ʱ�����ͻص���ʼ״̬
            if (lastAttackTime<0)
            {
                playerstates = PlayerStates.isMove;
                anim.SetTrigger("Reset");
                combonum = 0;
            }
        }
        //���������һ��������ʱ������
        if (combonum == 4)
        {
            playerstates = PlayerStates.isMove;
            anim.SetTrigger("Reset");
            combonum = 0;
        }
    }

    //����״̬
    public enum PlayerStates
    {
        isAnim,//��Ҵ��ڲ��ܶ��Ķ���״̬
        isFight,//����״̬
        isMove,//�����ƶ�״̬
        isJump//��Ծ״̬
    }

    //��ͼ��ҵĹ�����Χ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
    //������Χ�ڵ��˵�Ѫ
    void SowdAttackEnemy()
    {
        List<GameObject> allEnemy = new List<GameObject>();
        if (isDead) return;
        StopAllCoroutines();
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        //ͳ�ƹ�����Χ�����е��ˣ�ʯͷ
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
         //��Χ�ڵ��˵�Ѫ��ʯͷ�ɳ�
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
