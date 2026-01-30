using System;
using UnityEngine;

[Serializable]
public struct PlayerAbilityKey : IEquatable<PlayerAbilityKey>
{
    public PlayerBase.PlayerCharacterID character;
    public string ability; // The enum name

    public PlayerAbilityKey(PlayerBase.PlayerCharacterID character, Enum abilityEnum)
    {
        this.character = character;
        this.ability = abilityEnum.ToString();
    }

    public override int GetHashCode() => HashCode.Combine(character, ability);
    public override string ToString() => $"{character}_{ability}";
    public bool Equals(PlayerAbilityKey other) => character == other.character && ability == other.ability;
    public override bool Equals(object obj) => obj is PlayerAbilityKey other && Equals(other);
}


