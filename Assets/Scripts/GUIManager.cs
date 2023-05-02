using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    
    [SerializeField]
    private string sceneName;

    public void StartGame(){
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
}
