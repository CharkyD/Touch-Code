using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CamControls : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemaCam;
    [SerializeField] private float speed = 25f;
    [SerializeField] private float fovMax = 130f;
    [SerializeField] private float fovMin = 10f;
    [SerializeField] private Camera cam;
    private float zoomSpeed = 5f;
    private float targetFOV = 130f;
    private Vector2 moveDir;
    private InputAction touchPos0;
    private InputAction touchPos1;
    private InputAction touchDelta0;
    private InputAction touchDelta1;
    public bool isPinching = false;

    private void Awake()
    {
        touchPos0 = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch0/position");
        touchPos1 = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch1/position");
        touchDelta0 = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch0/delta");
        touchDelta1 = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch1/delta");

        touchPos0.Enable();
        touchPos1.Enable();
        touchDelta0.Enable();
        touchDelta1.Enable();
    }
    private void OnEnable()
    {
        touchPos1.performed += OnPinch;
    }
    private void OnDisable()
    {
        touchPos1.performed -= OnPinch;
    }
    private void Update()
    {
        if (!isPinching)
        {
            transform.position += new Vector3(moveDir.x, moveDir.y, 0f) * speed * Time.deltaTime;
        }
        ZoomHandler();
    }
    public void Movement(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }
    public void Pinch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isPinching = true;
        }
        if (context.canceled)
        {
            isPinching = false;
        }
    }

    private void OnPinch(InputAction.CallbackContext context)
    {
        Vector2 touch0Prev = (touchPos0.ReadValue<Vector2>() - touchDelta0.ReadValue<Vector2>());
        Vector2 touch1Prev = (touchPos1.ReadValue<Vector2>() - touchDelta1.ReadValue<Vector2>());

        float prevMagnitude = (touch0Prev - touch1Prev).magnitude;
        float currentMagnitude = (touchPos0.ReadValue<Vector2>() - touchPos1.ReadValue<Vector2>()).magnitude;

        if(prevMagnitude > currentMagnitude)
        {
            targetFOV += 5F;
        }else if(prevMagnitude < currentMagnitude)
        {
            targetFOV -= 5f;
        }
    }
    private void ZoomHandler()
    {
        targetFOV = Mathf.Clamp(targetFOV,fovMin,fovMax);
        cinemaCam.m_Lens.FieldOfView = Mathf.Lerp(cinemaCam.m_Lens.FieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }
}
