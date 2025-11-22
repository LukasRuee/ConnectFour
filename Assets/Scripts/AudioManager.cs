using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource sfx;
    public AudioClip drop;
    public AudioClip win;
    public AudioClip error;
    public AudioClip crowdCheer;

    void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }

    public void PlayDrop() { if (sfx && drop) sfx.PlayOneShot(drop); }
    public void PlayWin() { if (sfx && win) sfx.PlayOneShot(win); }
    public void PlayError() { if (sfx && error) sfx.PlayOneShot(error); }
    public void Cheer() { if (sfx && crowdCheer) sfx.PlayOneShot(crowdCheer); }
}