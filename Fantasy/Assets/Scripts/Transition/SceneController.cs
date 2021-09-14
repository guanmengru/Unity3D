using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefab;
    GameObject player;
    NavMeshAgent playerAgent;
    public AsyncOperation operation;//异步操作协同程序
    public SceneFader sceneFaderPrefab;//渐入渐出的UI
    public bool fadeFinished;//为了防止不断重复播放返回主菜单


    protected override void Awake()
    {
        base.Awake();
        fadeFinished=true;
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        GameManager.Instance.AddObserver(this);
    }

    //传送
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch(transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;

            case TransitionPoint.TransitionType.DifferentScene:
                LoadManager.Instance.LoadNextLevel();
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                
                break;
        }
    }

    //传送
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        SaveManager.Instance.SavePlayerData();//存档
        if(SceneManager.GetActiveScene().name!=sceneName)
        {

            yield return operation=SceneManager.LoadSceneAsync(sceneName);

            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);

            //关闭进度条页面
            LoadManager.Instance.loadScreen.SetActive(false);

            SaveManager.Instance.LoadPlayerData();//读档

            yield break;
        }
        else
        {
            //渐入渐出
            SceneFader fade= Instantiate(sceneFaderPrefab);
            yield return StartCoroutine(fade.FadeOut(2f));

            //传送时禁止玩家移动防止位置不对
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;
            
            yield return StartCoroutine(fade.FadeIn(2f));

            yield return null;
        }
        
    }

    //获取传送终点
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrance = FindObjectsOfType<TransitionDestination>();
        for(int i=0;i<entrance.Length;i++)
        {
            if (entrance[i].destinationTag == destinationTag)
                return entrance[i];
        }
        return null;
    }
    
    //第一次进入游戏
    public void TransitionToFirstLevel()
    {
        
        StartCoroutine(FirstLoadlevel("Fantasy"));
        
    }
    //读档
    public void TransitionToLoadGame()
    {
        StartCoroutine(Loadlevel(SaveManager.Instance.SceneName));
    }
    //菜单页面
    public void TransitionToMain()
    {
        if (Time.timeScale != 1)
            Time.timeScale = 1;
        if(BagController.Instance.openBag)
        {
            BagController.Instance.openBag = false;
            BagController.Instance.CloseBag();
        }
        StartCoroutine(LoadMain());
    }



    //第一次进入游戏
    IEnumerator FirstLoadlevel(string scene)
    {
        if(scene!="")
        {
            yield return operation= SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position,  GameManager.Instance.PlayerQuaternion());
            
            //关闭进度条页面
            LoadManager.Instance.loadScreen.SetActive(false);
            //保存数据
            SaveManager.Instance.SavePlayerData();
            yield break;
        }
            
    }

    //读档
    IEnumerator Loadlevel(string scene)
    {
        if(scene!="")
        {
            yield return operation=SceneManager.LoadSceneAsync(scene);

            yield return player = Instantiate(playerPrefab, GameManager.Instance.PlayerPosition(), GameManager.Instance.GetEntrance().rotation);
            //关闭进度条页面
            LoadManager.Instance.loadScreen.SetActive(false);
            //存档
            SaveManager.Instance.SavePlayerData();
            yield break;
        }
        else
        {
            TransitionToFirstLevel();
        }
            
    }
    //菜单页面
    IEnumerator LoadMain()
    {
        Cursor.visible=true;
        SceneFader fade= Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));

        yield return SceneManager.LoadSceneAsync("FantasyMain");
        
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    //玩家死亡回到主页面
    public void EndNotify()
    {
        if(fadeFinished)
        {
            Cursor.visible = true;
            fadeFinished=false;
            StartCoroutine(LoadMain());
        }
    }
}
