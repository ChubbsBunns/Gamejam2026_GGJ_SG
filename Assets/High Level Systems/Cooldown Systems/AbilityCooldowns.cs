using System.Collections.Generic;
using UnityEngine;

public class AbilityCooldowns : MonoBehaviour
{
    [System.Serializable]
    public class CooldownData
    {
        public float cooldownDuration;
        public float lastUsedTime;
    }

    private Dictionary<string, CooldownData> cooldowns = new();

    // -------------------------
    // Registration
    // -------------------------
    public void Register(string abilityId, float cooldownDuration)
    {
        if (!cooldowns.ContainsKey(abilityId))
        {
            cooldowns.Add(abilityId, new CooldownData
            {
                cooldownDuration = cooldownDuration,
                lastUsedTime = -999f
            });
        }
    }

    // -------------------------
    // Queries
    // -------------------------
    public bool IsReady(string abilityId)
    {
        if (!cooldowns.TryGetValue(abilityId, out var data))
            return true;

        return Time.time - data.lastUsedTime >= data.cooldownDuration;
    }

    public float GetRemaining(string abilityId)
    {
        if (!cooldowns.TryGetValue(abilityId, out var data))
            return 0f;

        float elapsed = Time.time - data.lastUsedTime;
        return Mathf.Max(0f, data.cooldownDuration - elapsed);
    }

    public float GetProgress(string abilityId)
    {
        if (!cooldowns.TryGetValue(abilityId, out var data))
            return 1f;

        float elapsed = Time.time - data.lastUsedTime;
        return Mathf.Clamp01(elapsed / data.cooldownDuration);
    }

    // -------------------------
    // Control
    // -------------------------
    public void StartCooldown(string abilityId)
    {
        if (cooldowns.TryGetValue(abilityId, out var data))
        {
            data.lastUsedTime = Time.time;
        }
    }
}
