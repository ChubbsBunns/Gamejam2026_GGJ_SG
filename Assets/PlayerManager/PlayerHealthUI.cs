using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Heart Container")]
    [SerializeField] private Transform heartsParent;  // Empty GameObject under Canvas where hearts spawn
    [SerializeField] private GameObject heartPrefab;  // Prefab for one heart (Image component)

    [Header("Heart Sprites")]
    [SerializeField] private List<UnityEngine.UI.Image> presetHearts = new List<UnityEngine.UI.Image>();
    private PlayerBase currentPlayer;

    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    public void UpdateHearts(int currHealth, int maxHealth)
    {
        // Build new hearts
        for (int i = 0; i < presetHearts.Count; i++)
        {
            if (i < maxHealth)
            {
                UnityEngine.UI.Image img = presetHearts[i];
                presetHearts[i].enabled = true;
                if (i < currHealth)
                {
                    img.sprite = fullHeartSprite;
                } else
                {
                    img.sprite = emptyHeartSprite;
                }
            }
            else
            {
                presetHearts[i].enabled = false;
            }

        }
        
    }
}
