using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Rocks : MonoBehaviour, IDestroyable
{
    [SerializeField] private float maxHP;
    private float currentHP;

    private Material baseMaterial;
    private SpriteRenderer sr;
    [SerializeField] private Material blinkMaterial;

    [SerializeField] private SOSound damageSound;
    [SerializeField] private SOSound killSound;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseMaterial = sr.material;
    }

    private void OnEnable()
    {
        currentHP = maxHP;
    }

    public void Destroyed()
    {
        gameObject.SetActive(false);
    }

    public void TakeDamage(float damageTaken)
    {
        currentHP -= damageTaken;
        if (currentHP <= 0)
        {
            SoundManager.Instance.Play(killSound);
            Destroyed();
        }
        else
        {
            SoundManager.Instance.Play(damageSound);
            StartCoroutine(BlinkingEffect());
        }
    }
    IEnumerator BlinkingEffect()
    {
        sr.material = blinkMaterial;
        yield return new WaitForSeconds(0.05f);
        sr.material = baseMaterial;
    }
}
