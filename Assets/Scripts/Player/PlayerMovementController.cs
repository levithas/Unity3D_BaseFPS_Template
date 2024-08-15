using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public bool CanMove = false;
    
    private Rigidbody _rigidbody;
    private GroundCheck _groundCheck;

    [SerializeField] private Transform hips;
    private Quaternion _hipsOriginRot;
    
    private Vector3 _movementControl;
    private float _pitch, _yaw;
    private bool _jump;
    
    // Start is called before the first frame update
    void Start()
    {
        _hipsOriginRot = hips.localRotation;
        _rigidbody = GetComponent<Rigidbody>();
        _groundCheck = GetComponentInChildren<GroundCheck>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(MoveDelay());
    }

    IEnumerator MoveDelay()
    {
        yield return new WaitForSeconds(1.0f);
        CanMove = true;
    }
    
    void UpdateControls()
    {
        _movementControl = Vector3.zero;

        if (!CanMove)
            return;

        _movementControl += Vector3.forward * InputManager.instance.GetAxis(InputManager.AxisName.Walk_ForwardBackward);
        _movementControl += Vector3.right * InputManager.instance.GetAxis(InputManager.AxisName.Walk_LeftRight);

        if (InputManager.instance.GetAction(InputManager.ActionName.Jump))
        {
            _jump = true;
        }

        _pitch += InputManager.instance.GetAxis(InputManager.AxisName.LookUpDown) * Time.deltaTime * 25.0f;
        _pitch = Mathf.Clamp(_pitch, -70.0f, 70.0f);
        _yaw += InputManager.instance.GetAxis(InputManager.AxisName.LookLeftRight) * Time.deltaTime * 25.0f;
    }

    void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(0.0f,_yaw, 0.0f);
        hips.transform.localRotation = _hipsOriginRot;
        hips.transform.localRotation *= Quaternion.Euler(_pitch, 0.0f,0.0f);
    }
    
    void UpdateMovement()
    {
        if (_groundCheck.IsGrounded)
        {
            float speed = 5.0f;
            Vector3 targetVelocity = transform.TransformVector(_movementControl * speed);
            Vector3 velocity = Vector3.Lerp(_rigidbody.velocity, targetVelocity, Time.fixedDeltaTime * 10.0f);
            velocity.y = _rigidbody.velocity.y;
            _rigidbody.velocity = velocity;

            if (_jump)
            {
                _rigidbody.AddForce(Vector3.up * 2.0f, ForceMode.VelocityChange);
            }
        }
        
        _jump = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateControls();
        UpdateRotation();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }
}
