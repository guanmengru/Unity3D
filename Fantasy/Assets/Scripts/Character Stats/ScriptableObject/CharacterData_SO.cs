using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    public bool isLevel;

    [Header("Stats Info")]
    public int maxHealth;//最大血量
    public int currentHealth;//当前血量
    public int baseDefence;//基础防御
    public int currentDefence;//当前防御
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

    //升级经验值
    public void UpdateExp()
    {
        if (currentExp >= baseExp)
        {
            LevelUp();
            isLevel = true;
        }                
    }

    //升级属性提高
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
