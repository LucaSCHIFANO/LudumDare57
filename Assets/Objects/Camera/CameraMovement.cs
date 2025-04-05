using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform target;
    private Vector3 hiddenTarget;

    [SerializeField] private Vector3 offset;

    [SerializeField] private float speed;
    private float currentSpeed;


    [Header("Transition")]
    [SerializeField] private Room startingRoom;
    private Room currentRoom;
    private Vector2 cameraSize;
    private float timeToWait;


    private static CameraMovement _instance = null;

    public static CameraMovement Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        _instance = this;

        var cam = GetComponent<Camera>();
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        cameraSize = new Vector2(width, height);

        currentSpeed = speed;

    }
    private void Start()
    {
        currentRoom = startingRoom;
        currentRoom.ActiveRoom(true);
    }


    private void FixedUpdate()
    {
        if (target == null) return;

        if (currentRoom != null)
        {
            var htXpos = Mathf.Clamp(target.position.x, currentRoom.WorldBottomLeftLimit.x + cameraSize.x / 2, currentRoom.WorldTopRightLimit.x - cameraSize.x / 2);
            var htYpos = Mathf.Clamp(target.position.y, currentRoom.WorldBottomLeftLimit.y + cameraSize.y / 2, currentRoom.WorldTopRightLimit.y - cameraSize.y / 2);
            hiddenTarget = new Vector3(htXpos, htYpos, 0);
        }
        else hiddenTarget = target.position;

        transform.position = Vector3.MoveTowards(transform.position, hiddenTarget + offset, currentSpeed * Time.deltaTime);
    }

    public void Transition(Transition transition)
    {
        currentRoom.ActiveRoom(false);
        currentRoom = transition.nextRoom;

        StartCoroutine(WaitToActivateRoom());
    }

    public IEnumerator WaitToActivateRoom()
    {
        yield return new WaitForSeconds(timeToWait);
        currentRoom.ActiveRoom(true);
    }
}