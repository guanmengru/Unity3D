using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStats { HitPlayer, HitEnemy, HitNothing }
    private Rigidbody rb;
    public RockStats rockStats;
    [Header("Basic Setting")]
    public float force;
    public GameObject target;
    public Vector3 direction;
    public int damage;
    public GameObject breakEffect;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one*3;
        rockStats = RockStats.HitPlayer;
        FlyToTarget();
    }
    private void FixedUpdate()
    {
        
        if(rb.velocity.sqrMagnitude<3)
        {
            rockStats = RockStats.HitNothing;
        }
    }

    //石头飞向目标
    public void FlyToTarget()
    {
        if (target == null)
            target = FindObjectOfType<PlayerController>().gameObject;
        direction = (target.transform.position - transform.position+Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
        Destroy(gameObject, 10f);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch(rockStats)
        {
            case RockStats.HitPlayer:
                if(other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force * 0.6f;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Cry");
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());

                    rockStats = RockStats.HitNothing;
                }
                break;
            case RockStats.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position,Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }

    
}
