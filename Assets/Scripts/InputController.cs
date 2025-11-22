using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    [SerializeField] private BoardRenderer boardRenderer;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private GameObject winExplosionPrefab;

    [Header("Wiggle Settings")]
    [SerializeField] private float wiggleDuration = 0.24f;
    [SerializeField] private float wiggleFrequency = 80f;
    [SerializeField] private float wiggleAmplitude = 0.06f;

    [Header("Drop/Camera Settings")]
    [SerializeField] private float cameraTiltAmount = 2.5f;
    [SerializeField] private float cameraTiltDuration = 0.08f;
    [SerializeField] private float cameraShakeDuration = 0.25f;
    [SerializeField] private float cameraShakeMagnitude = 0.06f;
    [SerializeField] private float threatTimeScale = 0.6f;
    [SerializeField] private float threatPauseRealtime = 0.15f;

    private bool isAnimating;

    private void Update()
    {
        if (isAnimating) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector3 wp = worldCamera.ScreenToWorldPoint(Input.mousePosition);
        TryPlace(WorldToColumn(wp.x));
    }

    private int WorldToColumn(float worldX)
    {
        var gm = GameManager.Instance;
        float offsetX = -(gm.columns - 1) * boardRenderer.CellSize * 0.5f;
        float localX = worldX - offsetX;
        return Mathf.FloorToInt(localX / boardRenderer.CellSize + 0.5f);
    }

    public void TryPlace(int col)
    {
        var gm = GameManager.Instance;
        if (gm.LastWinCells.Count > 0) return;
        if (col < 0 || col >= gm.columns) return;

        if (gm.IsColumnFull(col))
        {
            StartCoroutine(Wiggle(col));
            AudioManager.Instance?.PlayError();
            return;
        }

        int row = gm.DropPiece(col);
        if (row < 0) return;

        StartCoroutine(HandleAfterDrop(col, row, gm.CurrentPlayer));
    }

    private IEnumerator HandleAfterDrop(int col, int row, int player)
    {
        isAnimating = true;
        yield return boardRenderer.StartCoroutine(boardRenderer.AnimateDrop(col, row, player));

        if (CameraManager.Instance != null)
            StartCoroutine(CameraManager.Instance.Tilt(cameraTiltAmount, cameraTiltDuration));

        var gm = GameManager.Instance;

        if (gm.CheckWin(col, row))
        {
            UIManager.Instance.ShowWin(player);
            foreach (var c in gm.LastWinCells)
                boardRenderer.Cells[c.x, c.y].GetComponent<Cell>()?.Pulse();

            StartCoroutine(CameraManager.Instance.Shake(cameraShakeDuration, cameraShakeMagnitude));
            AudioManager.Instance?.PlayWin();
            AudioManager.Instance?.Cheer();

            if (winExplosionPrefab != null)
            {
                Color playerColor = gm.CurrentPlayer == 1 ? Color.red : Color.yellow;
                foreach (var c in gm.LastWinCells)
                {
                    var pos = boardRenderer.Cells[c.x, c.y].transform.position;
                    var fx = Instantiate(winExplosionPrefab, pos, Quaternion.identity)
                        .GetComponent<ParticleSystem>();
                    if (fx != null)
                    {
                        var main = fx.main;
                        main.startColor = playerColor;
                        fx.Play();
                    }
                }
            }

            isAnimating = false;
            yield break;
        }

        if (gm.IsBoardFull())
        {
            UIManager.Instance.ShowDraw();
            isAnimating = false;
            yield break;
        }

        if (gm.ThreatDetected(col, row))
        {
            Time.timeScale = threatTimeScale;
            yield return new WaitForSecondsRealtime(threatPauseRealtime);
            Time.timeScale = 1f;
        }

        gm.SwitchPlayer();
        UIManager.Instance.UpdateTurn(gm.CurrentPlayer);
        AudioManager.Instance?.PlayDrop();
        isAnimating = false;
    }

    private IEnumerator Wiggle(int col)
    {
        if (boardRenderer == null) yield break;
        var gm = GameManager.Instance;
        var topCell = boardRenderer.Cells[col, gm.rows - 1];
        if (topCell == null) yield break;

        Vector3 basePos = topCell.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < wiggleDuration)
        {
            elapsed += Time.deltaTime;
            float offset = Mathf.Sin(elapsed * wiggleFrequency) * wiggleAmplitude;
            topCell.transform.localPosition = basePos + Vector3.right * offset;
            yield return null;
        }

        topCell.transform.localPosition = basePos;
    }
}