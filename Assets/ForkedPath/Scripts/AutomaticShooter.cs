using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AutomaticShooter : MonoBehaviour
{
    public float currentTime { get; private set; } = 0f;
    
    ProjectilesPattern projectilesPattern;
    Vector2 direction;
    Vector2 offsetPosition;
    bool isShooting = false;
    Queue<(float, ProjectileWave)> shootingQueue = new Queue<(float, ProjectileWave)>();


    public event System.Action OnShoot;

    public static AutomaticShooter ReloadAutomaticShooter(GameObject go, ProjectilesPattern pattern)
    {
        Debug.Assert(pattern != null, $"{go.name}'s pattern is null");
        Debug.Assert(pattern.projectileWaves.Length > 0, $"{go.name}'s pattern has no waves");

        AutomaticShooter shooter;
        if(go.TryGetComponent<AutomaticShooter>(out shooter))
        {
            Destroy(shooter);
        }
        shooter = go.AddComponent<AutomaticShooter>();
        shooter.projectilesPattern = pattern;
        shooter.resetShooting();
        return shooter;
    }

    private void OnDestroy()
    {
        isShooting = false;
    }

    private void OnDisable()
    {
        isShooting = false;
    }

    private void FixedUpdate()
    {
        if (isShooting)
        {
            currentTime += Time.fixedDeltaTime;

            for (; shootingQueue.Count > 0 && currentTime >= shootingQueue.Peek().Item1;)
            {
                var pair = shootingQueue.Dequeue();
                if (pair.Item2 != null)
                {
                    var projectileWave = pair.Item2;

                    // Calculate spread
                    int count = Mathf.Max(1, projectileWave.projectileCount);
                    float spread = projectileWave.angleSpread;
                    float angleStep = count > 1 ? spread / (count - 1) : 0f;
                    float startAngle = -spread / 2f + projectileWave.angleOffset;

                    for (int i = 0; i < count; i++)
                    {
                        float angle = startAngle + i * angleStep;

                        // Add random angle
                        if (projectileWave.randomAngleRange > 0f)
                            angle += Random.Range(-projectileWave.randomAngleRange, projectileWave.randomAngleRange);

                        Vector2 shootDir = Quaternion.Euler(0, 0, angle) * direction;

                        // Calculate spawn position with offset and randomization
                        Vector2 spawnPos = (Vector2)transform.position + offsetPosition + projectileWave.offset;
                        if (projectileWave.randomOffsetRadius > 0f)
                            spawnPos += Random.insideUnitCircle * projectileWave.randomOffsetRadius;

                        ProjectileManager.Instance.Shoot(
                            spawnPos, shootDir, projectileWave.projectileConfig, transform);
                    }
                    OnShoot?.Invoke();
                }
                else
                {
                    resetShooting();
                }
            }
        }
    }
    public void Shoot(Vector2 direction, Vector2 offsetPosition)
    {
        isShooting = true;
        this.direction = direction;
        this.offsetPosition = offsetPosition;
    }

    private void resetShooting(bool resetTime = true)
    {
        isShooting = false;
        if(resetTime)
            currentTime = 0f;
        recreateQueue();
    }

    private void recreateQueue()
    {
        shootingQueue.Clear();
        float sum = 0f;
        for(int i = 0; i < projectilesPattern.projectileWaves.Length; i++)
        {
            shootingQueue.Enqueue((sum, projectilesPattern.projectileWaves[i]));
            sum += projectilesPattern.projectileWaves[i].delayAfterWave;
        }
        shootingQueue.Enqueue((sum, null));
    }

    public void StopShooting(bool resetTime = true)
    {
        resetShooting(resetTime);
    }
}