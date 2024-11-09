using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectModelScript : MonoBehaviour
{
    public List<GameObject> prefabs=new List<GameObject>();
    public GameObject AvatarElement;
    public RectTransform contentRectTransform;
    public Dictionary<string,RenderTexture> renderTextures=new Dictionary<string,RenderTexture>();
    // Start is called before the first frame update
    void Start()
    {
        LoadPrefabs();
        List<GameObject> avatars=new List<GameObject>();
        int cnt=0;
        foreach (GameObject prefab in prefabs){
            GameObject instance=Instantiate(prefab);
            instance.transform.position=new Vector3(9999f*(float)cnt,0f,0f);
            string AvatarName=prefab.name;
            int index=AvatarName.Length-3;
            AvatarName=AvatarName[0..index];
            
            avatars.Add(Instantiate(AvatarElement,contentRectTransform));
            
            /* RectTransform rectTransform=avatars.Last().GetComponent<RectTransform>();
            float xpos,ypos;
            xpos=250f*(float)(cnt % Mathf.CeilToInt(Screen.width/250f))-Screen.width/2f+100f+25f;
            ypos=-300f*(float)(cnt / Mathf.CeilToInt(Screen.width/250f))+(Screen.height-124.7f)/2f-125f-35f;
            rectTransform.anchoredPosition=new Vector2(xpos,ypos);
             */
            
            GameObject buttonObj=avatars.Last().transform.Find("Button").gameObject;
            Button button=buttonObj.GetComponent<Button>();
            button.onClick.AddListener(delegate{SetAvatar(AvatarName);});

            Image image=buttonObj.GetComponent<Image>();
            RenderTexture texture=Resources.Load<RenderTexture>($"Models/{AvatarName}");
            renderTextures.Add(AvatarName,texture);
            image.material=new Material(Shader.Find("Custom/TransparentRenderTexture"));
            image.material.mainTexture = texture;

            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text=AvatarName;
            cnt++;
        }
        
    }
    void LoadPrefabs()
    {
        // Resourcesフォルダ内の指定されたパスからすべてのプレハブを読み込む
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Models/Sets");

        // 読み込んだプレハブをListに追加
        foreach (GameObject prefab in loadedPrefabs)
        {
            prefabs.Add(prefab);
        }

        Debug.Log("Loaded " + prefabs.Count + " prefabs.");
    }

    public void SetAvatar(string AvatarName){

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
