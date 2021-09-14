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
    public AsyncOperation operation;//�첽����Эͬ����
    public SceneFader sceneFaderPrefab;//���뽥����UI
    public bool fadeFinished;//Ϊ�˷�ֹ�����ظ����ŷ������˵�


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

    //����
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

    //����
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        SaveManager.Instance.SavePlayerData();//�浵
        if(SceneManager.GetActiveScene().name!=sceneName)
        {

            yield return operation=SceneManager.LoadSceneAsync(sceneName);

            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);

            //�رս�����ҳ��
            LoadManager.Instance.loadScreen.SetActive(false);

            SaveManager.Instance.LoadPlayerData();//����

            yield break;
        }
        else
        {
            //���뽥��
            SceneFader fade= Instantiate(sceneFaderPrefab);
            yield return StartCoroutine(fade.FadeOut(2f));

            //����ʱ��ֹ����ƶ���ֹλ�ò���
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;
            
            yield return StartCoroutine(fade.FadeIn(2f));

            yield return null;
        }
        
    }

    //��ȡ�����յ�
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
    
    //��һ�ν�����Ϸ
    public void TransitionToFirstLevel()
    {
        
        StartCoroutine(FirstLoadlevel("Fantasy"));
        
    }
    //����
    public void TransitionToLoadGame()
    {
        StartCoroutine(Loadlevel(SaveManager.Instance.SceneName));
    }
    //�˵�ҳ��
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



    //��һ�ν�����Ϸ
    IEnumerator FirstLoadlevel(string scene)
    {
        if(scene!="")
        {
            yield return operation= SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position,  GameManager.Instance.PlayerQuaternion());
            
            //�رս�����ҳ��
            LoadManager.Instance.loadScreen.SetActive(false);
            //��������
            SaveManager.Instance.SavePlayerData();
            yield break;
        }
            
    }

    //����
    IEnumerator Loadlevel(string scene)
    {
        if(scene!="")
        {
            yield return operation=SceneManager.LoadSceneAsync(scene);

            yield return player = Instantiate(playerPrefab, GameManager.Instance.PlayerPosition(), GameManager.Instance.GetEntrance().rotation);
            //�رս�����ҳ��
            LoadManager.Instance.loadScreen.SetActive(false);
            //�浵
            SaveManager.Instance.SavePlayerData();
            yield break;
        }
        else
        {
            TransitionToFirstLevel();
        }
            
    }
    //�˵�ҳ��
    IEnumerator LoadMain()
    {
        Cursor.visible=true;
        SceneFader fade= Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));

        yield return SceneManager.LoadSceneAsync("FantasyMain");
        
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    //��������ص���ҳ��
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
