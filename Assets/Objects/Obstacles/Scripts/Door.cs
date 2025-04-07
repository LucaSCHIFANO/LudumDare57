using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private List<ButtonDoor> buttonDoorList = new List<ButtonDoor>();
    private int buttonDoorCountPressed = 0;
    bool isClosed = true;
    [SerializeField] private DoorType doorType = DoorType.AllButton;
    [SerializeField, ShowIf("doorType", DoorType.MinimumButton), Min(1)] private int minimumButton;

    public enum DoorType
    {
        AllButton,
        MinimumButton,
    }

    [SerializeField] private GameObject doorCollider;

    private void Start()
    {
        foreach (var buttonDoor in buttonDoorList)
        {
            buttonDoor.Init(this);
        }
    }


    /// <param name="isPressed">True if pressed, false if released</param>
    public void ButtonDoorPressed(bool isPressed)
    {
        buttonDoorCountPressed = isPressed ? buttonDoorCountPressed+1 : buttonDoorCountPressed-1;
        switch (doorType)
        {
            case DoorType.AllButton:

                if (buttonDoorCountPressed == buttonDoorList.Count)
                {
                    Open(true);
                }
                else if(!isClosed) Open(false);
                break;

            case DoorType.MinimumButton:

                if (buttonDoorCountPressed >= minimumButton)
                {
                    Open(true);
                }
                else if(!isClosed) Open(false);
                break;

            default:
                break;
        }
        
    }

    private void Open(bool open)
    {
        isClosed = !open;
        doorCollider.SetActive(!open);
    }

}
