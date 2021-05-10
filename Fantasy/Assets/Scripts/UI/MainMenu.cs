using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;
    

    private void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        newGameBtn.onClick.AddListener(NewGame);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
    }
    //新游戏
    void NewGame()
    {
        //删除存档
        PlayerPrefs.DeleteAll();
        LoadManager.Instance.LoadNextLevel();
        SceneController.Instance.TransitionToFirstLevel();
        SceneController.Instance.fadeFinished=true;

    }
    //继续游戏
    void ContinueGame()
    {
        LoadManager.Instance.LoadNextLevel();
        SceneController.Instance.TransitionToLoadGame();
        SceneController.Instance.fadeFinished=true;
    }

    //退出游戏
    void QuitGame()
    {
        Application.Quit();
    }
}
