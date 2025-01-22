using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public GameObject player,kite;
    CinemachineVirtualCamera vcam;
    CinemachineTransposer transposer;
    float initialDistance,distance;
    // Start is called before the first frame update
    void Start()
    {
        initialDistance=(player.transform.position-kite.transform.position).magnitude;
        vcam=GetComponent<CinemachineVirtualCamera>();
        transposer=vcam.GetCinemachineComponent<CinemachineTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        distance=(player.transform.position-kite.transform.position).magnitude;
        print(transposer);
        transposer.m_FollowOffset.z=-27*(distance/initialDistance);
    }
}
