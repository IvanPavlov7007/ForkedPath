using UnityEngine;

[RequireComponent(typeof(BetweenBoundariesMover))]
class TestBetweenBoundaries : MonoBehaviour
{
    [SerializeField]
    AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField]
    float Speed = 2f;
    BetweenBoundariesMover mover;

    private void Awake()
    {
        mover = GetComponent<BetweenBoundariesMover>();
    }

    private void Start()
    {
        MoveTowardsPlayer();
    }

    private void OnEnable()
    {
        mover.destinationReached += OnReach;
    }

    private void OnDisable()
    {
        mover.destinationReached -= OnReach;
    }

    void MoveTowardsPlayer()
    {
        var playerDir = Player.Instance != null && Player.Instance.CurrentAvatar != null ? (Vector2)(Player.Instance.CurrentAvatar.transform.position - transform.position) : Vector2.zero;
        mover.Move(playerDir, Speed, AnimationCurve);
    }

    void OnReach()
    {
        MoveTowardsPlayer();
    }

}