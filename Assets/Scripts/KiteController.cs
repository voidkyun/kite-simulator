using UnityEngine;

public class KiteController : MonoBehaviour
{
    public Rigidbody rb;
    public float windForce,coefficient,runco,upco;
    public GameObject GameManager,Handle,ropeObj;
    public Vector3 TensionalForce;
    Vector3 windDirection;
    GameManager gmScript;
    Rope rope;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gmScript=GameManager.GetComponent<GameManager>();
        rope=ropeObj.GetComponent<Rope>();
    }

    void Update()
    {
        windForce=gmScript.WindForce*transform.position.y*coefficient;
        windDirection=GameManager.transform.forward*windForce;
        float distance=Mathf.Clamp((transform.position-Handle.transform.position).magnitude - (float)rope.num*0.3f,0f,Mathf.Infinity);
        TensionalForce=(Handle.transform.position-transform.position).normalized*distance*rope.tension;
    }

    void FixedUpdate(){
        Vector3 verocity=rb.linearVelocity;
        windDirection=windDirection-verocity*runco;
        Vector3 upperForce=Vector3.up*windDirection.magnitude*upco;
        rb.AddForce(TensionalForce,ForceMode.Force);
        rb.AddForce(windDirection,ForceMode.Force);
        rb.AddForce(upperForce,ForceMode.Force);
        
    }
}
