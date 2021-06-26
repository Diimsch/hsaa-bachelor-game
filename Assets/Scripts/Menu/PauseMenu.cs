using System;
using System.Collections;
using System.Collections.Generic;
using Menu;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

enum EPauseMenuItem
{
    Resume,
    Exit,
}

public class PauseMenu : MonoBehaviour, IMenu
{
    public Color activeTextColor;
    public Color normalTextColor;
    
    [SerializeField]
    private TMP_Text[] items;
    private EPauseMenuItem currentItem;

    [Header("Components")]
    public GameObject student;

    private void Start()
    {
        MenuManager.ActiveMenu = GetComponent<PauseMenu>();
    }

    public void OnUp(InputAction.CallbackContext ctx)
    {
        if (currentItem == EPauseMenuItem.Resume)
        {
            return;
        }

        items[(int) currentItem].color = normalTextColor;
        currentItem = EPauseMenuItem.Resume;
        items[(int) currentItem].color = activeTextColor;
    }

    public void OnDown(InputAction.CallbackContext ctx)
    {
        if (currentItem == EPauseMenuItem.Exit)
        {
            return;
        }

        items[(int) currentItem].color = normalTextColor;
        currentItem = EPauseMenuItem.Exit;
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
            case EPauseMenuItem.Resume:
                student.GetComponent<Player>().Resume();
                break;
            case EPauseMenuItem.Exit:
                 SceneManager.LoadScene("StartMenu");
;                break;
        }
    }

    public void OnEscape(InputAction.CallbackContext ctx)
    {
        student.GetComponent<Player>().Resume();
    }
}