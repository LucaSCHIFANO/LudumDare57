using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private PlayerController target;
    private Vector3 hiddenTarget;

    [SerializeField] private Vector3 offset;

    [SerializeField] private float speed;
    [SerializeField] private float transitionSpeed;
    private float currentSpeed;


    [Header("Transition")]
    [SerializeField] private Room startingRoom;
    private Room currentRoom;
    private Vector2 cameraSize;
    [SerializeField] private float timeToWait;


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
            var htXpos = Mathf.Clamp(target.transform.position.x, currentRoom.WorldBottomLeftLimit.x + cameraSize.x / 2, currentRoom.WorldTopRightLimit.x - cameraSize.x / 2);
            var htYpos = Mathf.Clamp(target.transform.position.y, currentRoom.WorldBottomLeftLimit.y + cameraSize.y / 2, currentRoom.WorldTopRightLimit.y - cameraSize.y / 2);
            hiddenTarget = new Vector3(htXpos, htYpos, 0);
        }
        else hiddenTarget = target.transform.position;

        transform.position = Vector3.MoveTowards(transform.position, hiddenTarget + offset, currentSpeed * Time.deltaTime);
    }

    public void CamTransition(Transition transition)
    {
        if(transition.nextRoom == null)
        {
            Debug.Log("No next room");
            return;
        }

        currentRoom.ActiveRoom(false);
        currentRoom = transition.nextRoom;

        StartCoroutine(WaitToActivateRoom(transition.direction));
    }

    public IEnumerator WaitToActivateRoom(Transition.Direction dir)
    {
        target.ChangeState(PlayerController.State.CannotMove, dir);
        currentSpeed = transitionSpeed;

        yield return new WaitForSeconds(timeToWait);

        currentRoom.ActiveRoom(true);
        target.ChangeState(PlayerController.State.CanMove, Transition.Direction.NONE);
        currentSpeed = speed;
    }


    public Vector3 GetRestartPoint()
    {
        return currentRoom.CheckPoint.position;
    }
}