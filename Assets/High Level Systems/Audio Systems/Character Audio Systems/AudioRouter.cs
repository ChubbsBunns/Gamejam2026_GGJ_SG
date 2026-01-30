using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioRouter : MonoBehaviour
{
    public static AudioRouter Instance;

    [Header("One Shot Pool")]
    public AudioSource audioSourcePrefab;
    //private List<AudioSource> pool = new();
    private readonly Queue<AudioSource> pool = new();

    [Header("Looping Pool")]
    
    private AudioSource loopSource;

    [SerializeField] private AudioSource audioSourcePoolPrefab;
    [SerializeField] private int initialPoolSize = 10;
    

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.loop = true;        
    }

    public void PlayOneShot(AudioClip[] clips, Vector3 position, float volume = 1f)
    {
        if (clips == null || clips.Length == 0) return;

        var src = GetPooledSource();
        src.transform.position = position;
        src.clip = clips[Random.Range(0, clips.Length)];
        src.volume = volume;
        src.loop = false;

        src.Play();
        StartCoroutine(ReturnAfterPlay(src));
    }

    //
    // PLAY ONE SHOT HELPERS
    //

    AudioSource GetPooledSource()
    {
        if (pool.Count == 0)
            CreateNewSource();

        var src = pool.Dequeue();
        src.gameObject.SetActive(true);
        return src;
    }

    void ReturnToPool(AudioSource src)
    {
        src.Stop();
        src.clip = null;
        src.loop = false;
        src.gameObject.SetActive(false);
        pool.Enqueue(src);
    }

    //
    // LOOPING SOURCE HELPER
    //
    public void StartLoop(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        loopSource.clip = clip;
        loopSource.volume = volume;

        if (!loopSource.isPlaying)
            loopSource.Play();
    } 

    public void StopLoop()
    {
        if (!loopSource.isPlaying) return;

        loopSource.Stop();
        loopSource.clip = null;
    }

    IEnumerator ReturnAfterPlay(AudioSource src)
    {
        yield return new WaitWhile(() => src.isPlaying);
        ReturnToPool(src);
    }

    public AudioSource RequestLoopSource()
    {
        var src = GetPooledSource();
        src.loop = true;
        return src;
    }

    public void ReleaseLoopSource(AudioSource src)
    {
        if (src == null) return;
        ReturnToPool(src);
    }

    //Source Creation
    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
            CreateNewSource();
    }

    AudioSource CreateNewSource()
    {
        var src = Instantiate(audioSourcePrefab, transform);
        src.gameObject.SetActive(false);
        pool.Enqueue(src);
        return src;
    }
}
