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
    Queue<(float, ProjectileConfig)> shootingQueue = new Queue<(float, ProjectileConfig)>();


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

            for(; shootingQueue.Count > 0 && currentTime >= shootingQueue.Peek().Item1; )
            {
                var wave = shootingQueue.Dequeue();
                if (wave.Item2 != null)
                {
                    ProjectileManager.Instance.Shoot(
                        (Vector2)transform.position + offsetPosition, direction, wave.Item2, transform);
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

    private void resetShooting()
    {
        isShooting = false;
        currentTime = 0f;
        recreateQueue();
    }

    private void recreateQueue()
    {
        shootingQueue.Clear();
        float sum = 0f;
        for(int i = 0; i < projectilesPattern.projectileWaves.Length; i++)
        {
            shootingQueue.Enqueue((sum, projectilesPattern.projectileWaves[i].projectileConfig));
            sum += projectilesPattern.projectileWaves[i].delayAfterWave;
        }
        shootingQueue.Enqueue((sum, null));
    }

    public void StopShooting()
    {
        resetShooting();
    }
}