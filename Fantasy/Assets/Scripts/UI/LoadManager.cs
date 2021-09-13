using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//加载进度条
public class LoadManager : Singleton<LoadManager>
{
    public GameObject loadScreen;
    public Slider slider;
    public Text text,text2;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    //进度条页面
    public void LoadNextLevel()
    {
        StartCoroutine(LoadTheScene());
    }

    IEnumerator LoadTheScene()
    {
        //进度条页面开启
        loadScreen.SetActive(true);

        while(SceneController.Instance.operation==null)
            yield return null;

        //不激活新场景    
        SceneController.Instance.operation.allowSceneActivation=false;

        while(!SceneController.Instance.operation.isDone)
        {
            slider.value=SceneController.Instance.operation.progress;
            text.text=slider.value*100+"%";
            text2.text="Waiting";
            if(SceneController.Instance.operation.progress>=0.9f)
            {
                slider.value=1f;
                SceneController.Instance.operation.allowSceneActivation=true;
            }
            yield return null;
        }
    }
    
}
