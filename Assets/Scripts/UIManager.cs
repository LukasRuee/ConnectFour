using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (restartButton != null) restartButton.onClick.AddListener(OnRestart);
    }

    private void Start()
    {
        UpdateTurn(GameManager.Instance.CurrentPlayer);
    }

    public void UpdateTurn(int player)
    {
        if (statusText != null) statusText.text = $"Player {player} turn";
    }

    public void ShowWin(int player)
    {
        if (statusText != null) statusText.text = $"Player {player} wins!";
    }

    public void ShowDraw()
    {
        if (statusText != null) statusText.text = "Draw!";
    }

    private void OnRestart()
    {
        SceneManager.LoadScene("Game");
    }
}
