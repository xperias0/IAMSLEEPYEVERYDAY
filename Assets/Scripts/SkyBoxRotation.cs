using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxRotation : MonoBehaviour
{
    // Start is called before the first frame update
    public Material mat;
    public float rotSpeed;
    private float angle;
    private int direction = 1;
    void Start()
    {
        angle = 34;
    }

    // Update is called once per frame
    void Update()
    {
        angle += direction * rotSpeed * Time.deltaTime;

        if (angle >= 268f || angle <= 33f)
        {
            print("Swithch");
            direction *= -1;
        }
        
        mat.SetFloat("_Rotation",angle);
        print(angle);
    }
}
