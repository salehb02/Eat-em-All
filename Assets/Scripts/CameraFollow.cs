using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothness = 5f;
    public bool lookAt = false;
    public Vector3 lookAtOffset;

    private void Start()
    {
        if (target)
            transform.position = target.transform.position + offset;
    }

    private void LateUpdate()
    {
        if (!target)
            return;

        transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, Time.deltaTime * smoothness);

        if (lookAt)
            transform.LookAt(target.position + lookAtOffset);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        transform.position = target.transform.position + offset;
    }
}