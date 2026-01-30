using UnityEngine;
using System.Collections.Generic;

public enum CharacterAudioEvent
{
    
    AttackStart,
    AttackStartWeaponSound,
    AttackHit,
    AttackMiss,
    AttackCriticalStart,
    AttackCharge,
    ChargeStart,
    ChargeHold,
    ChargeRelease,
    Hurt,
    Death,
    Walk,
    MovementAbilityStart,
    MovementAbilityLoop,
    MovementAbilityEnd,
    MovementAbilityStart2,
    MovementAbilityLoop2,
    MovementAbilityEnd2,   
}

[System.Serializable]
public class AudioEventEntry
{
    public CharacterAudioEvent eventType;
    public AudioClip[] clips;
    public float volume = 1f;
}

[CreateAssetMenu(
    fileName = "CharacterAudioProfile",
    menuName = "Audio/Character Audio Profile"
)]
public class CharacterAudioProfile : ScriptableObject
{
    public List<AudioEventEntry> events = new();

    Dictionary<CharacterAudioEvent, AudioEventEntry> lookup;

    public AudioEventEntry Get(CharacterAudioEvent evt)
    {
        lookup ??= BuildLookup();
        lookup.TryGetValue(evt, out var entry);
        return entry;
    }

    Dictionary<CharacterAudioEvent, AudioEventEntry> BuildLookup()
    {
        var dict = new Dictionary<CharacterAudioEvent, AudioEventEntry>();
        foreach (var e in events)
        {
            if (!dict.ContainsKey(e.eventType))
                dict.Add(e.eventType, e);
        }
        return dict;
    }
}
