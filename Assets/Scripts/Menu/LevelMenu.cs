using System;
using System.Collections;
using System.Collections.Generic;
using Menu;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

enum EMenuItem
{
    LevelSelection = 0,
    Back,
}

enum ELevels
{
    Tutorial,
    Level_1,
    Level_2
}

public class LevelMenu : MonoBehaviour, IMenu
{
    public GameObject startMenu; 
    
    public Color activeTextColor;
    public Color normalTextColor;
    
    [SerializeField]
    private TMP_Text[] items;
    private EMenuItem currentItem = EMenuItem.LevelSelection;
    private ELevels currentLevel = ELevels.Tutorial;

    [Header("Components")]
    public GameObject student;

    public void OnUp(InputAction.CallbackContext ctx)
    {
        if (currentItem == EMenuItem.LevelSelection)
        {
            return;
        }

        items[(int) currentItem].color = normalTextColor;
        currentItem = EMenuItem.LevelSelection;
        items[(int) currentItem].color = activeTextColor;
    }

    public void OnDown(InputAction.CallbackContext ctx)
    {

        if (currentItem == EMenuItem.Back)
        {
            return;
        }

        items[(int) currentItem].color = normalTextColor;
        currentItem = EMenuItem.Back;
        items[(int) currentItem].color = activeTextColor;
    }
    
    public void OnLeft(InputAction.CallbackContext ctx)
    {
        if (currentItem != EMenuItem.LevelSelection || currentLevel == ELevels.Tutorial)
        {
            return;
        }

        currentLevel--;
        items[(int) currentItem].text = currentLevel.ToString().Replace("_", " ");
    }
    
    public void OnRight(InputAction.CallbackContext ctx)
    {
        if (currentItem != EMenuItem.LevelSelection || currentLevel == ELevels.Level_2)
        {
            return;
        }

        currentLevel++;
        items[(int) currentItem].text = currentLevel.ToString().Replace("_", " ");
    }

    
    public void OnSelect(InputAction.CallbackContext ctx)
    {
        switch (currentItem)
        {
            case EMenuItem.LevelSelection:
                StartCoroutine(LoadLevel());
                break;
            case EMenuItem.Back:
                gameObject.SetActive(false);
                startMenu.SetActive(true);
                MenuManager.ActiveMenu = startMenu.GetComponent<MainMenu>();
                break;
        }
    }

    IEnumerator LoadLevel()
    {
        student.GetComponent<Player>().Jump();
        yield return new WaitForSeconds(0.55f);

        switch (currentLevel)
        {
            case ELevels.Tutorial:
                SceneManager.LoadScene("Tutorial");
                break;
            case ELevels.Level_1:
                SceneManager.LoadScene("Game");
                break;
            case ELevels.Level_2:
                SceneManager.LoadScene("Level");
                break;
        }
    }
}