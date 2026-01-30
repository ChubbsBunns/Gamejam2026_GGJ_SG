using UnityEngine;
using System.Collections.Generic;

public class CharacterAudioEmitter : MonoBehaviour
{
    public CharacterAudioProfile profile;
    Dictionary<CharacterAudioEvent, AudioSource> activeLoops = new();
    AudioSource activeLoop;

    //ONESHOT HELPER

    public void Play(CharacterAudioEvent evt)
    {
        if (profile == null) return;

        var entry = profile.Get(evt);
        if (entry == null || entry.clips.Length == 0) return;

        AudioRouter.Instance.PlayOneShot(
            entry.clips,
            transform.position,
            entry.volume
        );
    }

    // LOOP HELPERS
    public void StartLoop(CharacterAudioEvent evt)
    {
        if (activeLoops.ContainsKey(evt))
            return; // this loop is already playing

        var entry = profile.Get(evt);
        if (entry == null || entry.clips.Length == 0) return;

        var src = AudioRouter.Instance.RequestLoopSource();
        src.clip = entry.clips[Random.Range(0, entry.clips.Length)];
        src.volume = entry.volume;
        src.transform.position = transform.position;

        src.Play();
        activeLoops.Add(evt, src);
    }

    public void StopLoop(CharacterAudioEvent evt)
    {
        if (!activeLoops.TryGetValue(evt, out var src))
            return;

        AudioRouter.Instance.ReleaseLoopSource(src);
        activeLoops.Remove(evt);
    }

    void OnDisable()
    {
        StopAllLoops();
    }

    public void StopAllLoops()
    {
        foreach (var src in activeLoops.Values)
            AudioRouter.Instance.ReleaseLoopSource(src);

        activeLoops.Clear();
    }

}
