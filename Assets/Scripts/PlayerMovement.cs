using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public VariableJoystick Joystick;

    private CharacterController _controller;
    private Vector3 _smoothedDirection;
    private Vector3 _previousPosition;
    private Animator _animator;
    private Player _player;

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

        _controller.SimpleMove(_smoothedDirection * Mathf.Lerp(_player.PlayerNormalSpeed, _player.PlayerFatSpeed, _player.Fatness));

        if (CustomObjectLookAt == null)
        {
            if (_smoothedDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(_smoothedDirection);
        }
        else
        {

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(CustomObjectLookAt.transform.position - transform.position), Time.deltaTime * 5f);
        }

        _animator.SetFloat("Velocity", Velocity);
    }

    public void GetVelocity()
    {
        var heightFixedCurrentPos = transform.position;
        heightFixedCurrentPos.y = 0;

        var heightFixedPreviousPos = _previousPosition;
        heightFixedPreviousPos.y = 0;

        Velocity = ((heightFixedCurrentPos - heightFixedPreviousPos).magnitude) / Time.deltaTime;
        _previousPosition = transform.position;
    }
}