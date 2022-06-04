using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
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
        Move();
    }

    private void Move()
    {
        _xPos = Input.GetAxis("Horizontal");
        _zPos = Input.GetAxis("Vertical");
        _movementVector = transform.right * _xPos + transform.forward * _zPos;
        Controller.Move(_movementVector.normalized * speed * Time.deltaTime);
        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0)
        {
           
            FPSCam.Instance.safeStep = true;
        }
        else
        {
            FPSCam.Instance.safeStep = false;
        }
    }
}