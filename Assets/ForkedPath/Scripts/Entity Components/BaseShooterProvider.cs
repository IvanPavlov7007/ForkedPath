using UnityEngine;

public abstract class BaseShooterProvider : EntityComponent, IShooterProvider
{
    protected bool wasShooting = false;

    public bool ConsumeShotThisFrame()
    {
        if (wasShooting)
        {
            wasShooting = false;
            return true;
        }
        return false;
    }
}