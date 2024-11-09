using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateArrowCamera : MonoBehaviour
{
    public GameObject Player;
    public float angle;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float dangle=Player.transform.rotation.eulerAngles.y-angle;
        angle=Player.transform.rotation.eulerAngles.y;
        transform.RotateAround(Vector3.zero,Vector3.up,dangle);        
    }
}
