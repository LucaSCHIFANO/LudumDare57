using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [SerializeField] private List<Room> listRoom = new List<Room>();
    [SerializeField, Min(0)] private int roomIndex = 0;

    [SerializeField] private bool isDebug = false;

    private static CheckPointManager _instance = null;

    public static CheckPointManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    public Vector3 GetRestartPoint()
    {
        if (isDebug) 
        {
            if (roomIndex >= listRoom.Count) return Vector3.zero;
            CameraMovement.Instance.InstanteCamTransition(listRoom[roomIndex]);
            return CameraMovement.Instance.GetRestartPoint();
        }
        else
        {
            return CameraMovement.Instance.GetRestartPoint();
        }
    }
}
