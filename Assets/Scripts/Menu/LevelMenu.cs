using System;
using System.Collections;
using System.Collections.Generic;
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

public class LevelMenu : MonoBehaviour
{
    public GameObject startMenu; 
    
    public Color activeTextColor;
    public Color normalTextColor;
    
    [SerializeField]
    private TMP_Text[] items;
    private EMenuItem currentItem;

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
    }
    
    public void OnRight(InputAction.CallbackContext ctx)
    {
    }

    
    public void OnSelect(InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx);
        if (ctx.phase == InputActionPhase.Performed)
        {
            return;
        }
        
        if (ctx.phase == InputActionPhase.Canceled && currentItem == EMenuItem.Back)
        {
            startMenu.GetComponent<PlayerInput>().enabled = true;
            return;
        }
        
        Debug.Log(ctx);
        
        switch (currentItem)
        {
            case EMenuItem.LevelSelection:
                StartCoroutine(LoadLevel());
                break;
            case EMenuItem.Back:
                gameObject.SetActive(false);
                gameObject.GetComponent<PlayerInput>().enabled = false;
                startMenu.SetActive(true);
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