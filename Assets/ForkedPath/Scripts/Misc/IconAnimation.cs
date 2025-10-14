using System.Collections;
using UnityEngine;

public class IconAnimation : MonoBehaviour
{
    public readonly static float sinAmplitude = 0.2f;
    public readonly static float sinFrequency = 2f;
    public readonly static float rotationAmplitude = 10f;
    public readonly static float rotationFrequency = 1f;
    public readonly static float xScale = 1f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }
    
    private void Update()
    {
        float x = transform.position.x;
        float newY = initialPosition.y + Mathf.Sin(x * xScale + Time.time * sinFrequency) * sinAmplitude;
        transform.localPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
        float newZRotation = Mathf.Sin(x * xScale + Time.time * rotationFrequency) * rotationAmplitude;
        transform.localRotation = initialRotation * Quaternion.Euler(0f, 0f, newZRotation);
    }
}