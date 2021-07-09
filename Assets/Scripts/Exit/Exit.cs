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
        switch (exitPath)
        {
            case ExitPath.MainMenu:
                SceneManager.LoadScene("Scenes/StartMenu");
                break;
            case ExitPath.Tutorial:
                SceneManager.LoadScene("Scenes/Tutorial");
                break;
            case ExitPath.Level1:
                SceneManager.LoadScene("Scenes/Level 1");
                break;
        }
    }
}
