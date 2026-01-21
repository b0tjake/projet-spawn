using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 40f;
    public float lifeTime = 0.3f;

    void Start() => Destroy(gameObject, lifeTime); // Auto-destroy after 2s

    private void OnTriggerEnter2D(Collider2D other)
{
    EnemyBase enemy = other.GetComponent<EnemyBase>();
    if (enemy != null)
    {
        Vector2 dir = transform.right; 
        enemy.TakeDamage(damage, dir, 0.2f);
        Destroy(gameObject); 
    }
}
}