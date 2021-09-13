using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagController : Singleton<BagController>
{
    bool openBag;
    public Button exitButton;
    public new CameraController camera;
    // Start is called before the first frame update
    void Start()
    {

        exitButton.onClick.AddListener(CloseBag);
        openBag = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        OpenTheBag();
    }

    //打开背包
    void OpenTheBag()
    {
        if(Input.GetKeyDown(KeyCode.B)&&!openBag)
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            InventoryManager.Instance.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            camera.enabled = false;
            PlayerController.Instance.playerstates = PlayerController.PlayerStates.nothing;
            
            openBag = true;        
        }
    }
    //关闭背包
    void CloseBag()
    {
        Cursor.visible = true;
        InventoryManager.Instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        Time.timeScale = 1;
        camera.enabled = true;
        PlayerController.Instance.playerstates = PlayerController.PlayerStates.isMove;

        openBag = false;
       
    }
}
