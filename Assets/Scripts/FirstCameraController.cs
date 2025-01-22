using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstCameraController : MonoBehaviour
{
    float sensX,sensY,coefficient,yAngle,xAngle;
    public GameObject Player,gmObj;
    PlayerMovement playerMovement;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
        gameManager=gmObj.GetComponent<GameManager>();
        coefficient=playerMovement.coefficient;
    }

    // Update is called once per frame
    void Update()
    {
        sensX=playerMovement.sensX;
        sensY=playerMovement.sensY;
        if(!gameManager.UIopen){
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX*coefficient;
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY*coefficient;
            yAngle+=mouseX;
            xAngle-=mouseY;
            if(xAngle>90f){
                xAngle=90f;
            }else if(xAngle<-90f){
                xAngle=-90f;
            }
        }
        transform.localRotation=Quaternion.Euler(xAngle,0f,0f);
    }
}
