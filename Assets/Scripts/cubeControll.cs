using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeControll : MonoBehaviour
{
    public enum direction
    {
        Left,
        Right,
        Up,
        Down
    }
    // Start is called before the first frame update
    public bool isUp;
    public int step;
    public float moveSpeed = 10f;
    public Transform targetCube;

    private bool isUsed = false;

    private bool startMove = false;

    private Vector3 targetPosition;

    private int length;
    // Update is called once per frame

    private void Start()
    {
        length = step * 2;
        targetPosition = isUp == true ? targetCube.position + targetCube.up * length 
                                      : targetCube.position + ((-targetCube.up) * length);
    }

    private void Update()
    {
        if (startMove)
        {
            targetCube.position = Vector3.Lerp(targetCube.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, transform.position + (-transform.up) * 1.5f, 0.1f * Time.deltaTime);
            if (Vector3.Distance(targetCube.position,targetPosition)<0.1f)
            {
                startMove = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isUsed && other.gameObject.CompareTag("Player"))
        {
            print("Press");
            startMove = true;
            isUsed = true;
        }
    }

    private void OnColliderEnter(Collider other)
    {
       
    }
}
