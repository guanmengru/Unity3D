using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

    public class CameraController : Singleton<CameraController>
{
    private float maxh = 20;
    private float minh = -15;
    public Vector3 thisAngle = new Vector3();

    CinemachineVirtualCamera cam;
    
    
    // Start is called before the first frame update
    void Start()
    {
        thisAngle = this.transform.eulerAngles;
    }
    // Update is called once per frame
    protected override void Awake()
    {
        base.Awake();
        cam=GetComponent<CinemachineVirtualCamera>();
    }
    void Update()
    {
        CameraRotate();
    }

    //摄像机随鼠标旋转
    void CameraRotate()
    {
        float y = Input.GetAxis("Mouse X");
        float x = Input.GetAxis("Mouse Y");
        thisAngle.y += y * 2;
        if (thisAngle.x - x * 2 < maxh && thisAngle.x - x * 2 > minh)
        {
            thisAngle.x -= x * 2;
        }
            
        this.transform.eulerAngles = thisAngle;
    }
    
}


