using UnityEngine;

public class GunAutoAim : MonoBehaviour
{
    [Header("Target settings")]
    public string enemyTag = "Enemy";
    public float detectionRadius = 8f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    void Update()
    {
        GameObject nearestEnemy = FindNearestEnemyInRange();

        if (nearestEnemy == null)
            return;

        Vector3 direction = nearestEnemy.transform.position - transform.position;
        direction.y = 0f; // не наклоняемся вверх/вниз

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    GameObject FindNearestEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject nearest = null;
        float minDistance = detectionRadius;

        Vector3 gunPos = new Vector3(transform.position.x, 0f, transform.position.z);

        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPos = new Vector3(
                enemy.transform.position.x,
                0f,
                enemy.transform.position.z
            );

            float distance = Vector3.Distance(gunPos, enemyPos);

            if (distance <= minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
#endif
}
