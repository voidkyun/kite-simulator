using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed,Sprint,JumpHeight,groundDrag,airDrag,gravity;
    Rigidbody rb;
    public LayerMask Ground;
    public bool grounded,isSprint,isJumping;
    public float sensX,sensY;
    public float yAngle;
    public float coefficient;
    public GameObject gmObj;
    public GameManager gameManager;
    public AudioSource[] audioSources;
    public PlayerStatus stat,laststat;
    private Vector3 translation;
    private float tsAngle;
    public Animator animator;
    public Renderer m_renderer;

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
            tsAngle=Vector3.SignedAngle(Vector3.forward,translation,Vector3.up);
            animator.SetFloat("tsAngle",tsAngle);

            if(!gameManager.UIopen){
                float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX*coefficient;
                yAngle+=mouseX;
            }
            transform.rotation=Quaternion.Euler(0f,yAngle,0f);

            if(Input.GetKeyDown(KeyCode.LeftShift)){
                isSprint=true;
            }
            if(Input.GetKeyUp(KeyCode.LeftShift)){
                isSprint=false;
            }
            if(Input.GetKeyDown(KeyCode.Space)){
                isJumping=true;
            }
            if(Input.GetKeyUp(KeyCode.Space)){
                isJumping=false;
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
            
            //地面と接しているかを判断
            grounded = Physics.Raycast(transform.position, Vector3.down, transform.lossyScale.y + 0.2f, Ground);
            //接している場合は、設定した減速値を代入しプレイヤーを滑りにくくする
            if (grounded)
                rb.drag = groundDrag;
            else
                rb.drag = airDrag;

            if(translation==Vector3.zero){
                stat=PlayerStatus.Stop;
            }else{
                if(isSprint){
                    stat=PlayerStatus.Running;
                }else{
                    stat=PlayerStatus.Walking;
                }
            }
            if(grounded){
                rb.AddForce(Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up)*translation* Speed * 10f*(isSprint? Sprint:1f), ForceMode.Acceleration);
            }

            if(grounded & isJumping){
                rb.AddForce(Vector3.up*10f*JumpHeight,ForceMode.Impulse);
            }
        }
    }
}
