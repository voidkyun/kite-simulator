using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BackCameraController : MonoBehaviour
{
    public float distance, initialAngle;
    float sensX,sensY,coefficient;
    public GameObject player,gmObj;
    PlayerMovement playerMovement;
    GameManager gameManager;
    Vector3 lastPlayerPosition;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        gameManager=gmObj.GetComponent<GameManager>();
        coefficient=playerMovement.coefficient;

        Vector3 offset = new Vector3(0, 0, -distance);
        transform.position = player.transform.position + offset;

        // Initialize last player position
        lastPlayerPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        sensX=playerMovement.sensX;
        sensY=playerMovement.sensY;
        if(!gameManager.UIopen){
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX*coefficient;
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY*coefficient;

            // Rotate around the player using player's transform axes
            transform.RotateAround(player.transform.position, Vector3.up, mouseX);
            transform.RotateAround(player.transform.position, transform.right, -mouseY);

            // Move the camera based on player's movement delta
            Vector3 playerMovementDelta = player.transform.position - lastPlayerPosition;
            transform.position += playerMovementDelta;

            // Update last player position
            lastPlayerPosition = player.transform.position;

            // Prevent the camera from moving in front of the player
            Vector3 directionToCamera = transform.position - player.transform.position;
            if (Vector3.Dot(player.transform.forward, directionToCamera) > 0)
            {
                //transform.position = player.transform.position - directionToCamera;
            }
        }
        
    }
}
