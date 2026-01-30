using System.Collections;
using UnityEngine;

public class CrystalManager : MonoBehaviour
{
    [SerializeField] private float _dissolveTime = 1.2f;

    private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;

    public bool useDissolve = true;
    public bool useVertical = true;
    
    private int dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private int verticalDissolveAmount = Shader.PropertyToID("_VerticalDissolve");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _materials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _materials[i] = _spriteRenderers[i].material;
        }
    }

    public void VanishCrystal()
    {
        StartCoroutine(Vanish(useDissolve, useVertical));
    }

    public void AppearCrystal()
    {
        StartCoroutine(Appear(useDissolve, useVertical));        
    }

    private IEnumerator Vanish(bool useDissolve, bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime/_dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(0f, 1.1f, elapsedTime/ _dissolveTime);

            for (int i = 0; i < _materials.Length; i++)
            {
                if (useDissolve)
                {
                    _materials[i].SetFloat(dissolveAmount, lerpedDissolve);
                }

                if (useVertical)
                {
                    _materials[i].SetFloat(verticalDissolveAmount, lerpedVerticalDissolve);
                }
            }
            yield return null;
        }

    }

    private IEnumerator Appear(bool useDissolve, bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedDissolve = Mathf.Lerp(1.1f, 0, elapsedTime/_dissolveTime);
            float lerpedVerticalDissolve = Mathf.Lerp(1.1f, 0, elapsedTime/ _dissolveTime);

            for (int i = 0; i < _materials.Length; i++)
            {
                if (useDissolve)
                {
                    _materials[i].SetFloat(dissolveAmount, lerpedDissolve);
                }

                if (useVertical)
                {
                    _materials[i].SetFloat(verticalDissolveAmount, lerpedVerticalDissolve);
                }
            }
            yield return null;
        }

    }     
}
