using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerShooterController : MonoBehaviour
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
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerController.fixedUpdated += onPlayerFixedUpdated;
        automaticShooter = AutomaticShooter.ReloadAutomaticShooter(gameObject, projectilePattern);
        animatorController = GetComponent<SimpleEntityAnimatorController>();
    }

    private void OnEnable()
    {
        if(automaticShooter != null && animatorController != null)
        {
            automaticShooter.OnShoot += animatorController.Shoot;
        }
    }

    private void OnDisable()
    {
        if (automaticShooter != null && animatorController != null)
        {
            automaticShooter.OnShoot -= animatorController.Shoot;
        }
    }

    void onPlayerFixedUpdated()
    {
        if (automaticShooter == null)
            return;

        if (playerController.shooting)
        {
            Vector2 direction = playerController.CurrentDirectionVector;
            automaticShooter.Shoot(playerController.CurrentDirectionVector, shoulderOffset + direction * shootOffset);
        }
        else
        {
            automaticShooter.StopShooting();
        }
    }
}