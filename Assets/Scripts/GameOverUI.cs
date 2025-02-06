using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private GameObject youWin;
    [SerializeField]
    private GameObject youLose;
    [SerializeField]
    private Button rematchButton;

    void Awake()
    {
        rematchButton.onClick.AddListener(OnRematch);
        Hide();
    }

    private void Start()
    {
        GameManager.Instance.OnGameWinner += OnGameEnded;
        GameManager.Instance.OnGameRematch += OnGameRematch;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameWinner -= OnGameEnded;
        GameManager.Instance.OnGameRematch -= OnGameRematch;
        rematchButton.onClick.RemoveListener(OnRematch);
    }

    public void Hide()
    {
        youWin.SetActive(false);
        youLose.SetActive(false);
        rematchButton.gameObject.SetActive(false);
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
        rematchButton.gameObject.SetActive(true);
    }

    private void OnGameRematch(object sender, EventArgs e)
    {
        Hide();
    }

    private void OnRematch()
    {
        GameManager.Instance.RematchRpc();
    }
}
