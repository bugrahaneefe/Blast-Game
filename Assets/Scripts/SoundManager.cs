using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip boxBreakSound;
    public AudioClip stoneBreakSound;
    public AudioClip vaseBreakSound;

    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }
    #endregion

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayClick()
    {
        PlaySound(clickSound);
    }

    public void PlayBoxBreak()
    {
        PlaySound(boxBreakSound);
    }

    public void PlayStoneBreak()
    {
        PlaySound(stoneBreakSound);
    }

    public void PlayVaseBreak()
    {
        PlaySound(vaseBreakSound);
    }
}
