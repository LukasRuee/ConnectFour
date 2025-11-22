using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    private Vector3 startPos;

    [Header("Tilt Settings")]
    [SerializeField] private float tiltAngle = 2.5f;
    [SerializeField] private float tiltDuration = 0.08f;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeStrength = 0.06f;

    [Header("Background Settings")]
    [SerializeField] private float backgroundLerpTime = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        startPos = transform.localPosition;
    }

    public IEnumerator Tilt(float angle, float duration)
    {
        Quaternion start = transform.rotation;
        Quaternion end = Quaternion.Euler(0, 0, angle);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(start, end, t / duration);
            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(end, start, t / duration);
            yield return null;
        }
    }

    public IEnumerator Shake(float duration, float strength)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localPosition = startPos + (Vector3)Random.insideUnitCircle * strength;
            yield return null;
        }
        transform.localPosition = startPos;
    }

    public IEnumerator LerpBackground(Color target, float duration)
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;

        Color start = cam.backgroundColor;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            cam.backgroundColor = Color.Lerp(start, target, t / duration);
            yield return null;
        }

        cam.backgroundColor = target;
    }

    public void TiltDefault() => StartCoroutine(Tilt(tiltAngle, tiltDuration));
    public void ShakeDefault() => StartCoroutine(Shake(shakeDuration, shakeStrength));
    public void LerpBackgroundDefault(Color target) => StartCoroutine(LerpBackground(target, backgroundLerpTime));
}