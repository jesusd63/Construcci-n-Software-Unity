using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PauseMenu: MonoBehaviour{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _pausebutton; 
    public void QuitGame(){
    Application.Quit();
    }
    public void Menu(){
    SceneManager.LoadScene("Inicio");
    }
    public void PauseButton(){
    Time.timeScale = 0.0f;
    }
    public void ResumeButton(){
    Time.timeScale = 1.0f;
    _pauseMenu.SetActive(false);
    _pausebutton.SetActive(true);
    }
}
