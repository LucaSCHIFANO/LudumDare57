using UnityEngine;

public interface IDestroyable
{
    void TakeDamage(float damageTaken);
    void Destroyed();
}
