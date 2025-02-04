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
    }

    void Start()
    {
        GameManager.Instance.OnGameStarted += OnGameStarted;
        GameManager.Instance.OnCurrentPlayerChanged += OnCurrentPlayerChanged;
    }

    private void OnCurrentPlayerChanged(object sender, PlayerType type)
    {
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
                break;
            case PlayerType.Circle:
                circleText.SetActive(true);
                break;
        }
    }
}
