using System;
using System.Collections;
using System.Collections.Generic;
using Menu;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

enum EMainMenuItem
{
    Play,
    Exit,
}

public class MainMenu : MonoBehaviour, IMenu
{
    public GameObject levelMenu; 
    
    public Color activeTextColor;
    public Color normalTextColor;
    
    [SerializeField]
    private TMP_Text[] items;
    private EMainMenuItem currentItem;

    [Header("Components")]
    public GameObject student;

    private void Start()
    {
        MenuManager.ActiveMenu = GetComponent<MainMenu>();
    }

    public void OnUp(InputAction.CallbackContext ctx)
    {
        if (currentItem == EMainMenuItem.Play)
        {
            return;
        }

        items[(int) currentItem].color = normalTextColor;
        currentItem = EMainMenuItem.Play;
        items[(int) currentItem].color = activeTextColor;
    }

    public void OnDown(InputAction.CallbackContext ctx)
    {
        if (currentItem == EMainMenuItem.Exit)
        {
            return;
        }

        items[(int) currentItem].color = normalTextColor;
        currentItem = EMainMenuItem.Exit;
        items[(int) currentItem].color = activeTextColor;
    }

    public void OnLeft(InputAction.CallbackContext ctx)
    {
        return;
    }

    public void OnRight(InputAction.CallbackContext ctx)
    {
        return;
    }

    public void OnSelect(InputAction.CallbackContext ctx)
    {
        switch (currentItem)
        {
            case EMainMenuItem.Play:
                gameObject.SetActive(false);
                levelMenu.SetActive(true);
                MenuManager.ActiveMenu = levelMenu.GetComponent<LevelMenu>();
                break;
            case EMainMenuItem.Exit:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }

    IEnumerator LoadLevel()
    {
        student.GetComponent<Player>().Jump();
        yield return new WaitForSeconds(0.55f);

        SceneManager.LoadScene("Game");

    }
}