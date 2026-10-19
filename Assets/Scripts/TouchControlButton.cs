using UnityEngine;
using UnityEngine.EventSystems;

public class TouchControlButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum Control { Left, Right, Jump }

    [SerializeField] private PlayerController2D player;
    [SerializeField] private Control control;
    private int activePointerId = int.MinValue;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (player == null || activePointerId != int.MinValue) return;
        activePointerId = eventData.pointerId;
        if (control == Control.Left) player.SetLeftHeld(true);
        if (control == Control.Right) player.SetRightHeld(true);
        if (control == Control.Jump) player.RequestJump();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == activePointerId) Release();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerId == activePointerId) Release();
    }

    private void OnDisable() { Release(); }

    private void Release()
    {
        if (activePointerId == int.MinValue) return;
        if (player != null)
        {
            if (control == Control.Left) player.SetLeftHeld(false);
            if (control == Control.Right) player.SetRightHeld(false);
        }
        activePointerId = int.MinValue;
    }
}
