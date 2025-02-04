using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button buttonHost;

    [SerializeField]
    private Button buttonClient;

    void Awake()
    {
        buttonHost.onClick.AddListener(OnHostClicked);
        buttonClient.onClick.AddListener(OnClientClicked);

    }

    private void OnClientClicked()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void OnHostClicked()
    {
        NetworkManager.Singleton.StartHost();
    }
}
