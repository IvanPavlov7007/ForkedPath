[System.Serializable]
public enum EntityState
{
    Alive,
    Dead,
    Hit,
    Falling,
    Invincible,
    DeadFalling
}

public class EntityStateChangeData
{
    public DamageEventData damageEventData;
    public DeathEventData deathEventData;
    public FallingEventData fallingEventData;
    public CorpseLandedEventData corpseLandedEventData;
    public InvincibilityEventData invincibilityEventData;
}