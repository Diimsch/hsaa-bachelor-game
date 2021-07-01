using UnityEngine.InputSystem;

namespace Menu
{
    public interface IMenu
    {
        void OnUp(InputAction.CallbackContext ctx);
        void OnDown(InputAction.CallbackContext ctx);
        void OnLeft(InputAction.CallbackContext ctx);
        void OnRight(InputAction.CallbackContext ctx);
        void OnSelect(InputAction.CallbackContext ctx);
        void OnEscape(InputAction.CallbackContext ctx);
    }
}