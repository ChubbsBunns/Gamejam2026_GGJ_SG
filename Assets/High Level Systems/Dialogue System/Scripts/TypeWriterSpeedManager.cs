using PixelCrushers.DialogueSystem;
using UnityEngine;

public class TypeWriterSpeedManager : MonoBehaviour
{
    public int charactersPerSecondBase = 50;
    public int charactersPerSecondSlow = 5;

    [SerializeField] TextMeshProTypewriterEffect[] textMeshProTypewriterEffects;

    public void SetTypeWriterSpeedSlow()
    {
        for (int i = 0; i < textMeshProTypewriterEffects.Length; i++)
        {
            textMeshProTypewriterEffects[i].charactersPerSecond = charactersPerSecondSlow;
        }
    }

    public void ResetTypeWriterSpeed()
    {
        for (int i = 0; i < textMeshProTypewriterEffects.Length; i++)
        {
            textMeshProTypewriterEffects[i].charactersPerSecond = charactersPerSecondBase;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
