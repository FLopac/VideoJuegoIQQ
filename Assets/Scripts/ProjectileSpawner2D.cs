using System.Collections;
using UnityEngine;

public class ProjectileSpawner2D : MonoBehaviour
{
    [SerializeField] private Projectile2D projectilePrefab;
    [SerializeField] private float secondsBetweenShots = 1.8f;
    [SerializeField] private float projectileSpeed = 6f;
    [SerializeField] private Vector2 direction = Vector2.down;

    private IEnumerator Start()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Asigna el prefab del proyectil en ProjectileSpawner2D.", this);
            yield break;
        }

        while (true)
        {
            yield return new WaitForSeconds(Mathf.Max(0.2f, secondsBetweenShots));
            Projectile2D projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.Launch(direction, projectileSpeed);
        }
    }
}
