using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    [SerializeField] private float shakeAmount = 0.7f;
    [SerializeField] private float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void OnEnable()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            Vector2 randCricle = Random.insideUnitCircle;
            transform.position = originalPos + new Vector3(randCricle.x * shakeAmount, randCricle.y * shakeAmount, -10);

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            transform.position = new Vector3(originalPos.x, originalPos.y, -10);
        }
    }
}
