using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public VariableJoystick Joystick;
    public float MovementSpeed = 2f;

    private CharacterController _controller;
    private Vector3 _smoothedDirection;

    // Inputs
    private float _horizontalInput;
    private float _verticalInput;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Inputs();
        Movement();
    }

    private void Inputs()
    {
        _horizontalInput = Joystick.Horizontal;
        _verticalInput = Joystick.Vertical;
    }

    private void Movement()
    {
        _smoothedDirection = Vector3.Lerp(_smoothedDirection, new Vector3(_horizontalInput, 0, _verticalInput), Time.deltaTime * 5f);

        _controller.SimpleMove(_smoothedDirection * MovementSpeed);

        if (_smoothedDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_smoothedDirection);
    }
}