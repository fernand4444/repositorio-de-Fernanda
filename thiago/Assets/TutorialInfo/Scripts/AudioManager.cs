using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    private AudioSource systemSource;
    private List<AudioClip> activeSources;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {

            Destroy(gameObject);
        }
    }
    // funcoes de gerenciamento de audio 
    public void PlaySound(AudioClip clip)
    {
        systemSource.clip = clip;
        systemSource.Play();
    }
}


