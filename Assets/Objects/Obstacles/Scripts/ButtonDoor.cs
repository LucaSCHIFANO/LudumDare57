using System.Collections.Generic;
using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    private Door door;
    private List<Collider2D> colliders = new List<Collider2D>();
    private bool isPressed = false;
    public void Init(Door _door)
    {
        door = _door;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" || collision.tag == "Bone" || collision.tag == "Obstacle")
        {
            colliders.Add(collision);
            if(!isPressed)
            {
                isPressed = true;
                door.ButtonDoorPressed(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Bone")
        {
            colliders.Remove(collision);

            if(colliders.Count == 0)
            {
                isPressed = false;
                door.ButtonDoorPressed(false);
            }
        }
    }
}
