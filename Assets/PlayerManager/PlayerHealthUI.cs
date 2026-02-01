using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Numerics;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Heart Container")]
    [SerializeField] private Transform heartsParent;  // Empty GameObject under Canvas where hearts spawn
    [SerializeField] private GameObject heartPrefab;  // Prefab for one heart (Image component)

    [Header("Heart Sprites")]
    [SerializeField] private List<UnityEngine.UI.Image> presetHearts = new List<UnityEngine.UI.Image>();

    public GameObject healthBar;

    public void UpdateHearts(int currHealth, int maxHealth)
    {
        float t = (float)currHealth/(float)maxHealth;
        healthBar.transform.localScale = new UnityEngine.Vector3(t, 1, 1);

        
    }
}
