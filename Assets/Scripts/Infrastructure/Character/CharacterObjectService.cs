using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObjectService : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    [SerializeField] private bool gravity;
    [SerializeField] private float impactBuildUpSpeed = 10f;
    [SerializeField] private float impactDecaySpeed = 1f;
    [SerializeField] private float rotationSpeed = 10f;

    public bool IsGrounded => characterController.isGrounded;
    public Transform CharacterTransform => transform;
    public Vector3 Velocity => characterController.velocity;

    private float _verticalVelocity;
    private Vector3 _impactForce;
    private Vector3 _targetImpactForce;
    private Vector3 _dashForce;
    private float _dashTimer;
    private Vector3 _horizontalMove;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        Vector3 combinedMove = Vector3.zero;

        // 1. Handle Horizontal Movement
        combinedMove += _horizontalMove;
        _horizontalMove = Vector3.zero; // Reset after consuming

        // 2. Handle Gravity & Vertical Velocity
        if (gravity)
        {
            if (characterController.isGrounded && _verticalVelocity < 0)
            {
                _verticalVelocity = -2f;
            }
            _verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;
        }
        combinedMove.y += _verticalVelocity;

        // 3. Handle Impact Forces
        // Build up towards the target force
        _impactForce = Vector3.MoveTowards(_impactForce, _targetImpactForce, impactBuildUpSpeed * Time.fixedDeltaTime);
        // Decay the target force over time
        _targetImpactForce = Vector3.Lerp(_targetImpactForce, Vector3.zero, impactDecaySpeed * Time.fixedDeltaTime);

        if (_impactForce.magnitude > 0.1f)
        {
            combinedMove += _impactForce;
        }

        // 4. Handle Dash
        if (_dashTimer > 0)
        {
            combinedMove += _dashForce;
            _dashTimer -= Time.fixedDeltaTime;
            if (_dashTimer <= 0) _dashForce = Vector3.zero;
        }

        // Apply everything in one call per tick
        characterController.Move(combinedMove * Time.fixedDeltaTime);
    }

    public void ToggleGravity(bool status)
    {
        gravity = status;
        if (!status) _verticalVelocity = 0;
    }

    public void Move(Vector3 direction, float moveSpeed)
    {
        _horizontalMove = direction * moveSpeed;
    }

    public void SetVerticalVelocity(float velocity)
    {
        _verticalVelocity = velocity;
    }

    public void ApplyForce(Vector3 force)
    {
        _targetImpactForce += force;
    }

    public void RotateToDirection(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        transform.forward = direction;
    }

    public void FastFlyingRotate(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Vector3 flightUp = direction.normalized;
        Vector3 worldRelativeUp = Vector3.up;

        // Avoid gimbal lock/zero cross product when flying perfectly vertical
        if (Mathf.Abs(Vector3.Dot(flightUp, worldRelativeUp)) > 0.99f)
        {
            worldRelativeUp = Vector3.forward;
        }

        // Calculate a stable 'chest' direction that prevents the character from rolling/spinning
        Vector3 shoulders = Vector3.Cross(flightUp, worldRelativeUp).normalized;
        Vector3 chest = Vector3.Cross(shoulders, flightUp).normalized * -1;

        Quaternion targetRotation = Quaternion.LookRotation(chest, flightUp);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }


    public void Dash(Vector3 direction, float force, float duration)
    {
        _dashForce = direction.normalized * force;
        _dashTimer = duration;
    }
}
