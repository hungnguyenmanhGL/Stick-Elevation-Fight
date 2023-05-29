using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private bool sfxOn = true;

    public void Play(AudioClip requestedAudio) {
        audioSource.PlayOneShot(requestedAudio);
    }

    public void ToggleSfx() {
        sfxOn = !sfxOn;
        //Debug.Log(string.Format("SFX {0}", sfxOn));
        if (sfxOn) audioSource.volume = 1;
        else audioSource.volume = 0;
    }
}
