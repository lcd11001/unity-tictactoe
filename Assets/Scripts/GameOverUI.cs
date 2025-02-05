using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private GameObject youWin;
    [SerializeField]
    private GameObject youLose;

    void Awake()
    {
        ResetUI();
    }

    public void ResetUI()
    {
        youWin.SetActive(false);
        youLose.SetActive(false);
    }

    private void Start()
    {
        GameManager.Instance.OnGameWinner += OnGameEnded;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameWinner -= OnGameEnded;
    }

    private void OnGameEnded(object sender, OnGameWinnerArgs e)
    {
        if (e.Winner == GameManager.Instance.GetLocalPlayerType())
        {
            youWin.SetActive(true);
        }
        else
        {
            youLose.SetActive(true);
        }
    }
}
