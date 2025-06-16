using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;

    void Update()
    {
        transform.Rotate(_rotationAxis * _rotationSpeed * Time.deltaTime, Space.World);
    }
}
