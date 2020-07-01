using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDisplay : MonoBehaviour
{

    public static AudioDisplay instance;

    public AudioClip mainMusic;
    public AudioClip onHitMusic;

    public List<AudioClip> FinishMusicList;

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
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = mainMusic;
        _audioSource.volume = 0.1f;
        _audioSource.Play(0);
    }

}
