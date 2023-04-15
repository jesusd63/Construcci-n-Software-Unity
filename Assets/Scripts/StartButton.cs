using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour{    
    public Button myButton; 
  void Start(){
        myButton.onClick.AddListener(LoadScene);
    }

    void LoadScene(){
        SceneManager.LoadScene("Start");
        Debug.Log("Button Clicked");
    }
}
