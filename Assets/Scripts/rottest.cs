using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rottest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation=Quaternion.AngleAxis(90f, Vector3.forward)*Quaternion.AngleAxis(90f, Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
