using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class UsingItem : MonoBehaviour
{
    public GameObject Handle,rope;
    public Rope ropescript;
    public GameManager gameManager;
    public float pickupRange = 5f;
    Transform KiteSet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            Vector3 pos=new Vector3(Screen.width/2f, Screen.height/2f,0f);
            Ray ray=Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;            
            // 無視するレイヤーのLayerMaskを作成
            int ignoreLayer = LayerMask.NameToLayer("Ghost");
            int layerMask = ~(1 << ignoreLayer); // ignoreLayerを無視するためにビット反転

            if(Physics.Raycast(ray, out hit, pickupRange,layerMask)){
                print(hit.collider.name);
                if(hit.collider.CompareTag("Handle")){
                    print("handle!");
                    Handle=hit.collider.gameObject;
                    Handle.transform.Find("Canvas").gameObject.SetActive(false);
                    KiteSet=Handle.transform.parent;
                    rope=KiteSet.Find("Rope").gameObject;
                    ropescript=rope.GetComponent<Rope>();
                    Rigidbody rb=Handle.GetComponent<Rigidbody>();
                    HandleController handleController=Handle.GetComponent<HandleController>();
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    handleController.isPicked = true;
                    Handle.transform.SetParent(transform);
                    float defaultHeight = Variables.Object(gameManager.PlayerAvatar).Get<float>("Height");
                    Handle.transform.localPosition = new Vector3(0.7f,0.83f,0.7f)*(defaultHeight/1.6f); // 保持位置の原点に配置
                    Handle.transform.localRotation = Quaternion.identity; // 回転をリセット
                }
            }else{
                print("no obj");
            }
        }

        if(Input.GetKeyDown(KeyCode.Q)){
            if(Handle!=null){
                Handle.transform.Find("Canvas").gameObject.SetActive(true);
                Handle.GetComponent<HandleController>().isPicked=false;
                Handle.transform.SetParent(KiteSet);
                Rigidbody rb=Handle.GetComponent<Rigidbody>();
                rb.isKinematic=false;
                rb.useGravity = true;
                rope=null;
                Handle=null;
            }
        }

        if(!gameManager.UIopen & Handle!=null){
            if(Input.GetMouseButton(0)){
                ropescript.Addvertices();
            }else if(Input.GetMouseButton(1)){
                ropescript.Removevertices();
            }
        }
    }
}
