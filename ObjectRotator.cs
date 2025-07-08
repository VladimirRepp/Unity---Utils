using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private float _speed = 50f;
    [SerializeField] private Vector3 _axis = Vector3.up;

    void Update()
    {
        transform.Rotate(_axis * _speed * Time.deltaTime, Space.World);
    }
}
