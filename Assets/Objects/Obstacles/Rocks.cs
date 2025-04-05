using System.Collections;
using UnityEngine;

public class Rocks : MonoBehaviour, IDestroyable
{
    [SerializeField] private float maxHP;
    private float currentHP;

    private Material baseMaterial;
    private SpriteRenderer sr;
    [SerializeField] private Material blinkMaterial;

    private void Awake()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();
        baseMaterial = sr.material;
    }

    public void Destroyed()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float damageTaken)
    {
        currentHP -= damageTaken;
        StartCoroutine(BlinkingEffect());
        if (currentHP <= 0)
        {
            Destroyed();
        }
    }
    IEnumerator BlinkingEffect()
    {
        sr.material = blinkMaterial;
        yield return new WaitForSeconds(0.05f);
        sr.material = baseMaterial;
    }
}
