using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool _paused = false;
    public GameObject pauseMenu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel")){
            if(_paused == true){
                Resume();
            }
            else{
                Pause();
            }
        }
    }

    public void Resume(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        _paused = false;
    }

    public void Pause(){
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        _paused = true;
    }

    public void Restart(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Inicio(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("Inicio");
    }

    public void Quit(){
        Application.Quit();
    }
}
