using System.Collections;
using UnityEngine;

public class LimitedLifetime : MonoBehaviour
{
    public float lifetime{ get; private set; }
    SimpleTimer timer;

    public void Initialize(float lifetime)
    {
        this.lifetime = lifetime;
        timer = new SimpleTimer(lifetime);
    }

    private void Update()
    {
        if (timer != null && timer.tick(Time.deltaTime))
        {
            Destroy(gameObject);
            timer = null;
        }
    }
}