using UnityEngine;

public class HamburgerController : BaseShooterProvider, IFacingDirectionProvider, IMovementProvider
{
    public Vector2 Direction => throw new System.NotImplementedException();

    public bool IsMoving => throw new System.NotImplementedException();

    public Vector2 Velocity => throw new System.NotImplementedException();
}
