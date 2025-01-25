using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateArrowCamera : MonoBehaviour
{
    public GameObject Player,TPCamera;
    public float angle;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float dangle=0;
        if(Player.GetComponent<PlayerMovement>().cameraMode==PlayerMovement.CameraMode.FirstPerson){
            dangle=Player.transform.rotation.eulerAngles.y-angle;
            angle=Player.transform.rotation.eulerAngles.y;
        }else if(Player.GetComponent<PlayerMovement>().cameraMode==PlayerMovement.CameraMode.ThirdPerson){
            dangle=TPCamera.transform.rotation.eulerAngles.y-angle;
            angle=TPCamera.transform.rotation.eulerAngles.y;
        }
        transform.RotateAround(Vector3.zero,Vector3.up,dangle);        
    }
}
