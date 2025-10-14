using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class BlobController : EntityComponent, IFacingDirectionProvider
{
    private EntitySpawnData spawnData;

    public float jumpCycleDuration = 0.5f;
    public float jumpHeight = 0.3f;
    public float bodyReturnSpeed = 8f;

    private Vector3 bodyInitialLocalPos;


    public Vector2 Direction { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        bodyInitialLocalPos = body.localPosition;
    }

    private void Start()
    {
        spawnData = GetComponent<EntitySpawnData>();
    }

    private Vector2 currentDirection()
    {
        if (spawnData == null)
            return default;

        if (spawnData.splineContainer != null)
        {
            return closestSplineDirection();
        }
        if (spawnData.moveDirection != Vector2.zero)
        {
            return spawnData.moveDirection.normalized;
        }
        return Vector2.zero;
    }

    private Vector2 closestSplineDirection()
    {
        if (spawnData == null || spawnData.splineContainer == null) return Vector2.zero;
        var spline = spawnData.splineContainer.Spline;
        Vector3 currentPosition = spawnData.splineContainer.transform.InverseTransformPoint(transform.position);
        float splineT;
        float res = SplineUtility.GetNearestPoint(spline, currentPosition, out _, out splineT);
        Vector3 direction = spline.EvaluateTangent(splineT);
        return direction;
    }

    private void FixedUpdate()
    {
        if (entity == null || entity.Rb == null || entity.Config == null || body == null) return;

        // Only animate/move if not dead or falling
        bool canJump = entity.CurrentState == EntityState.Alive
            || entity.CurrentState == EntityState.Hit
            || entity.CurrentState == EntityState.Invincible;

        Vector2 dir = currentDirection();

        // Calculate hopping phase (0 to 1)
        float phase = (Time.fixedTime % jumpCycleDuration) / jumpCycleDuration;
        // Use a sine wave for smooth hop, clamp to positive only (no negative velocity)
        float hopFactor = Mathf.Max(0f, Mathf.Sin(phase * Mathf.PI));

        if (canJump && dir != Vector2.zero)
        {
            // Project velocity along direction, scaled by moveSpeed and hopFactor
            entity.Rb.linearVelocity = dir.normalized * entity.Config.moveSpeed * hopFactor;

            // Animate body jump
            Vector3 jumpOffset = new Vector3(0, hopFactor * jumpHeight, 0);
            body.localPosition = bodyInitialLocalPos + jumpOffset;
        }
        else
        {
            // Stop movement
            entity.Rb.linearVelocity = Vector2.zero;

            // Smoothly return body to initial position
            body.localPosition = Vector3.Lerp(
                body.localPosition,
                bodyInitialLocalPos,
                Time.fixedDeltaTime * bodyReturnSpeed
            );
        }
        Direction = entity.Rb.linearVelocity.normalized;
    }
}
