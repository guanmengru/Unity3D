using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家不能移动
public class PlayerStop : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController.Instance.playerstates = PlayerController.PlayerStates.isAnim;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController.Instance.playerstates = PlayerController.PlayerStates.isAnim;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController.Instance.playerstates = PlayerController.PlayerStates.isMove;
    }
}
