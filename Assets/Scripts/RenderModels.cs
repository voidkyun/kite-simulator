using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniVRM10;

public class RenderModels : MonoBehaviour
{
    public List<GameObject> prefabs=new List<GameObject>();
    public GameObject AvatarElement;
    public RectTransform contentRectTransform;
    public AvatarManager avatarManager;
    public Dictionary<string,RenderTexture> renderTextures=new Dictionary<string,RenderTexture>();
    public bool isStarted=false;
    // Start is called before the first frame update
    async void Start()
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
            button.onClick.AddListener(delegate{avatarManager.SetAvatar(AvatarName);});

            Image image=buttonObj.GetComponent<Image>();
            RenderTexture texture=Resources.Load<RenderTexture>($"Models/{AvatarName}");
            renderTextures.Add(AvatarName,texture);
            image.material=new Material(Shader.Find("Custom/TransparentRenderTexture"));
            image.material.mainTexture = texture;

            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text=AvatarName;
            cnt++;
        }

        string path=PlayerPrefs.GetString("AvatarDirectoryPath",UnityEngine.Application.persistentDataPath);
        List<string> VrmFiles=GetVrmFileNames(path);

        foreach (string VrmFile in VrmFiles){
            try{            
                print(""+VrmFile);
                GameObject VrModel=await LoadVRM(VrmFile);
                GameObject Set=new GameObject(VrModel.name+"set");
                Set.transform.position=new Vector3(9999f*(float)cnt,0f,0f);
                VrModel.transform.SetParent(Set.transform);
                VrModel.transform.localPosition=Vector3.zero;
                
                GameObject body=VrModel.transform.Find("Body").gameObject;
                Bounds bounds=body.GetComponent<SkinnedMeshRenderer>().bounds;
                float height=bounds.size.y;
                GameObject CameraObj=new GameObject(VrModel.name+"Camera");
                Camera camera=CameraObj.AddComponent<Camera>();
                CameraObj.transform.SetParent(Set.transform);
                CameraObj.transform.localRotation=Quaternion.Euler(0f,180f,0f);
                CameraObj.transform.localPosition=new Vector3(0f,0.76f*(height/1.6f),1.57f*(height/1.6f));
                
                renderTextures.Add(VrModel.name,new RenderTexture(1024, 1024, 24));
                camera.targetTexture = renderTextures[VrModel.name];
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0, 0, 0, 0);

                avatars.Add(Instantiate(AvatarElement,contentRectTransform));

                GameObject buttonObj=avatars.Last().transform.Find("Button").gameObject;
                Button button=buttonObj.GetComponent<Button>();
                button.onClick.AddListener(delegate{avatarManager.SetAvatar(VrModel.name);});
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text=VrModel.name;

                Image image=buttonObj.GetComponent<Image>();
                image.material=new Material(Shader.Find("Custom/TransparentRenderTexture"));
                image.material.mainTexture = renderTextures[VrModel.name];
                cnt++;
            }catch(System.Exception){
            }
        }
        isStarted=true;
    }
    private async Task<GameObject> LoadVRM(string path){
        GameObject vrm1;
        Vrm10Instance vrm10Instance = await Vrm10.LoadPathAsync(path);
        vrm1 = GameObject.Find("VRM1");
        string name=Path.GetFileName(path);
        int index=name.Length-4;
        name=name[0..index];
        vrm1.name=name;
        return vrm1;
    }

    // Update is called once per frame
    void Update()
    {
        
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

    List<string> GetVrmFileNames(string path)
    {
        List<string> fileNames = new List<string>();

        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.vrm", SearchOption.TopDirectoryOnly);

            fileNames.AddRange(files);
        }
        else
        {
            Debug.LogError($"指定したフォルダーは存在しません: {path}");
        }
        return fileNames;
    }
}
