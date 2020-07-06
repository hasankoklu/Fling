using ES3Types;
using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class AudioDisplay : MonoBehaviour
{

    public static AudioDisplay instance;

    public AudioClip MainMusic;
    public AudioClip AmbianceMusic;
    public AudioClip OnHitMusic;

    public List<AudioClip> FinishMusicList;
    public List<AudioClip> GrumbleMusicList;
    public List<AudioClip> SlideMusicList;
    public List<AudioClip> ScreamMusicList;

    [HideInInspector]
    public AudioSource _audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    AudioSource mainMenuAudioSource;
    AudioSource AmbianceAudioSource;
    private void Start()
    {
        mainMenuAudioSource = gameObject.AddComponent<AudioSource>();
        mainMenuAudioSource.loop = true;
        StartCoroutine(MainMenuAudioSourceVolume());
        AmbianceAudioSource = gameObject.AddComponent<AudioSource>();
        AmbianceAudioSource.clip = AmbianceMusic;
        AmbianceAudioSource.loop = true;
        AmbianceAudioSource.volume = 0.35f;
        AmbianceAudioSource.Play();

    }

    IEnumerator MainMenuAudioSourceVolume()
    {
        mainMenuAudioSource.clip = MainMusic;
        mainMenuAudioSource.volume = 0.001f;
        mainMenuAudioSource.Play();
        while (mainMenuAudioSource.volume < 0.49f)
        {
            mainMenuAudioSource.volume = Mathf.Lerp(mainMenuAudioSource.volume, 0.5f, 0.00005f);
            yield return new WaitForEndOfFrame();
        }
    }
}
