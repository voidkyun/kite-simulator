using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    public float Acceleration,MaxSpeed,Sprint,JumpHeight,groundDrag,gravity;
    Rigidbody rb;
    public LayerMask Ground;
    public bool grounded,isSprint;
    public float sensX,sensY;
    public float yAngle;
    public float coefficient;
    public GameObject gmObj,thirdPersonCamera;
    public GameManager gameManager;
    public AudioSource[] audioSources;
    public PlayerStatus stat,laststat;
    private Vector3 translation;
    private float tsAngle;
    public Animator animator;
    public Renderer m_renderer;

    public CameraMode cameraMode; 
    public enum CameraMode{
        FirstPerson,
        ThirdPerson,
    }

    public bool Started=false;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        gameManager=gmObj.GetComponent<GameManager>();
        yield return new WaitUntil(() => gameManager.PlayerAvatarLoaded);
        animator=gameManager.PlayerAvatar.GetComponent<Animator>();
        m_renderer=GetComponent<Renderer>();
        m_renderer.enabled = false;
        audioSources=GetComponents<AudioSource>();
        rb=GetComponent<Rigidbody>();
        Started=true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Started){
            translation=new Vector3(Input.GetAxisRaw("Horizontal"),0f,Input.GetAxisRaw("Vertical")).normalized;

            switch(cameraMode){
                case CameraMode.FirstPerson:
                    FirstPersonUpdate();
                    break;
                case CameraMode.ThirdPerson:
                    ThirdPersonUpdate();
                    break;
            }

            if(Input.GetKeyDown(KeyCode.LeftShift)){
                isSprint=true;
            }
            if(Input.GetKeyUp(KeyCode.LeftShift)){
                isSprint=false;
            }

            if(Input.GetKeyDown(KeyCode.Space) & grounded){
                rb.AddForce(Vector3.up*JumpHeight, ForceMode.Impulse);
            }
            
            if(stat==PlayerStatus.Stop){
                animator.SetBool("Walking",false);
                animator.SetBool("Sprint",false);
            }else if(stat==PlayerStatus.Walking){
                animator.SetBool("Walking",true);
                animator.SetBool("Sprint",false);
            }else if(stat==PlayerStatus.Running){
                animator.SetBool("Walking",false);
                animator.SetBool("Sprint",true);
            }


            if(laststat==PlayerStatus.Running){
                if(stat==PlayerStatus.Walking){
                    audioSources[1].Stop();
                    audioSources[0].Play();
                }else if(stat==PlayerStatus.Stop){
                    audioSources[1].Stop();
                }
            }else if(laststat==PlayerStatus.Walking){
                if(stat==PlayerStatus.Running){
                    audioSources[0].Stop();
                    audioSources[1].Play();
                }else if(stat==PlayerStatus.Stop){
                    audioSources[0].Stop();
                }
            }else{
                if(stat==PlayerStatus.Running){
                    audioSources[1].Play();
                }else if(stat==PlayerStatus.Walking){
                    audioSources[0].Play();
                }
            }
            laststat=stat;

            if(Input.GetKeyDown(KeyCode.F5)){
                if(cameraMode==CameraMode.FirstPerson){
                    cameraMode=CameraMode.ThirdPerson;
                    thirdPersonCamera.SetActive(true);
                }else{
                    cameraMode=CameraMode.FirstPerson;
                    thirdPersonCamera.SetActive(false);
                }
            }
        }
    }

    void FirstPersonUpdate(){
        tsAngle=Vector3.SignedAngle(Vector3.forward,translation,Vector3.up);
        animator.SetFloat("tsAngle",tsAngle);

        if(!gameManager.UIopen){
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX*coefficient;
            yAngle+=mouseX;
        }
        transform.rotation=Quaternion.Euler(0f,yAngle,0f);
    }

    Quaternion gmd_rot;
    Vector3 thirdPersonMoveDir;
    void ThirdPersonUpdate(){
        animator.SetFloat("tsAngle",0);
        
        if(translation!=Vector3.zero){
            gmd_rot =  Quaternion.LookRotation(translation, Vector3.up);
            Vector3 cam_forward = thirdPersonCamera.transform.forward;
            thirdPersonMoveDir = (gmd_rot * (new Vector3 (cam_forward.x,0,cam_forward.z))).normalized;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(thirdPersonMoveDir, Vector3.up), 720f*Time.deltaTime);
            yAngle=transform.rotation.eulerAngles.y;
        }else{
            thirdPersonMoveDir = Vector3.zero;
        }
    }

    public enum PlayerStatus{
        Running,
        Walking,
        Stop
    }

    void FixedUpdate(){
        if(Started){
            rb.AddForce(Vector3.down*gravity, ForceMode.Acceleration);
            
            grounded = Physics.Raycast(transform.position, Vector3.down, transform.lossyScale.y + 0.2f, Ground);

            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            if (grounded & horizontalVelocity.sqrMagnitude > 0.1f)
                rb.AddForce(-horizontalVelocity.normalized * groundDrag, ForceMode.Acceleration);
            
            if(horizontalVelocity.sqrMagnitude>Mathf.Pow(MaxSpeed*(isSprint? Sprint: 1),2)){
                rb.linearVelocity=new Vector3(0, rb.linearVelocity.y, 0) + horizontalVelocity.normalized*MaxSpeed*(isSprint? Sprint: 1);
            }

            if(translation==Vector3.zero){
                stat=PlayerStatus.Stop;
            }else{
                if(isSprint){
                    stat=PlayerStatus.Running;
                }else{
                    stat=PlayerStatus.Walking;
                }
            }

            switch(cameraMode){
                case CameraMode.FirstPerson:
                    FirstPersonFixedUpdate();
                    break;
                case CameraMode.ThirdPerson:
                    ThirdPersonFixedUpdate();
                    break;
            }
        }
    }

    void FirstPersonFixedUpdate(){
        rb.AddForce(Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up)*translation*Acceleration, ForceMode.Acceleration);
//      rb.AddForce(Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up)*translation* Speed * 10f*(isSprint? Sprint:1f), ForceMode.Acceleration);
    }

    void ThirdPersonFixedUpdate(){
        rb.AddForce(thirdPersonMoveDir* Acceleration, ForceMode.Acceleration);
    }
}
