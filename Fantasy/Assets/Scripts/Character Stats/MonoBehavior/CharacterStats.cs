using UnityEngine;
using System;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdataHealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    public AttackData_SO templateAttack;
    private bool init;

    [HideInInspector]
    public bool isCritical;


    private void Awake()
    {
        //��¡ģ���Է����ݻ�ͨ
        if (templateData != null)
            characterData = Instantiate(templateData);
        if (templateAttack != null)
            attackData = Instantiate(templateAttack);

    }
    private void Update()
    {
        if(PlayerController.Instance!=null)
        {
            if (!init && gameObject.CompareTag("Enemy"))
            {
                Level();
                init = true;
            }
        }
        
    }
    #region Read from Data_SO
    public int MaxHealth
    {
        get{    if (characterData != null)  return characterData.maxHealth; else return 0; }
        set{    characterData.maxHealth = value;}
    }
    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }
    public int BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat

    //��Ѫ
    public void TakeDamage(CharacterStats attacker,CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if(attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }

        UpdataHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //������������Ҿ���ֵ���
        if (CurrentHealth <= 0)
            attacker.characterData.currentExp += characterData.currentKillPoint;
            
    }
    //����ʯͷ
    public void TakeDamage(int damage,CharacterStats defener)
    {
        int currentDamage = Mathf.Max(damage-defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdataHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //ʯͷ����������Ҿ���ֵ���
        if (CurrentHealth <= 0)
            GameManager.Instance.playerStats.characterData.currentExp += characterData.currentKillPoint;
    }

    //�����˺�
    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        //�����˺�
        if(isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
        }

        return (int)coreDamage;
    }

    #endregion

    //����������ȼ����
    public void Level()
    {
        int playerLevel = PlayerController.Instance.characterStats.characterData.currentlevel;
        characterData.currentlevel = UnityEngine.Random.Range(playerLevel - 3, playerLevel + 3);
        if (characterData.currentlevel < 1) { characterData.currentlevel = 1; }
        characterData.currentKillPoint = (int)(characterData.killPoint * (characterData.currentlevel-1)*0.5f)+ characterData.killPoint;
        characterData.currentDefence+= characterData.currentlevel-1;
        characterData.maxHealth= (int)(characterData.maxHealth * (characterData.currentlevel - 1)*0.1+ characterData.maxHealth);
        attackData.minDamage += characterData.currentlevel-1;
        attackData.maxDamage += characterData.currentlevel-1;
    }
    
}
