using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TransitionPoint : MonoBehaviour
{
    //同场景还是异场景
    public enum TransitionType
    {
        SameScene,DifferentScene
    }

    [Header("Transition Info")]
    public string sceneName;
    public TransitionType transitionType;
    public TransitionDestination.DestinationTag destinationTag;//目标点

    private bool canTrans;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)&&canTrans)
        {
            PlayerController.Instance.enemy = 0;
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    //到达传送点血条回满,存档，可传送状态
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = true;
            PlayerController.Instance.characterStats.CurrentHealth = PlayerController.Instance.characterStats.MaxHealth;
            SaveManager.Instance.SavePlayerData();
           
        }
            
    }
    //离开传送点，不可传送状态
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false;
    }

}
