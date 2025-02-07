using Unity.Netcode;
using UnityEngine;

public class SoundManager : NetworkBehaviour
{
    [SerializeField]
    private AudioSource sfxPlace;

    private void Start()
    {
        GameManager.Instance.OnCellClicked += OnCellClicked;
    }

    public override void OnDestroy()
    {
        GameManager.Instance.OnCellClicked -= OnCellClicked;
        base.OnDestroy();
    }

    private void OnCellClicked(object sender, OnCellClickedEventArgs e)
    {
        PlaySfxPlaceRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlaySfxPlaceRpc()
    {
        Debug.Log("PlaySfxPlaceRpc localID   " + NetworkManager.Singleton.LocalClientId + " is server " + NetworkManager.Singleton.IsServer);
        sfxPlace.Play();
    }
}
