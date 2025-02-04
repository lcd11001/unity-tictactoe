using System;
using Unity.VisualScripting;
using UnityEngine;

public class HudUI : MonoBehaviour
{
    [SerializeField]
    private GameObject crossArrow;
    [SerializeField]
    private GameObject crossText;
    [SerializeField]
    private GameObject circleArrow;
    [SerializeField]
    private GameObject circleText;

    void Awake()
    {
        crossArrow.SetActive(false);
        crossText.SetActive(false);

        circleArrow.SetActive(false);
        circleText.SetActive(false);

        // Fixed: event OnGameStarted was invoked in Main scene, before being subscribed in Game scene
        OnGameStarted(this, EventArgs.Empty);
        OnCurrentPlayerChanged(this, EventArgs.Empty);
    }

    void Start()
    {
        GameManager.Instance.OnGameStarted += OnGameStarted;
        GameManager.Instance.OnCurrentPlayerChanged += OnCurrentPlayerChanged;
    }

    private void OnCurrentPlayerChanged(object sender, EventArgs e)
    {
        PlayerType type = GameManager.Instance.GetCurrentPlayerType();
        Debug.Log("Current player changed to " + type);
        switch (type)
        {
            case PlayerType.Cross:
                crossArrow.SetActive(true);
                circleArrow.SetActive(false);
                break;
            case PlayerType.Circle:
                crossArrow.SetActive(false);
                circleArrow.SetActive(true);
                break;
        }
    }

    private void OnGameStarted(object sender, EventArgs e)
    {
        switch (GameManager.Instance.GetLocalPlayerType())
        {
            case PlayerType.Cross:
                crossText.SetActive(true);
                circleText.SetActive(false);
                break;
            case PlayerType.Circle:
                circleText.SetActive(true);
                crossText.SetActive(false);
                break;
        }
    }
}
