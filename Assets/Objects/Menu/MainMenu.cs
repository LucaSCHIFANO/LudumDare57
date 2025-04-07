using UnityEngine;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup musicAudioMixer;
    [SerializeField] private AudioMixerGroup sfxAudioMixer;

    private bool isMusicMuted = false;
    private bool isSFXMuted = false;

    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
            Application.Quit();
        
    }

    public void MuteMusic()
    {
        if (isMusicMuted)
        {
            musicAudioMixer.audioMixer.SetFloat("MusicVolume", 0f);
        }
        else
        {

            musicAudioMixer.audioMixer.SetFloat("MusicVolume", -80f);
        }

        isMusicMuted = !isMusicMuted;
    }

    public void MuteSFX()
    {
        if (isSFXMuted)
        {
            musicAudioMixer.audioMixer.SetFloat("SFXVolume", 0f);
        }
        else
        {

            musicAudioMixer.audioMixer.SetFloat("SFXVolume", -80f);
        }
        isSFXMuted = !isSFXMuted;
    }
}
