using System;
using UnityEngine;

public class camscipt : MonoBehaviour
{
public Transform target;
[Range(0.05f,1f)] public float smoothSpeed = 0.15f;
public Vector3 offset = new Vector3(0,1,-10);
Vector3 velocity;
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
      transform.position = Vector3.SmoothDamp(
        transform.position,
        target.position + offset,
        ref velocity,
        smoothSpeed
      );
    }
}
