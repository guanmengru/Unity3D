using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    string sceneName;

    public string SceneName
    {
        //获取存档中人物场景
        get { return PlayerPrefs.GetString(sceneName);}
    }
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        //返回主菜单
        if(Input.GetKeyDown(KeyCode.Escape)&& SceneManager.GetActiveScene().name!="FantasyMain")
        {
            SceneController.Instance.TransitionToMain();
        }
        
    }
    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
        Save(GameManager.Instance.playerStats.attackData, GameManager.Instance.playerStats.attackData.name);
        Save(InventoryManager.Instance.InventoryData, InventoryManager.Instance.InventoryData.name);

    }
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
        Load(GameManager.Instance.playerStats.attackData, GameManager.Instance.playerStats.attackData.name);
        Load(InventoryManager.Instance.InventoryData, InventoryManager.Instance.InventoryData.name);
    }

    //保存数据
    public void Save(object data,string key)
    {
        var jsonData = JsonUtility.ToJson(data,true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);

        //保存玩家坐标
        PlayerPrefs.SetFloat("playerX",GameManager.Instance.playerStats.gameObject.transform.position.x);
        PlayerPrefs.SetFloat("playerY",GameManager.Instance.playerStats.gameObject.transform.position.y);
        PlayerPrefs.SetFloat("playerZ",GameManager.Instance.playerStats.gameObject.transform.position.z); 

        //保存玩家方向
        PlayerPrefs.SetFloat("playerQX",GameManager.Instance.playerStats.gameObject.transform.rotation.x);
        PlayerPrefs.SetFloat("playerQY",GameManager.Instance.playerStats.gameObject.transform.rotation.y);
        PlayerPrefs.SetFloat("playerQZ",GameManager.Instance.playerStats.gameObject.transform.rotation.z);  

        PlayerPrefs.Save();
    }
    
    //读取数据
    public void Load(object data, string key)
    {
        if(PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}
