using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {

    public List<GameObject> vertices = new List<GameObject>();
    public GameObject Prefab,Kite,Handle;
    public int num;
    public float tension;
    LineRenderer line;
    HingeJoint m_hingeJoint;
    Rigidbody rb;

    void Start ()
    {	
        num=(int)((Handle.transform.position-Kite.transform.position).magnitude /0.3f);
        Vector3 direction=(Handle.transform.position-Kite.transform.position).normalized;
        
        for (int i=0; i<num; i++){
            vertices.Add(Instantiate (Prefab, Kite.transform.position+direction*(float)(i+1)*0.3f, Quaternion.identity));
            
//            GameObject childObject = new GameObject("SphereCollider");
//            childObject.transform.parent = vertices[i].transform;
//            childObject.transform.localPosition = Vector3.zero;
//            childObject.AddComponent<SphereCollider>();

            rb=vertices[i].GetComponent<Rigidbody>();
            rb.mass=0.001f;
            
            if (i>0){
                m_hingeJoint=vertices[i].GetComponent<HingeJoint>();
                rb=vertices[i-1].GetComponent<Rigidbody>();
                m_hingeJoint.connectedBody=rb;
            }
        }

//        rb=vertices[num-1].GetComponent<Rigidbody>();
//        rb.useGravity=false;
//        rb.isKinematic=true;
        m_hingeJoint=Handle.GetComponent<HingeJoint>();
        rb=vertices[num-1].GetComponent<Rigidbody>();
        m_hingeJoint.connectedBody=rb;

        m_hingeJoint=vertices[0].GetComponent<HingeJoint>();
        rb=Kite.GetComponent<Rigidbody>();
        m_hingeJoint.connectedBody =rb;

        line = GetComponent<LineRenderer>();

        line.positionCount = num;

        foreach (GameObject v in vertices)
        {            
            v.GetComponent<MeshRenderer> ().enabled = false;
        }
    }
	
    void Update () 
    {
        int idx = 0;
        foreach (GameObject v in vertices)
        {
            line.SetPosition(idx, v.transform.position);
            idx++;
        }
    }

    public void Addvertices(){
        vertices[num-1].transform.position = Handle.transform.position+Vector3.up*0.6f;
        num++;
        vertices.Add(Instantiate (Prefab, Handle.transform.position+Vector3.up*0.3f, Quaternion.identity));
        line.positionCount = num;
        vertices[num-1].GetComponent<MeshRenderer> ().enabled = false;
        vertices[num-1].GetComponent<Rigidbody>().mass=0.001f;
        
        m_hingeJoint=vertices[num-1].GetComponent<HingeJoint>();
        rb=vertices[num-2].GetComponent<Rigidbody>();
        m_hingeJoint.connectedBody=rb;

        m_hingeJoint=Handle.GetComponent<HingeJoint>();
        rb=vertices[num-1].GetComponent<Rigidbody>();
        m_hingeJoint.connectedBody=rb;
    }

    public void Removevertices(){
        if (num>5){
            vertices[num-2].transform.position=Handle.transform.position;
            m_hingeJoint=Handle.GetComponent<HingeJoint>();
            rb=vertices[num-2].GetComponent<Rigidbody>();
            m_hingeJoint.connectedBody=rb;

            line.positionCount--;
            GameObject removev=vertices[num-1];
            vertices.RemoveAt(num-1);
            Destroy(removev);
            num--;
        }
        
    }
}