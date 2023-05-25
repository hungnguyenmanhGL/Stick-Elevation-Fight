using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> audioList = new List<AudioClip>();


    public void Play(AudioClip requestedAudio) {
        Debug.Log(requestedAudio.name);
        foreach (AudioClip audio in audioList) {
            if (audio.name.Equals(requestedAudio.name)) {
                Debug.Log("found");
                audioSource.clip = audio;
                audioSource.Play();
            }
        }
    }
}
