using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPayer : MonoBehaviour
{
    public GameObject Player;
    public GameManager gameManager;
    public float yOffset,zOffset;
    public Animator animator;
    private Transform transHead;
    public bool Started=false;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => gameManager.PlayerAvatarLoaded);
        animator=gameManager.PlayerAvatar.GetComponent<Animator>();
        transHead = animator.GetBoneTransform(HumanBodyBones.Head);
        Started=true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void LateUpdate(){
        if(Started){
            transform.position=transHead.position+transHead.up*yOffset+transHead.forward*zOffset;
        }
    }
}
