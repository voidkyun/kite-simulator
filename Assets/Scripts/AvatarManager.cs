using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UniVRM10;


public class AvatarManager : MonoBehaviour
{
    public TextMeshProUGUI NowAvatarNameText;
    public RawImage NowAvatarImage;
    public GameObject PreviewSet,PreviewCam,vrm1,AplyError,AplySucces;
    public TMP_InputField PathInputField;
    public RenderModels renderModels;
    //public float AddingAvatarHeight;
    //public UnityEngine.UI.Button AddButton;
    //public TMP_InputField NameInputField;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => renderModels.isStarted);

        PathInputField.text=PlayerPrefs.GetString("AvatarDirectoryPath",UnityEngine.Application.persistentDataPath);
        string AvatarName=PlayerPrefs.GetString("AvatarName","Default1");
        NowAvatarNameText.text=AvatarName;
        RenderTexture texture=renderModels.renderTextures[AvatarName];
        NowAvatarImage.texture=texture;
    }

    // Update is called once per frame
    void Update()
    {

        /*
        if(AddingAvatarHeight>0 & vrm1!=null & NameInputField.text!=""){
            AddButton.interactable=true;
        }else{
            AddButton.interactable=false;
        }
        */
    }

    public void SetAvatar(string AvatarName){
        PlayerPrefs.SetString("AvatarName", AvatarName);
        NowAvatarNameText.text=AvatarName;
        RenderTexture texture=renderModels.renderTextures[AvatarName];
        NowAvatarImage.texture=texture;
    }

    public void AplyAvatarDirectoryPath(){
        string path=PathInputField.text;
        if(Directory.Exists(path)){
            PlayerPrefs.SetString("AvatarDirectoryPath",path);
            PlayerPrefs.SetString("AvatarName", "Default1");
            AplyError.SetActive(false);
            StartCoroutine(ReLoadScene());
        }else{
            AplyError.SetActive(true);
        }
    }

    public void RestoreDirectoryPath(){
        PathInputField.text=UnityEngine.Application.persistentDataPath;
    }

    private IEnumerator ReLoadScene(){
        // シーンを非同期でロードする
        var async = SceneManager.LoadSceneAsync("Avatar");
        async.allowSceneActivation = false;

        string[] messages = { "Reloading .", "Reloading . .", "Reloading . . ." };
        int index = 0;
        AplySucces.SetActive(true);
        // ロードが完了するまで待機する
        while (async.progress < 0.9f) {
            AplySucces.GetComponent<TextMeshProUGUI>().text = messages[index];
            index = (index + 1) % messages.Length; // インデックスを更新
            yield return new WaitForSeconds(1f); // 1秒待つ
        }
        AplySucces.SetActive(false);
        async.allowSceneActivation = true;
    }

    public void LoadTitle(){
        SceneManager.LoadScene("Title");
    }

/*     public async void UploadVRM(){
        vrm1 = GameObject.Find("VRM1");
        if(vrm1 != null){
            Destroy(vrm1);
        }

        string path = LoadFile();
        if (path.Length != 0)
        {
            Vrm10Instance vrm10Instance = await Vrm10.LoadPathAsync(path);
            vrm1 = GameObject.Find("VRM1");
            
            vrm1.transform.SetParent(PreviewSet.transform);
            vrm1.transform.localPosition=Vector3.zero;

            GameObject body=vrm1.transform.Find("Body").gameObject;
            Bounds bounds=body.GetComponent<SkinnedMeshRenderer>().bounds;
            AddingAvatarHeight=bounds.size.y;
            PreviewCam.transform.localPosition=new Vector3(0f,0.76f*(AddingAvatarHeight/1.6f),1.57f*(AddingAvatarHeight/1.6f));
        }
    }
 */
/*     private string LoadFile(){
        var filePath = string.Empty;

        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "VRM files|*.vrm;*.VRM";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;
            }
        }
        return filePath;
    } */

/*     public void AddAvatar(){
        string Name=NameInputField.text;
        vrm1.name=Name;
        PreviewSet.name=Name+"set";
        
    } */
    
    private Transform transSelf;
    private Animator anime;
    private Transform transHead;
    private Transform transFoot;
    private float defaultHeight;
    public float MeasureVRMHeight(GameObject VRMModel){
        anime = VRMModel.GetComponent<Animator>();
        transSelf = VRMModel.GetComponent<Transform>();
        transHead = anime.GetBoneTransform(HumanBodyBones.Head);
        transFoot = anime.GetBoneTransform(HumanBodyBones.RightFoot);
        defaultHeight = transHead.position.y - transFoot.position.y;
        return(defaultHeight);
    }
}
