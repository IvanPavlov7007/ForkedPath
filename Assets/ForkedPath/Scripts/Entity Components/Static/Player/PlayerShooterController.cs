using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerShooterController : MonoBehaviour, IShooterProvider
{
    [SerializeField]
    Vector2 shoulderOffset = Vector2.up;
    [SerializeField]
    float shootOffset = 0.5f;
    [SerializeField]
    ProjectilesPattern projectilePattern;

    AutomaticShooter automaticShooter;
    PlayerController playerController;
    SimpleEntityAnimatorController animatorController;

    bool wasShooting = false;
    public bool ConsumeShotThisFrame()
    {
        if(wasShooting)
        {
            wasShooting = false;
            return true;
        }
        return false;
    }

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerController.OnFixedUpdated += onPlayerFixedUpdated;
        automaticShooter = AutomaticShooter.ReloadAutomaticShooter(gameObject, projectilePattern);
        animatorController = GetComponent<SimpleEntityAnimatorController>();
    }

    private void OnEnable()
    {
        if(automaticShooter != null && animatorController != null)
        {
            automaticShooter.OnShoot += OnShoot;
        }
    }

    private void OnDisable()
    {
        if (automaticShooter != null && animatorController != null)
        {
            automaticShooter.OnShoot -= OnShoot;
        }
    }

    private void OnShoot()
    {
        wasShooting = true;
    }

    void onPlayerFixedUpdated()
    {
        if(playerController == null)
            return;
        if (automaticShooter == null)
            return;

        if (playerController.shooting)
        {
            Vector2 direction = playerController.Direction;
            automaticShooter.Shoot(playerController.Direction, shoulderOffset + direction * shootOffset);
        }
        else
        {
            automaticShooter.StopShooting();
        }
    }
}