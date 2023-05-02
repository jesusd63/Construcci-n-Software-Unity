using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    
    [SerializeField]
    private string sceneName;

    public GameObject pauseMenu;

    public Player player;
    
    void Start(){
        pauseMenu.SetActive(false);
        player=GetComponent<Player>();
        Debug.Log(player);
    }

    public void StartGame(){
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    public void Quit(){
        Application.Quit();
    }

    public void Settings(){
        pauseMenu.SetActive(true);
    }

    public void Inicio(){
        SceneManager.LoadScene("Inicio");
    }
    public void Restart(){
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Continue(){
        //MAL
        // if(player==null){
        //     pauseMenu.SetActive(false);
        // }else{
        //         player.Pause();
        // }
    }
}
