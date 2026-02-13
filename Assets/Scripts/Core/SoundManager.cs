using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance {get; private set;}
    private AudioSource soundSource;
    private AudioSource musicSource;


    private void Awake() {
        soundSource = GetComponent<AudioSource>();
        musicSource = transform.GetChild(0).GetComponent<AudioSource>();
        instance = this;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        //added to trigger reading the last values of sound and music volume in Change methods (without modifying it)
        ChangeMusicVolume(0);
        ChangeSoundVolume(0);
    }

    public void PlaySound(AudioClip clip)
    {
        soundSource.PlayOneShot(clip);
    }

    private void ChangeSourceVolume(AudioSource source, float _change, string volumeTypeName)
    {
        float currentVolume = PlayerPrefs.GetFloat(volumeTypeName, 1);
        currentVolume += _change;
        if (currentVolume > 1)
        {
            currentVolume = 0;
        }
        else if (currentVolume < 0)
        {
            currentVolume = 1;
        }
        source.volume = currentVolume; 

        PlayerPrefs.SetFloat(volumeTypeName, currentVolume);
    }
    public void ChangeSoundVolume( float _change)
    {
        ChangeSourceVolume(soundSource, _change, "soundVolume");
    }
    public void ChangeMusicVolume( float _change)
    {
        ChangeSourceVolume(musicSource, _change, "musicVolume");
    }
}
