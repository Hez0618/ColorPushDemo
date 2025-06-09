using UnityEngine;
using System.Collections;


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    public bool loop = false;

    public float startTime = 0f;   // 新增：从第几秒开始播放
    public float playDuration = 0f; // 新增：播放多久（0 表示播放整段）

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sounds;

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
            return;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }

        if (s.playDuration <= 0f)
        {
            // 播放完整音效
            s.source.time = s.startTime;
            s.source.Play();
        }
        else
        {
            // 播放指定区间
            StartCoroutine(PlaySegment(s));
        }
    }

    private IEnumerator PlaySegment(Sound s)
    {
        s.source.time = s.startTime;
        s.source.Play();
        yield return new WaitForSeconds(s.playDuration);
        s.source.Stop();
    }

    public void PlayOneShot(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }
        s.source.PlayOneShot(s.clip);
    }


    public void PlayWithFadeIn(string name, float fadeDuration)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        StartCoroutine(FadeIn(s, fadeDuration));
    }

    private IEnumerator FadeIn(Sound s, float duration)
    {
        s.source.volume = 0f;
        s.source.Play();
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            s.source.volume = Mathf.Lerp(0f, s.volume, t / duration);
            yield return null;
        }
        s.source.volume = s.volume;
    }

    public void Stop(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.Stop();
    }
}

