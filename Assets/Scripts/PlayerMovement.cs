using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public VariableJoystick Joystick;

    private CharacterController _controller;
    private Vector3 _smoothedDirection;
    private Vector3 _previousPosition;
    private Animator _animator;
    private Player _player;
    private float _currentSpeed;

    // Inputs
    private float _horizontalInput;
    private float _verticalInput;

    public float Velocity { get; private set; }
    public Transform CustomObjectLookAt { get; set; }

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
        _horizontalInput = Joystick.Horizontal;
        _verticalInput = Joystick.Vertical;
    }

    private void Movement()
    {
        _smoothedDirection = Vector3.Lerp(_smoothedDirection, new Vector3(_horizontalInput, 0, _verticalInput), Time.deltaTime * 5f);

        _controller.SimpleMove(_smoothedDirection * Mathf.Lerp(_currentSpeed, _currentSpeed / 2f, _player.Fatness));

        if (CustomObjectLookAt == null)
        {
            if (_smoothedDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(_smoothedDirection);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(CustomObjectLookAt.transform.position - transform.position);
        }

        _animator.SetFloat("Velocity", Velocity);
    }

    public void GetVelocity()
    {
        Velocity = ((transform.position - _previousPosition).magnitude) / Time.deltaTime;
        _previousPosition = transform.position;
    }
}