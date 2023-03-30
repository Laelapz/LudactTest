using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [Header("Ship Data")]
    public float Speed = 1f;

    private Rigidbody _myRigidbody;
    private Action<ShipMovement> _destroyAction;

    private void Awake()
    {
        _myRigidbody = GetComponentInChildren<Rigidbody>();
    }

    public void Init(Action<ShipMovement> destroyAction)
    {
        _destroyAction = destroyAction;
    }

    private void Start()
    {
        StartCoroutine(TimeBeforeDestroy());
    }

    private void OnEnable()
    {
        _myRigidbody.velocity = Vector3.zero;
        StartCoroutine(TimeBeforeDestroy());
    }

    private void FixedUpdate()
    {
        _myRigidbody.velocity += (transform.up * Speed);
    }

    private IEnumerator TimeBeforeDestroy()
    {
        yield return new WaitForSeconds(5f);
        _destroyAction.Invoke(this);
    }
}
