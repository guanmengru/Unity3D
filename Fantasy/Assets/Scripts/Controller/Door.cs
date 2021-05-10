using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //回血存档
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.Instance.characterStats.CurrentHealth = PlayerController.Instance.characterStats.MaxHealth;
            SaveManager.Instance.SavePlayerData();
           
        }
            
    }
}
