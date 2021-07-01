using System.Collections;
using System.Collections.Generic;
using Menu;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour, Menu.IMenu
{
    private static IMenu _activeMenu;

    public static IMenu ActiveMenu
    {
        get { return _activeMenu; }
        set { _activeMenu = value; }
    }

    public void OnUp(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started)
        {
            return;
        }
        _activeMenu.OnUp(ctx);
    }

    public void OnDown(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started)
        {
            return;
        }
        _activeMenu.OnDown(ctx);
    }

    public void OnLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started)
        {
            return;
        }
        _activeMenu.OnLeft(ctx);
    }

    public void OnRight(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started)
        {
            return;
        }
        _activeMenu.OnRight(ctx);
    }

    public void OnSelect(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started)
        {
            return;
        }
        _activeMenu.OnSelect(ctx);
    }

    public void OnEscape(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started)
        {
            return;
        }
        _activeMenu.OnEscape(ctx);
    }
}
