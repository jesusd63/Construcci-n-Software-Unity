using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    
    [SerializeField]
    private string sceneName;
    
    void Start()
    {
        
    }

    public void StartGame(){
        SceneManager.LoadScene(sceneName);
    }

    public void Quit(){
        Application.Quit();
    }

}
