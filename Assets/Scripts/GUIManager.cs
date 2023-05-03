using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    
    [SerializeField]
    private string sceneName;

    public AudioMixer _music_mixer;
    public AudioMixer _sound_mixer;

    public void StartGame(){
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    void Start(){
        _music_mixer.SetFloat("MusicVol", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1.0f)) * 20);
        _sound_mixer.SetFloat("SoundsVol", Mathf.Log10(PlayerPrefs.GetFloat("SoundsVolume", 1.0f)) * 20);
    }
}
