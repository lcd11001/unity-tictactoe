using Unity.Netcode;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource sfxPlace;

    [SerializeField]
    private AudioSource sfxError;

    [SerializeField]
    private AudioSource sfxWin;

    [SerializeField]
    private AudioSource sfxLose;

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
            case SoundType.Win:
                PlaySfxWin();
                break;
            case SoundType.Lose:
                PlaySfxLose();
                break;
        }
    }


    private void PlaySfxPlace()
    {
        Debug.Log("PlaySfxPlace localID " + NetworkManager.Singleton.LocalClientId + " is server " + NetworkManager.Singleton.IsServer);
        sfxPlace.Play();
    }

    private void PlaySfxError()
    {
        Debug.Log("PlaySfxError localID " + NetworkManager.Singleton.LocalClientId + " is server " + NetworkManager.Singleton.IsServer);
        sfxError.Play();
    }

    private void PlaySfxWin()
    {
        Debug.Log("PlaySfxWin localID " + NetworkManager.Singleton.LocalClientId + " is server " + NetworkManager.Singleton.IsServer);
        sfxWin.Play();
    }

    private void PlaySfxLose()
    {
        Debug.Log("PlaySfxLose localID " + NetworkManager.Singleton.LocalClientId + " is server " + NetworkManager.Singleton.IsServer);
        sfxLose.Play();
    }
}
