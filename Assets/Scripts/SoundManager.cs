using Unity.Netcode;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource sfxPlace;

    [SerializeField]
    private AudioSource sfxError;

    private void Start()
    {
        GameManager.Instance.OnGameSound += OnGameSound;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameSound -= OnGameSound;
    }

    private void OnGameSound(object sender, OnGameSoundArgs e)
    {
        switch (e.SoundType)
        {
            case SoundType.Place:
                PlaySfxPlace();
                break;
            case SoundType.Error:
                PlaySfxError();
                break;
        }
    }


    private void PlaySfxPlace()
    {
        Debug.Log("PlaySfxPlaceRpc localID " + NetworkManager.Singleton.LocalClientId + " is server " + NetworkManager.Singleton.IsServer);
        sfxPlace.Play();
    }

    public void PlaySfxError()
    {
        Debug.Log("PlaySfxErrorRpc localID " + NetworkManager.Singleton.LocalClientId + " is server " + NetworkManager.Singleton.IsServer);
        sfxError.Play();
    }
}
