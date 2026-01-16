using UnityEngine;

public class TurretAutoAim : MonoBehaviour
{
    public float attackRadius = 10f;
    public float attackSpeed = 1f;
    public float damage = 10f;
    public float critChance = 0.1f;

    string enemyTag = "Enemy";
    Transform target;

    void Update()
    {
        target = null;
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float d = Mathf.Infinity;

        foreach (var e in enemies)
        {
            float nd = Vector3.Distance(transform.position, e.transform.position);
            if (nd < d && nd <= attackRadius)
            {
                d = nd;
                target = e.transform;
            }
        }

        if (!target) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        var rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 1 * Time.deltaTime * 100f);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}