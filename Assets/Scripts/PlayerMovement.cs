using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController Controller;
    public float speed = 12f;

    private float _xPos;
    private float _zPos;
    private Vector3 _movementVector;

    void Update()
    {
        _xPos = Input.GetAxis("Horizontal");
        _zPos = Input.GetAxis("Vertical");
        _movementVector = transform.right * _xPos + transform.forward * _zPos;
        Controller.Move(_movementVector *speed* Time.deltaTime);
    }
}