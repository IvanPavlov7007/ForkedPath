public enum InvincibilityReason { AfterHit, PowerUp, Script }

public sealed class InvincibilityEventData
{
    public Entity Entity { get; }
    public bool IsInvincible { get; }
    public float Duration { get; }
    public InvincibilityReason Reason { get; }

    public InvincibilityEventData(Entity entity, bool isInvincible, float duration, InvincibilityReason reason)
    {
        Entity = entity;
        IsInvincible = isInvincible;
        Duration = duration;
        Reason = reason;
    }
}