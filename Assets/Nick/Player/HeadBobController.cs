using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;
    [SerializeField, Range(0, 0.1f)] private float _amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private float _frequency = 10.0f;

    [SerializeField] private Transform _camera = null;
    [SerializeField] private Transform _cameraHolder = null;

    [SerializeField] private float _toggleSpeed;
    private Vector3 _startPos;
    private NickPlayerController _controller;
    private Rigidbody _rb;

    void Awake()
    {
        _controller = gameObject.GetComponent<NickPlayerController>();
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    void Start()
    {
        _startPos = _camera.localPosition;
    }
    
    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        //pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude * 2;
        return pos;
    }

    private void CheckMotion()
    {
        float speed = new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude;

        ResetPosition();

        if (speed < _toggleSpeed) return;
        if (!_controller.Grounded) return;

        _camera.position += FootStepMotion();
    }

    private void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, Time.deltaTime);
    }

    void Update()
    {
        if (!_enabled) return;

        CheckMotion();
        _camera.LookAt(FocusTarget());
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.localPosition.y, transform.position.z);
        pos += _cameraHolder.forward * 15.0f;
        return pos;
    }
}
