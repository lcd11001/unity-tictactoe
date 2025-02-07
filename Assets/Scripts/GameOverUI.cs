using System;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using System.Reflection;
#endif

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private GameObject youWin;
    [SerializeField]
    private GameObject youLose;
    [SerializeField]
    private GameObject youDraw;
    [SerializeField]
    private Button rematchButton;

    void Awake()
    {
        rematchButton.onClick.AddListener(OnRematchClicked);
        Hide();
    }

    private void Start()
    {
        GameManager.Instance.OnGameWinner += OnGameEnded;
        GameManager.Instance.OnGameRematch += OnGameRematch;
        GameManager.Instance.OnGameDraw += OnGameDraw;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameWinner -= OnGameEnded;
        GameManager.Instance.OnGameRematch -= OnGameRematch;
        GameManager.Instance.OnGameDraw -= OnGameDraw;
        rematchButton.onClick.RemoveListener(OnRematchClicked);
    }

    public void Hide()
    {
        youWin.SetActive(false);
        youLose.SetActive(false);
        youDraw.SetActive(false);
        rematchButton.gameObject.SetActive(false);

        // clear log messages from the console
        Debug.ClearDeveloperConsole();

#if UNITY_EDITOR
        ClearLog();
#endif
    }

#if UNITY_EDITOR
    // https://stackoverflow.com/questions/40577412/clear-editor-console-logs-from-script
    private void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
#endif

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

    private void OnGameDraw(object sender, EventArgs e)
    {
        youDraw.SetActive(true);
        rematchButton.gameObject.SetActive(true);
    }

    private void OnGameRematch(object sender, EventArgs e)
    {
        Hide();
    }

    private void OnRematchClicked()
    {
        GameManager.Instance.RematchRpc();
    }
}
