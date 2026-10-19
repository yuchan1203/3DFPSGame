using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private void Awake()
    {
        if (_target == null)
        {
            Debug.LogError("No target assigned!");
        }
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }
        transform.position = _target.position;
    }
}