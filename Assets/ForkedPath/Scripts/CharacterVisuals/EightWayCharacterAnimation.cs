using System;
using System.Collections.Generic;
using UnityEngine;

public class EightWayCharacterAnimation : MonoBehaviour
{
    [Serializable]
    public class DirectionalAnimation
    {
        public FacingDirection direction;
        public Sprite[] frames;
        public float frameDuration = 0.1f; // Duration per frame in seconds
    }

    public class AnimationState
    {
        public EntityState state;
        public DirectionalAnimation[] animationss;
    }

    [SerializeField]
    private List<DirectionalAnimation> animations;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private PlayerController playerController;

    private float timeCounter;
    private FacingDirection currentDirection = FacingDirection.Down;
    

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerController.fixedUpdated += () =>
        {
            currentDirection = playerController.CurrentDirection;
        };
    }

    void Update()
    {
        // Example: Get direction from PlayerController (replace with your actual reference)
        // currentDirection = playerController.CurrentFacingDirection;

        timeCounter += Time.deltaTime;

        var anim = animations.Find(a => a.direction == currentDirection);
        if (anim == null || anim.frames == null || anim.frames.Length == 0) return;

        int frameCount = anim.frames.Length;
        float totalAnimTime = frameCount * anim.frameDuration;
        float timeInAnim = timeCounter % totalAnimTime;
        int frameIndex = Mathf.FloorToInt(timeInAnim / anim.frameDuration);

        spriteRenderer.sprite = anim.frames[frameIndex];
    }

    // Optional: Call this to change direction externally
    public void SetDirection(FacingDirection direction)
    {
        if (currentDirection != direction)
        {
            currentDirection = direction;
            timeCounter = 0f; // Optionally reset animation on direction change
        }
    }
}