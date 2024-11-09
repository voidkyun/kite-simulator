using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCameraRotation : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 現在の回転角度を取得
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // y軸の回転を徐々に増加させる
        currentRotation.y += speed * Time.deltaTime;

        // 新しい回転角度を設定
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
