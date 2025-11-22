using System.Collections;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [HideInInspector] public int col, row;
    private int owner;
    private SpriteRenderer sr;

    [SerializeField] private Material baseMaterial;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseDuration = 0.6f;
    [SerializeField] private float pulseAmplitude = 0.08f;

    private void Awake() => sr = GetComponent<SpriteRenderer>();

    public void SetIndex(int c, int r) => (col, row) = (c, r);

    public void SetOwner(int p)
    {
        owner = p;
        if (!sr) return;

        sr.color = p switch
        {
            1 => GameManager.Instance.playerColor1,
            2 => GameManager.Instance.playerColor2,
            _ => Color.white
        };
    }

    public void Pulse() => StartCoroutine(PulseRoutine());

    private IEnumerator PulseRoutine()
    {
        Vector3 baseScale = transform.localScale;
        float t = 0f;

        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            float s = 1f + Mathf.Sin(t * Mathf.PI * 2f) * pulseAmplitude;
            transform.localScale = baseScale * s;
            yield return null;
        }

        transform.localScale = baseScale;
    }
}