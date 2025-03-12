using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.Threading.Tasks;
using System;
using System.IO;
using UniVRM10;




public class GameManager : MonoBehaviour
{
    public AnimationCurve gauss,initialwindforce;
    
    public StringTable table;

    [Range(0f,10f)]
    public float WindForce;

    public GameObject WindAudio1,WindAudio2,SettingUI,Kite,Player,DebugScreen,PlayerAvatar;
    public TMP_Dropdown LanguageMenue;
    public Slider MouseXSlider,MouseYSlider,AmbientVolumeSlider,PlayerVolumeSlider,CrosshairAlphaSlider;
    public TextMeshProUGUI shojun,debug;
    public RuntimeAnimatorController PlayerAnimatorController;
    public Avatar Humanoid;
    public PlayerMovement playerMovement;
    public Rope rope;
    public AudioSource windAudioSource1,windAudioSource2;
    public string[] locales=new string[]{"en","ja"};

    public bool UIopen=false;

    public bool PlayerAvatarLoaded=false;

    // Start is called before the first frame update

    bool started=false;
    async void Start()
    {
        await LoadPlayerAvatar();
        PlayerAvatarLoaded=true;

        Cursor.lockState = CursorLockMode.Locked;
        table = LocalizationSettings.StringDatabase.GetTable("Language");

        MouseXSlider.value = PlayerPrefs.GetFloat("mousex",1f);
        MouseYSlider.value = PlayerPrefs.GetFloat("mousey",1f);
        AmbientVolumeSlider.value = PlayerPrefs.GetFloat("ambientv",1f);
        PlayerVolumeSlider.value = PlayerPrefs.GetFloat("playerv",1f);
        CrosshairAlphaSlider.value = PlayerPrefs.GetFloat("crossa",1f);
        print(PlayerPrefs.GetString("locale"));
        LanguageMenue.value = Array.IndexOf(locales, PlayerPrefs.GetString("locale", "en"));

        transform.Rotate(0f,UnityEngine.Random.value*360f,0f);
        //WindForce=UnityEngine.Random.value*7f;
        WindForce=8f;
        StartCoroutine(Playwind2(26f));

        started=true;
    }

    private async Task LoadPlayerAvatar(){
        string AvatarName=PlayerPrefs.GetString("AvatarName","Default1");
        PlayerAvatar=await LoadAvatarModel(AvatarName);
        PlayerAvatar.layer=10;
        float defaultHeight = Variables.Object(PlayerAvatar).Get<float>("Height");
        PlayerAvatar.transform.localRotation = Quaternion.identity;
        PlayerAvatar.transform.localScale=new Vector3(1f,1f,1f);
        CapsuleCollider capsuleCollider=Player.GetComponent<CapsuleCollider>();
        capsuleCollider.height=defaultHeight;
        capsuleCollider.center = new Vector3(0, defaultHeight / 2f, 0);
    }

    private async Task<GameObject> LoadAvatarModel(string AvatarName)
    {
        // Resourcesフォルダ内の指定されたパスからすべてのプレハブを読み込む
        GameObject loadedPrefab = Resources.Load<GameObject>($"Avatars/{AvatarName}");
        if(loadedPrefab!=null){
            GameObject PlayerAvatar=Instantiate(loadedPrefab,Player.transform);
            PlayerAvatar.transform.localPosition = Vector3.zero;
            return PlayerAvatar;
        }else{
            string path=PlayerPrefs.GetString("AvatarDirectoryPath",UnityEngine.Application.persistentDataPath);
            if(Directory.Exists(path)){
                List<string> VrmFileNames=GetVrmFileNames(path);
                if(VrmFileNames.Contains(AvatarName)){
                    GameObject PlayerAvatar=await LoadVRM($"{path}\\{AvatarName}.vrm",AvatarName);
                    GameObject body=PlayerAvatar.transform.Find("Body").gameObject;
                    Bounds bounds=body.GetComponent<SkinnedMeshRenderer>().bounds;
                    float height=bounds.size.y;
                    PlayerAvatar.transform.SetParent(Player.transform);
                    PlayerAvatar.transform.localPosition = Vector3.zero;
                    Variables variablesComponent = PlayerAvatar.AddComponent<Variables>();
                    variablesComponent.declarations.Set("Height", height);
                    Animator animator=PlayerAvatar.GetComponent<Animator>();
                    animator.runtimeAnimatorController=PlayerAnimatorController;
                    //animator.avatar=Humanoid;
                    return PlayerAvatar;
                }else{
                    print("The VRM file does not exist.");
                    PlayerPrefs.SetString("AvatarName","Default1");
                    GameObject PlayerAvatar=Instantiate(Resources.Load<GameObject>("Avatars/Default1"),Player.transform); 
                    PlayerAvatar.transform.localPosition = Vector3.zero;
                    return PlayerAvatar;
                }
            }else{
                print("The Avatar Directory does not exist.");
                PlayerPrefs.SetString("AvatarDirectoryPath",UnityEngine.Application.persistentDataPath);
                PlayerPrefs.SetString("AvatarName","Default1");
                GameObject PlayerAvatar=Instantiate(Resources.Load<GameObject>("Avatars/Default1"),Player.transform);
                PlayerAvatar.transform.localPosition = Vector3.zero;
                return PlayerAvatar;
            }

        }
    }

    private async Task<GameObject> LoadVRM(string path,string AvatarName){
        GameObject vrm1;
        Vrm10Instance vrm10Instance = await Vrm10.LoadPathAsync(path);
        vrm1 = vrm10Instance.gameObject;
        vrm1.name=AvatarName;
        return vrm1;
    }

    List<string> GetVrmFileNames(string path)
    {
        List<string> fileNames = new List<string>();

        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.vrm", SearchOption.TopDirectoryOnly);

            foreach(string file in files){
                string t=Path.GetFileName(file);
                int index=t.Length-4;
                t=t[0..index];
                fileNames.Add(t);
            }
        }
        else
        {
            Debug.LogError($"指定したフォルダーは存在しません: {path}");
        }
        return fileNames;
    }

    // ロケーション変更時に呼び出すメソッド
    public void OnLocationChange()
    {
        StartCoroutine(LoadStringTable());
    }

    // StringTableを非同期に取得
    IEnumerator LoadStringTable()
    {
        var loadOperation = LocalizationSettings.StringDatabase.GetTableAsync("Language");
        yield return loadOperation;

        if (loadOperation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            table = loadOperation.Result;
            UpdateUIText(); // 取得後にUIのテキストを更新
        }
        else
        {
            Debug.LogError("StringTableの読み込みに失敗しました。");
        }
    }

    // UIのテキストを更新するメソッド
    void UpdateUIText()
    {
        if (table != null)
        {
            debug.text = $"{table.GetEntry("height").Value} : {Kite.transform.position.y - Player.transform.position.y + 1.75f / 2f:0.00}m\n" +
                         $"{table.GetEntry("windv").Value} : {WindForce:0.00}m/s\n" +
                         $"{table.GetEntry("rope").Value} : {rope.num * 0.3f}m";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!started)
            return;

        playerMovement.sensX=MouseXSlider.value;
        playerMovement.sensY=MouseYSlider.value;
        Color color=shojun.color;
        color.a=CrosshairAlphaSlider.value;
        shojun.color=color;
        UpdateUIText();
        AudioSource[] moveaudio=Player.GetComponents<AudioSource>();
        moveaudio[0].volume=PlayerVolumeSlider.value;
        moveaudio[1].volume=PlayerVolumeSlider.value;
        PlayerPrefs.SetFloat("mousex",MouseXSlider.value);
        PlayerPrefs.SetFloat("mousey",MouseYSlider.value);
        PlayerPrefs.SetFloat("ambientv",AmbientVolumeSlider.value);
        PlayerPrefs.SetFloat("playerv",PlayerVolumeSlider.value);
        PlayerPrefs.SetFloat("crossa",CrosshairAlphaSlider.value);


        transform.Rotate(0f,CurveWeightedRandom(gauss),0f);
        WindForce=Mathf.Clamp(WindForce+CurveWeightedRandom(gauss)*0.1f,0f,10f);

        if(Input.GetKeyDown(KeyCode.Escape)){
            if(UIopen){
                Cursor.lockState=CursorLockMode.Locked;
            }else{
                Cursor.lockState=CursorLockMode.Confined;
            }
            UIopen=!UIopen;
            SettingUI.SetActive(!SettingUI.activeSelf);
        }
        if(Input.GetKeyDown(KeyCode.F3)){
            DebugScreen.SetActive(!DebugScreen.activeSelf);
        }
    }

    float CurveWeightedRandom(AnimationCurve curve) {
        return curve.Evaluate(UnityEngine.Random.value);
    }

    IEnumerator Playwind2(float delay){
        windAudioSource1.Play();
        float time=0f;
        while(time<delay){
            float windForceNormalized = WindForce / 10f;
            float ambientVolume = AmbientVolumeSlider.value;
            time+=Time.deltaTime;
            windAudioSource1.volume=windForceNormalized*ambientVolume;
            yield return null;
        }
        //print("loop end");
        windAudioSource2.Play();
        StartCoroutine(CrossfadeAudio(windAudioSource1,windAudioSource2,51f));
    } 
    IEnumerator CrossfadeAudio(AudioSource audioSource1, AudioSource audioSource2, float fadeDuration)
    {
        //print("hi");
        while(true){
            float time = 0;
            while (time < fadeDuration)
            {
                float windForceNormalized = WindForce / 10f;
                float ambientVolume = AmbientVolumeSlider.value;
                //Debug.Log(windForceNormalized*ambientVolume);
                time += Time.deltaTime;
                audioSource1.volume = Mathf.Lerp(windForceNormalized*ambientVolume, 0, time / fadeDuration);
                audioSource2.volume = Mathf.Lerp(0, windForceNormalized*ambientVolume, time / fadeDuration);
                yield return null;
            }
        }
    }

    public void LoadTile(){
        SceneManager.LoadScene("Title");
    }

    
    public async void DropdownChange(int value)
    {
        await ChangeLocale(locales[value]);
    }

    private async Task ChangeLocale(string key)
    {
        // 指定したLocaleを取得
        var locale = LocalizationSettings.AvailableLocales.Locales.Find((x) => x.Identifier.Code == key);
        if (locale == null)
        {
            Debug.LogError($"Locale '{key}' not found.");
            return;
        }

        // Localeの設定
        LocalizationSettings.SelectedLocale = locale;


        // 初期化待ち
        await LocalizationSettings.InitializationOperation.Task;

        if (LocalizationSettings.InitializationOperation.IsDone)
        {
            Debug.Log($"Locale change is completed.\nkey:{key}\nlocale:{locale}");
            PlayerPrefs.SetString("locale",key);
            
            if(SceneManager.GetActiveScene().name=="fwOF_FreeDemo_OldForest"){
                OnLocationChange();
            }
        }
        else
        {
            Debug.LogError("Locale change failed.");
        }

    }
}
