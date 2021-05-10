using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : Singleton<MusicController>
{
    public AudioClip[] music;//音乐列表

    public AudioSource audioSource;

    public bool isFight;

    public musicStats mymusic;



    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        mymusic = musicStats.isIdel;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = music[0];
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(mymusic==musicStats.isFight&&audioSource.clip!=music[1])
        {
            audioSource.clip = music[1];
            audioSource.Play();
        }
        else if(mymusic == musicStats.isIdel && audioSource.clip!=music[0])
        {
            audioSource.clip = music[0];
            audioSource.Play();
        }
        
    }
    public enum musicStats
    {
        isFight,
        isIdel
    }

}
