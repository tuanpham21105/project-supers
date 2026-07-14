using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    private Transform horizontalRotator;
    private Transform verticalRotator;
    private Transform cameraObject;
    [SerializeField] private Transform character;
    [SerializeField] private SpeedLinesVfxController speedLinesVfxController;
    private float yaw;
    private float pitch;
    [SerializeField] private Camera _cam;
    [SerializeField] private Camera targetVfxCamera;
    [Header("Collision Settings")]
    [SerializeField] private float cameraRadius = 0.2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, -5f);
    [Tooltip("Layers the camera should hit.")]
    [SerializeField] private LayerMask includeLayers = ~0;
    [Tooltip("Layers the camera should ignore.")]
    [SerializeField] private LayerMask excludeLayers;

    [SerializeField] private int minFov = 70;
    [SerializeField] private int maxFov = 120;
    [SerializeField] private int maxPow2MoveSpeed = 5000;
    [SerializeField] private float fovSmoothTime = 0.2f;
    private float fovVelocity;

    [Header("Target Lock Settings")]
    private bool isTargetLocking;
    private Transform targetLockTarget;
    [SerializeField] private float targetLockMaxRotateSpeed = 360f;

    void Awake()
    {
        instance = this;
    }

    void OnDestroy()
    {
        instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        horizontalRotator = transform;
        verticalRotator = transform.GetChild(0);
        cameraObject = verticalRotator.GetChild(0);

        // _cam = cameraObject.GetComponent<Camera>();

        // Initialize from current rotation
        yaw = horizontalRotator.eulerAngles.y;
        pitch = verticalRotator.localEulerAngles.x;
        if (pitch > 180) pitch -= 360;
    }

    void LateUpdate()
    {
        if (character != null)
        {
            horizontalRotator.position = character.position;
        }

        HandleCameraCollision();
    }

    private void HandleCameraCollision()
    {
        // Direction from pivot to desired camera position based on offset
        Vector3 desiredLocalPos = offset;
        Vector3 desiredWorldPos = verticalRotator.TransformPoint(desiredLocalPos);
        Vector3 dir = (desiredWorldPos - verticalRotator.position).normalized;
        float maxDistance = offset.magnitude;
        int layerMask = includeLayers.value & ~excludeLayers.value;
        RaycastHit hit;

        if (Physics.SphereCast(verticalRotator.position, cameraRadius, dir, out hit, maxDistance, layerMask))
        {
            // If we hit something, place camera at hit point (slightly offset forward to avoid clipping into the hit wall)
            cameraObject.position = hit.point + (verticalRotator.position - hit.point).normalized * 0.1f;
        }
        else
        {
            // Otherwise, stay at default offset
            cameraObject.localPosition = desiredLocalPos;
        }
    }

    public Vector3 GetCameraDirection()
    {
        return verticalRotator.forward;
    }

    public void Rotate(Vector2 lookInput)
    {
        if (isTargetLocking) return;

        yaw += lookInput.x;
        pitch -= lookInput.y;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        horizontalRotator.rotation = Quaternion.Euler(0, yaw, 0);
        verticalRotator.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    public void TargetLock(Transform target)
    {
        isTargetLocking = true;
        targetLockTarget = target;
        target.GetComponent<CharacterObjectsData>().targetLockVfx.SetActive(true);
    }

    public void RemoveTargetLock()
    {
        targetLockTarget.GetComponent<CharacterObjectsData>().targetLockVfx.SetActive(false);
        isTargetLocking = false;
        targetLockTarget = null;

        yaw = horizontalRotator.eulerAngles.y;
        pitch = verticalRotator.localEulerAngles.x;
        if (pitch > 180) pitch -= 360;
    }

    public void SetCharacter(Transform gameObject)
    {
        character = gameObject;
    }

    void FixedUpdate()
    {
        SetCameraWithMoveSpeed();

        if (isTargetLocking && targetLockTarget != null)
        {
            Vector3 dir = targetLockTarget.position - horizontalRotator.position;
            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                horizontalRotator.rotation = Quaternion.RotateTowards(
                    horizontalRotator.rotation,
                    targetRot,
                    targetLockMaxRotateSpeed * Time.fixedDeltaTime);
            }
        }
    }

    void SetCameraWithMoveSpeed()
    {
        float pow2MoveSpeed = character.GetComponent<CharacterStatesData>().currentPow2AllSpeed;

        speedLinesVfxController.SetMoveSpeed(pow2MoveSpeed);

        float t = Mathf.Clamp01(pow2MoveSpeed / maxPow2MoveSpeed);
        float targetFov = Mathf.Lerp(minFov, maxFov, t);

        _cam.fieldOfView = Mathf.SmoothDamp(
            _cam.fieldOfView,
            targetFov,
            ref fovVelocity,
            fovSmoothTime);

        targetVfxCamera.fieldOfView = _cam.fieldOfView;
    }
}
