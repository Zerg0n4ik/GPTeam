using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [System.Serializable]
public class Sound {
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [HideInInspector] public AudioSource source;
}

    public List<Sound> sounds;
    public AudioSource musicSource;
    public AudioSource sfxSource;

void Start() {
    foreach (Sound s in sounds) {
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
    }
}
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

public void PlaySound(string name) {
    Sound s = sounds.Find(sound => sound.name == name);
    if (s != null) s.source.Play();
}

    public void PlayMusic(AudioClip music, bool loop = true)
    {
        musicSource.clip = music;
        musicSource.loop = loop;
        musicSource.Play();
    }
}