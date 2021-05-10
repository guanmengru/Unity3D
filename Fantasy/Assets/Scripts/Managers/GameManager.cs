using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;
    private CinemachineVirtualCamera followCamera; 


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }


    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;

        //虚拟相机跟随并看向玩家
        followCamera=FindObjectOfType<CinemachineVirtualCamera>();
        if(followCamera!=null)
        {
            followCamera.Follow=playerStats.transform.GetChild(4);
            followCamera.LookAt=playerStats.transform.GetChild(4);
        }
    }

    //所有观察者列表
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    //怪物死亡从列表删除
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //玩家死亡，调用EndNotify庆祝
    public void NotifyObservers()
    {
        foreach(var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
    //出生点坐标
    public Transform GetEntrance()
    {
        foreach(var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER)
            {
                return item.transform;
            }
                
        }
        return null;
    }
    //读取玩家坐标存档
    public Vector3 PlayerPosition()
    {
        Vector3 playerPosition=new Vector3();
        playerPosition.x=PlayerPrefs.GetFloat("playerX");
        playerPosition.y=PlayerPrefs.GetFloat("playerY");
        playerPosition.z=PlayerPrefs.GetFloat("playerZ");
        return playerPosition;
    }
    //读取玩家方向存档
    public Quaternion PlayerQuaternion()
    {
        Quaternion playerQuaternion=new Quaternion();
        playerQuaternion.x=PlayerPrefs.GetFloat("playerQX");
        playerQuaternion.y=PlayerPrefs.GetFloat("playerQY");
        playerQuaternion.z=PlayerPrefs.GetFloat("playerQZ");
        return playerQuaternion;
    }
    
}
