using System.Collections;
using UnityEngine;

public class BoardRenderer : MonoBehaviour
{
    public GameObject[,] Cells { get; private set; }
    [SerializeField] public float CellSize { get; private set; } = 1f;

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform boardParent;

    [Header("Drop Settings")]
    [SerializeField] private float dropTimePerCell = 0.05f;
    [SerializeField] private float cellScale = 2f;

    [Header("Trail Settings")]
    [SerializeField] private float trailTime = 0.18f;
    [SerializeField] private float trailStartWidth = 0.18f;
    [SerializeField] private float trailEndWidth = 0.02f;

    [Header("Impact FX")]
    [SerializeField] private ParticleSystem impactFx;

    private void Start() => CreateGrid();

    private void CreateGrid()
    {
        var gm = GameManager.Instance;
        Cells = new GameObject[gm.columns, gm.rows];

        float offsetX = -(gm.columns - 1) * CellSize * 0.5f;
        float offsetY = -(gm.rows - 1) * CellSize * 0.5f;

        for (int x = 0; x < gm.columns; x++)
        {
            for (int y = 0; y < gm.rows; y++)
            {
                var go = Instantiate(cellPrefab, boardParent);
                go.transform.localPosition = new Vector3(x * CellSize + offsetX, y * CellSize + offsetY, 0f);
                go.name = $"Cell_{x}_{y}";

                Cells[x, y] = go;
                if (go.TryGetComponent<Cell>(out var c)) c.SetIndex(x, y);
            }
        }
    }

    public IEnumerator AnimateDrop(int col, int row, int player)
    {
        var gm = GameManager.Instance;

        float offsetX = -(gm.columns - 1) * CellSize * 0.5f;
        float offsetY = -(gm.rows - 1) * CellSize * 0.5f;

        Vector3 start = new(col * CellSize + offsetX, (gm.rows + 1) * CellSize, 0f);
        Vector3 target = new(col * CellSize + offsetX, row * CellSize + offsetY, 0f);

        var disc = new GameObject($"Disc_{col}_{row}");
        var sprite = disc.AddComponent<SpriteRenderer>();
        disc.transform.SetParent(boardParent, false);
        disc.transform.localPosition = start;

        if (cellPrefab.TryGetComponent<SpriteRenderer>(out var source)) sprite.sprite = source.sprite;
        sprite.color = player == 1 ? gm.playerColor1 : gm.playerColor2;

        float distance = Mathf.Abs(start.y - target.y);
        float duration = distance * dropTimePerCell;
        float t = 0f;

        var trail = disc.AddComponent<TrailRenderer>();
        trail.time = trailTime;
        trail.startWidth = trailStartWidth;
        trail.endWidth = trailEndWidth;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
        trail.textureMode = LineTextureMode.Stretch;
        trail.numCapVertices = 4;
        trail.material = new Material(Shader.Find("Sprites/Default"));

        var baseCol = sprite.color;
        var bright = baseCol * 1.2f;
        bright.a = baseCol.a;

        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(baseCol, 0f), new GradientColorKey(bright, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        trail.colorGradient = gradient;

        while (t < duration)
        {
            t += Time.deltaTime;
            float f = Mathf.Clamp01(t / duration);
            float s = Mathf.Sin(f * Mathf.PI);

            disc.transform.localPosition = Vector3.Lerp(start, target, f);
            disc.transform.localScale = new Vector3(
                cellScale + (1f - s) * 0.12f,
                cellScale + s * 0.18f,
                1f
            );

            yield return null;
        }

        disc.transform.localPosition = target;
        disc.transform.SetParent(Cells[col, row].transform, true);
        if (Cells[col, row].TryGetComponent<Cell>(out var cell)) cell.SetOwner(player);

        if (impactFx != null)
        {
            var fx = Instantiate(impactFx, disc.transform.position, Quaternion.identity, boardParent);
            var main = fx.main;
            main.startColor = sprite.color;
            fx.Play();
        }
    }
}