using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{

    public AudioMixer _music_mixer;
    public Slider _music_slider;
    public AudioMixer _sound_mixer;
    public Slider _sound_slider;

    void Start(){
        _music_slider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        _sound_slider.value = PlayerPrefs.GetFloat("SoundsVolume", 1.0f);
    }

    public void SetMusicVol (float sliderValue){
        _music_mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void SetSoundVol (float sliderValue){
        _sound_mixer.SetFloat("SoundsVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SoundsVolume", sliderValue);
    }
}
