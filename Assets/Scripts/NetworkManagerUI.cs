using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button buttonHost;

    [SerializeField]
    private Button buttonClient;

    [SerializeField]
    private GameObject loadingPannel;

    void Awake()
    {
        buttonHost.onClick.AddListener(OnHostClicked);
        buttonClient.onClick.AddListener(OnClientClicked);
        loadingPannel.SetActive(false);
    }

    private void OnClientClicked()
    {
        bool success = NetworkManager.Singleton.StartClient();
        if (success)
        {
            loadingPannel.SetActive(true);

            buttonHost.gameObject.SetActive(false);
            buttonClient.gameObject.SetActive(false);
        }
    }

    private void OnHostClicked()
    {
        bool success = NetworkManager.Singleton.StartHost();
        if (success)
        {
            loadingPannel.SetActive(true);

            buttonHost.gameObject.SetActive(false);
            buttonClient.gameObject.SetActive(false);
        }
    }
}
