using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

enum EMainMenuItem
{
    Play,
    Exit,
}

public class MainMenu : MonoBehaviour
{
    public GameObject levelMenu; 
    
    public Color activeTextColor;
    public Color normalTextColor;
    
    [SerializeField]
    private TMP_Text[] items;
    private EMainMenuItem currentItem;

    [Header("Components")]
    public GameObject student;

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

    public void OnSelect(InputAction.CallbackContext ctx)
    {
     
        if (ctx.phase == InputActionPhase.Performed)
        {
            return;
        }

        if (ctx.phase == InputActionPhase.Canceled && currentItem == EMainMenuItem.Play)
        {
            levelMenu.GetComponent<PlayerInput>().enabled = true;
            return;
        }
        
        Debug.Log(ctx);
        
        switch (currentItem)
        {
            case EMainMenuItem.Play:
                gameObject.SetActive(false);
                gameObject.GetComponent<PlayerInput>().enabled = false;
                levelMenu.SetActive(true);
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