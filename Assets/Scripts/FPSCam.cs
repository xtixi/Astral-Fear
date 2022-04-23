using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FPSCam : MonoBehaviour
{
    public static FPSCam Instance;
    [SerializeField] private float mouseSens = 1f;
    [SerializeField] private Transform player;
    private float _xRotation = 0f;
    private float _mouseX;
    private float _mouseY;
    private Transform _head;
    private Tween _rightStep;
    private Tween _leftStep;
    private bool _stepName; //true = left, false = right;

    [SerializeField] private float stepDuration = 0.2f;

    //[Range(0f, 1f)] private float _lerpValue;
    public bool safeStep;
    private bool _stepLock = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        DefineSteps();
        Step();
    }

    void Update()
    {
        // Debug.Log("steplock: " + _stepLock);
        //Debug.Log("safestep: " + safeStep);
        Look();
        if (safeStep && _stepLock == false)
        {
            Step();
        }
    }

    private void FixedUpdate()
    {
        throw new NotImplementedException();
    }

    private void Look()
    {
        _mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        _mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;
        _xRotation -= _mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(new Vector3(_xRotation, 0f, 0f));
        player.Rotate(Vector3.up * _mouseX);
    }


    private void Step()
    {
        Debug.Log("stepped");
        if (_stepName)
        {
            //_leftStep.Play();
            _head
                .DORotate(_head.rotation.eulerAngles + new Vector3(-3f, 0, -3f), stepDuration)
                .SetEase(Ease.OutCirc).OnComplete(() =>
                {
                    _head.DORotate(
                            _head.rotation.eulerAngles + new Vector3(3f, 0f, 3f), stepDuration).SetEase(Ease.OutCirc)
                        .OnComplete(
                            () => { _stepLock = false; });
                });
            _stepLock = true;
            _stepName = !_stepName;
        }
        else
        {
            //_rightStep.Play();
            _head
                .DORotate(_head.rotation.eulerAngles + new Vector3(3f, 0, 3f), stepDuration)
                .SetEase(Ease.OutCirc).OnComplete(() =>
                {
                    _head.DORotate(
                            _head.rotation.eulerAngles + new Vector3(-3f, 0f, -3f), stepDuration).SetEase(Ease.OutCirc)
                        .OnComplete(
                            () => { _stepLock = false; });
                });
            _stepLock = true;
            _stepName = !_stepName;
        }
    }


    private void DefineSteps()
    {
        _head = transform;
        _rightStep =
            _head
                .DORotate(_head.rotation.eulerAngles + new Vector3(3f, 0, 3f), stepDuration)
                .SetEase(Ease.OutCirc).OnComplete(() =>
                {
                    _head.DORotate(
                            _head.rotation.eulerAngles + new Vector3(-3f, 0f, -3f), stepDuration).SetEase(Ease.OutCirc)
                        .OnComplete(
                            () => { _stepLock = false; });
                });
        _leftStep =
            _head
                .DORotate(_head.rotation.eulerAngles + new Vector3(-3f, 0, -3f), stepDuration)
                .SetEase(Ease.OutCirc).OnComplete(() =>
                {
                    _head.DORotate(
                            _head.rotation.eulerAngles + new Vector3(3f, 0f, 3f), stepDuration).SetEase(Ease.OutCirc)
                        .OnComplete(
                            () => { _stepLock = false; });
                });
    }
}