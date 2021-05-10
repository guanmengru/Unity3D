using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    public bool isLevel;

    [Header("Stats Info")]
    public int maxHealth;//���Ѫ��
    public int currentHealth;//��ǰѪ��
    public int baseDefence;//��������
    public int currentDefence;//��ǰ����
    [Header("Level")]
    public int currentlevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;
    [Header("Kill")]
    public int killPoint;
    public int currentKillPoint;

    

    public float LevelMultiplier
    {
        get { return 1 + (currentlevel - 1) * levelBuff; }
    }

    //��������ֵ
    public void UpdateExp()
    {
        if (currentExp >= baseExp)
        {
            LevelUp();
            isLevel = true;
        }                
    }

    //�����������
    private void LevelUp()
    {
        currentlevel = Mathf.Clamp(currentlevel + 1,0,maxLevel);
        currentExp = currentExp - baseExp;
        baseExp += (int)(baseExp * LevelMultiplier);

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;       
        currentDefence++;

    }
}
