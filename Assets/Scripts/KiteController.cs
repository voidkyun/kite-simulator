using UnityEngine;

public class KiteController : MonoBehaviour
{
    Rigidbody rb;
    public float windMagnitude, coefficient, runco, upco;
    public GameObject GameManager, Handle, ropeObj;
    public Vector3 TensionalForce;
    Vector3 windDirection;
    GameManager gmScript;
    Rope rope;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gmScript = GameManager.GetComponent<GameManager>();
        rope = ropeObj.GetComponent<Rope>();
    }

    void Update()
    {
        windMagnitude = gmScript.WindForce * transform.position.y * coefficient;
        windDirection = GameManager.transform.forward;
        float distance = Mathf.Clamp((transform.position - Handle.transform.position).magnitude - (float)rope.num * 0.3f, 0f, Mathf.Infinity);
        TensionalForce = (Handle.transform.position - transform.position).normalized * distance * rope.tension;
    }

    void FixedUpdate()
    {
        Vector3 verocity = rb.linearVelocity;
        Vector3 windForce = windDirection * windMagnitude - verocity * runco;
        Vector3 liftForce = Vector3.up * windForce.magnitude * upco;
        rb.AddForce(TensionalForce, ForceMode.Force);
        rb.AddForce(windForce, ForceMode.Force);
        rb.AddForce(liftForce, ForceMode.Force);
    }
}
