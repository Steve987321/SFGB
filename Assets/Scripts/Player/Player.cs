using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _health = 100;

    public void DoDamage(float val)
    {
        _health -= val;
        if (_health < 0)
        {
            _health = 0;
            Die();
        }

    }

    void Die()
    {
        print("player has died");
    }
}
