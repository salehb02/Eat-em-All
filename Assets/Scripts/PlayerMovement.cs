using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public VariableJoystick Joystick;

    private CharacterController _controller;
    private Vector3 _smoothedDirection;
    private Vector3 _previousPosition;
    private float _velocity;
    private Animator _animator;
    private Player _player;
    private float _currentSpeed;

    // Inputs
    private float _horizontalInput;
    private float _verticalInput;

    public bool Controllable { get; set; } = true;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        _currentSpeed = _player.PlayerSpeed;

        Inputs();
        Movement();
        GetVelocity();
    }

    private void Inputs()
    {
        if (!Controllable)
        {
            _horizontalInput = _verticalInput = 0f;
            return;
        }

        _horizontalInput = Joystick.Horizontal;
        _verticalInput = Joystick.Vertical;
    }

    private void Movement()
    {
        _smoothedDirection = Vector3.Lerp(_smoothedDirection, new Vector3(_horizontalInput, 0, _verticalInput), Time.deltaTime * 5f);

        _controller.SimpleMove(_smoothedDirection * Mathf.Lerp(_currentSpeed, _currentSpeed / 2f, _player.Fatness));

        if (_smoothedDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_smoothedDirection);

        _animator.SetFloat("Velocity", _velocity);
    }

    private void GetVelocity()
    {
        _velocity = ((transform.position - _previousPosition).magnitude) / Time.deltaTime;
        _previousPosition = transform.position;
    }
}