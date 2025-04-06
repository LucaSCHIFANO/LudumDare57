using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private Transform bottomLeftPoint;
    [SerializeField] private Transform topRightPoint;

    private Vector3 centralPoint;
    private Vector2 roomSize;
    public Vector3 WorldBottomLeftLimit { get => bottomLeftPoint.position; }
    public Vector3 WorldTopRightLimit { get => topRightPoint.position; }

    [SerializeField] private Color gizmosColor;

    [Header("Colliders")]
    private List<ColliderTransition> colliders = new List<ColliderTransition>();
    [SerializeField] private ColliderTransition colliderPrefab;
    [SerializeField] private float colliderSize;

    [Header("Transitions")]
    [SerializeField] private Transition transition;
    [SerializeField] private float transitionSize;

    [Header("CheckPoints")]
    [SerializeField] private Transform checkPoint;

    [Header("Obstacles")]
    [SerializeField] private List<GameObject> obstaclesList = new List<GameObject>();
     private List<Obstacle> trueObstaclesList = new List<Obstacle>();

    [Header("ActiveItem")]
    [SerializeField] private List<GameObject> activeItemList = new List<GameObject>();

    public Transform CheckPoint { get => checkPoint; }

    private void Awake()
    {
        centralPoint = new Vector3((bottomLeftPoint.position.x + topRightPoint.position.x) / 2,
            (bottomLeftPoint.position.y + topRightPoint.position.y) / 2,
            0);
        roomSize = new Vector2(topRightPoint.position.x - bottomLeftPoint.position.x,
            topRightPoint.position.y - bottomLeftPoint.position.y);
        ActiveRoom(false);
    }

    private void Start()
    {
        foreach (var obstacle in obstaclesList)
        {
            var obt = new Obstacle(obstacle, obstacle.transform.position);
            trueObstaclesList.Add(obt);
        }
    }

    public void ActiveRoom(bool isActive)
    {
        foreach (var collider in colliders)
        {
            Destroy(collider);
        }
        colliders.Clear();

        if (isActive) SetColliders();

        foreach (var item in activeItemList)
        {
            item.SetActive(isActive);
        }
    }

    private void SetColliders()
    {
        var rightSize = transition.direction == Transition.Direction.Right ? true : false;
        var leftSize = transition.direction == Transition.Direction.Left ? true : false;

        var bot = Instantiate(colliderPrefab, new Vector3(centralPoint.x, bottomLeftPoint.position.y, 0), Quaternion.identity, transform);
        colliders.Add(bot);
        bot.Init(false, new Vector2(roomSize.x, colliderSize), null);

        var right = Instantiate(colliderPrefab, new Vector3(topRightPoint.position.x, centralPoint.y, 0), Quaternion.identity, transform);
        colliders.Add(right);
        right.Init(rightSize, new Vector2(rightSize ? transitionSize : colliderSize, roomSize.y), rightSize ? transition : null);

        var top = Instantiate(colliderPrefab, new Vector3(centralPoint.x, topRightPoint.position.y, 0), Quaternion.identity, transform);
        colliders.Add(top);
        top.Init(false, new Vector2(roomSize.x, colliderSize), null);

        var left = Instantiate(colliderPrefab, new Vector3(bottomLeftPoint.position.x, centralPoint.y, 0), Quaternion.identity, transform);
        colliders.Add(left);
        left.Init(leftSize, new Vector2(leftSize ? transitionSize : colliderSize, roomSize.y), leftSize ? transition : null);
    }

    private void OnDrawGizmosSelected()
    {
        if (bottomLeftPoint == null || topRightPoint == null) return;

        var BL = new Vector2(bottomLeftPoint.transform.position.x, bottomLeftPoint.transform.position.y);
        var BR = new Vector2(topRightPoint.transform.position.x, bottomLeftPoint.transform.position.y);
        var TR = new Vector2(topRightPoint.transform.position.x, topRightPoint.transform.position.y);
        var TL = new Vector2(bottomLeftPoint.transform.position.x, topRightPoint.transform.position.y);

        Gizmos.color = gizmosColor;
        Gizmos.DrawLine(BL, BR);
        Gizmos.DrawLine(BR, TR);
        Gizmos.DrawLine(TR, TL);
        Gizmos.DrawLine(TL, BL);
    }

    public void Restart()
    {
        foreach (var obstacle in trueObstaclesList)
        {
            obstacle.go.SetActive(true);
            obstacle.go.transform.position = obstacle.position;
        }
    }
}

[Serializable]
public class Transition
{
    public Room nextRoom;
    public Direction direction;

    public enum Direction
    {
        Left,
        Right,
        NONE
    }
}

[Serializable]
public class Obstacle
{
    public GameObject go;
    public Vector3 position;

    public Obstacle(GameObject _go, Vector3 _position)
    {
        go= _go;
        position = _position;
    }
}