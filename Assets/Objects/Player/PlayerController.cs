using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CollapsedManager collapsedManager;
    int maxBones = 0;
    private BoneHandler[] bones;

    public UnityEvent SwitchToTRexForme;
    public UnityEvent SwitchToCollapsedForme;

    private void Start()
    {
        bones = FindObjectsByType<BoneHandler>(FindObjectsSortMode.None);
        SwitchToTRexForme.Invoke();
        SetBonesActive(false);
    }

    public void ToRexFormeInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (collapsedManager.numberOfActiveBones < bones.Length)
                return;

            SwitchToTRexForme.Invoke();
            SetBonesActive(false);
        }
    }
    
    public void ToCollapsedFormeInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwitchToCollapsedForme.Invoke();
            SetBonesActive(true);
        }
    }

    private void SetBonesActive(bool value)
    {
        foreach (var bone in bones)
        {
            if (bone.gameObject == collapsedManager.gameObject)
                continue;
            bone.gameObject.SetActive(value);
        }
    }
}
