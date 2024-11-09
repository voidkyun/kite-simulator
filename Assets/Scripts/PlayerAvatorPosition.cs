using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerAvatorPosition : MonoBehaviour
{
    public float defaultHeight;
    private GameObject PlayerCapsule;
    private CapsuleCollider capsuleCollider;
    // Start is called before the first frame update
    void Start()
    {
        defaultHeight = Variables.Object(gameObject).Get<float>("Height");
        transform.localPosition=new Vector3(0f,-defaultHeight/2f,0f);
        transform.localRotation= Quaternion.identity;

        PlayerCapsule=transform.parent.gameObject;
        capsuleCollider=PlayerCapsule.GetComponent<CapsuleCollider>();
        capsuleCollider.height=defaultHeight;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
