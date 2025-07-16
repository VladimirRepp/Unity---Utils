using UnityEngine;

public class SmoothLookAtTarget : MonoBehaviour
{
    [Header("Common Settings")]
    [SerializeField]
    [Tooltip("Quaternion.Slerp – классический плавный поворот. " +
    "RotateTowards – более предсказуемый, но менее \"плавный\" на больших углах.  " +
    "SmoothDamp – очень плавный, но может быть избыточным для простых случаев.")]
    private EMethodSmoothRotation _method = EMethodSmoothRotation.Quaternion_Slerp;
    [SerializeField] private Transform _target;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private bool _isInvertDirection = true;

    [Header("LookAt + SmoothDamp Settings")]
    [SerializeField][Tooltip("Время сглаживания")] private float smoothTime = 0.3f;
    private Vector3 currentVelocity;

    public Transform Target
    {
        get => _target;
        set => _target = value;
    }

    void Update()
    {
        if (_target == null) return;

        switch (_method)
        {
            case EMethodSmoothRotation.Quaternion_Slerp:
                QuaternionSlerp_Method();
                break;

            case EMethodSmoothRotation.Transform_Rotate:
                TransformRotate_Method();
                break;


            case EMethodSmoothRotation.LookAt_SmoothDamp:
                LookAtSmoothDamp_Method();
                break;
        }
    }

    private void QuaternionSlerp_Method()
    {
        Vector3 direction = Vector3.zero;
        if (_isInvertDirection)
            direction = transform.position - _target.position;
        else
            direction = _target.position - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }
    }

    private void TransformRotate_Method()
    {
        Vector3 direction = Vector3.zero;
        if (_isInvertDirection)
            direction = transform.position - _target.position;
        else
            direction = _target.position - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            if (angleDifference > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    private void LookAtSmoothDamp_Method()
    {
        Vector3 direction = Vector3.zero;
        if (_isInvertDirection)
            direction = transform.position - _target.position;
        else
            direction = _target.position - transform.position;

        if (direction != Vector3.zero)
        {
            Vector3 smoothDirection = Vector3.SmoothDamp(
                transform.forward,
                direction,
                ref currentVelocity,
                smoothTime
            );

            transform.rotation = Quaternion.LookRotation(smoothDirection);
        }
    }
}

public enum EMethodSmoothRotation
{
    Quaternion_Slerp,
    Transform_Rotate,
    LookAt_SmoothDamp
}