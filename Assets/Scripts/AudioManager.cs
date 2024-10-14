using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip maidenSong;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = maidenSong;

        audioSource.Play();
        audioSource.volume = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
