[System.Serializable]
public enum EntityState
{
    Alive,
    Dead,
    Hit,
    Falling,
    Invincible,
    DeadFalling,
    Despawned
}

public class EntityStateChangeData
{
    public DamageEventData damageEventData;
    public DeathEventData deathEventData;
    public FallingEventData fallingEventData;
    public CorpseLandedEventData corpseLandedEventData;
    public InvincibilityEventData invincibilityEventData;
    public EatingEventData eatingEventData;
}