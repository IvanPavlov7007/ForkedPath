using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// TODO make it use EntitySpawnData struct for direction and magnitude
/// </summary>
public class DirectionalEnemySpawn : EnemySpawn
{
    [Header("Directional Spawn")]
    [Tooltip("Direction in which spawned enemies will move.")]
    public Vector2 direction = Vector2.right;
    public float magnitude = 1f;

    [SerializeField, Tooltip("Visual length of the direction arrow in the Scene view.")]
    private float arrowLength = 2f;

    private void OnDrawGizmos()
    {
        // Draw a simple arrow gizmo in the Scene (no Editor API required).
        Vector3 origin = transform.position;
        Vector3 dir = new Vector3(direction.x, direction.y, 0f);
        if (dir.sqrMagnitude < 1e-6f) dir = Vector3.right;
        dir.Normalize();

        Vector3 end = origin + dir * arrowLength;
        Gizmos.color = Color.cyan;

        // Shaft
        Gizmos.DrawLine(origin, end);

        // Arrowhead
        float headLen = arrowLength * 0.25f;
        float headWidth = arrowLength * 0.12f;
        Vector3 right = new Vector3(-dir.y, dir.x, 0f);
        Vector3 p1 = end - dir * headLen + right * headWidth;
        Vector3 p2 = end - dir * headLen - right * headWidth;

        Gizmos.DrawLine(end, p1);
        Gizmos.DrawLine(end, p2);
        Gizmos.DrawLine(p1, p2);
    }

    protected override Entity spawnEnemy(EntityConfig config, Vector2 position)
    {
        // Spawn enemy and set its movement direction
        Entity enemy = base.spawnEnemy(config, position);
        if (enemy != null)
        {
            var additionalDataContainer = enemy.AddComponent<EntitySpawnDataContainer>();
            additionalDataContainer.spawnData.moveDirection = direction;
            additionalDataContainer.spawnData.magnitude = magnitude;
        }
        return enemy;
    }
}