using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCam : MonoBehaviour
{
    [SerializeField] private float mouseSens = 1f;
    [SerializeField] private Transform player;
    private float _xRotation = 0f;
    private float _mouseX;
    private float _mouseY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        _mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        _mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;
        _xRotation -= _mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(new Vector3(_xRotation, 0f, 0f));
        player.Rotate(Vector3.up * _mouseX);
    }
}