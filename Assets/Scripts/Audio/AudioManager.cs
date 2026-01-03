using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource musicSource;

    void Awake()
    {
        // هاد الكود كيخلي الموسيقى ما تطفاش فاش كتبدل المرحلة (Scene)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        musicSource = GetComponent<AudioSource>();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void ChangeVolume(float volume)
    {
        musicSource.volume = volume;
    }
}