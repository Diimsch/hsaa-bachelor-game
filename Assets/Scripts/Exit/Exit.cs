using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ExitPath
{
    MainMenu,
    Tutorial,
    Level1
};

public class Exit : MonoBehaviour
{
    public ExitPath exitPath;
    
    public void OnExit()
    {
        SceneManager.LoadScene("Scenes/StartMenu");
    }
}
