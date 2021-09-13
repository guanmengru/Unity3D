using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;//��������
    public float skillRange;//Զ�̾���
    public float coolDown;//cd
    public int minDamage;//��С������ֵ
    public int maxDamage;//��󹥻���ֵ
    public float criticalMultiplier;//�����ӳ�
    public float criticalChance;//������

    //װ���ӳ�
    public void ApplyWeaponData(AttackData_SO weapon)
    {
        attackRange += weapon.attackRange;
        skillRange += weapon.skillRange;
        minDamage += weapon.minDamage;
        maxDamage += weapon.maxDamage;
        criticalMultiplier += weapon.criticalMultiplier;
        criticalChance += weapon.criticalChance;
    }
    //ж��װ�����ӳ���ʧ
    public void RemoveWeaponData(AttackData_SO weapon)
    {
        attackRange -= weapon.attackRange;
        skillRange -= weapon.skillRange;
        minDamage -= weapon.minDamage;
        maxDamage -= weapon.maxDamage;
        criticalMultiplier -= weapon.criticalMultiplier;
        criticalChance -= weapon.criticalChance;
    }

}
