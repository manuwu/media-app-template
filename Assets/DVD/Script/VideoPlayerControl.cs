using System;
using System.Collections;
using System.Collections.Generic;
using UMP;
using UnityEngine;
using UnityEngine.UI;

public class VideoPlayerControl : SingletonMB<VideoPlayerControl> {

    public UniversalMediaPlayer UmpPlayer;
    public GameObject playerPanel;
    public Action QuitPlayer;
    [SerializeField]
    private GameObject pauseBtn;
    [SerializeField]
    private GameObject playBtn;
    
    public GameObject playerRoot;
    public Slider Position;
    private void Start()
    {
        UmpPlayer = GetComponent<UniversalMediaPlayer>();
    }

    public void   Pause()
    {
        UmpPlayer.Pause();
        playBtn.gameObject.SetActive(true);
        pauseBtn.gameObject.SetActive(false);
    }

    public void  Play()
    {
        UmpPlayer.Play();
        playBtn.gameObject.SetActive(false);
        pauseBtn.gameObject.SetActive(true);
    }

    public void Back()
    {
        playerRoot.SetActive(false);
        if (QuitPlayer != null)
            QuitPlayer();
    }

    public void UpdateProcess()
    {
        Position.value = UmpPlayer.Position;
    }
}
