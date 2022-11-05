using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCtrl : MonoBehaviour
{
    public List<AudioClip> sound = new List<AudioClip>();
    public AudioSource audioSource;

    private static AudioCtrl instance = null;
    public static AudioCtrl Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayClip(int i)
    {
        if (i > sound.Count) return;
        audioSource.clip = sound[i];
        audioSource.Play();
    }
}
