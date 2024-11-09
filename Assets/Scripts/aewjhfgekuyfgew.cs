using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class aewjhfgekuyfgew : MonoBehaviour
{
    public StringTable table;
    public TMP_Dropdown LanguageMenue;
    public string[] locales=new string[]{"en","ja"};
    // Start is called before the first frame update
    async void Start()
    {
        table = LocalizationSettings.StringDatabase.GetTable("Language");
        Cursor.visible = true;
        await ChangeLocale(PlayerPrefs.GetString("locale","en"));
        LanguageMenue.value = Array.IndexOf(locales, PlayerPrefs.GetString("locale", "en"));
    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
        else
        {
            Debug.LogError("Locale change failed.");
        }

    }

    public void StartHost()
    {
        //ホスト開始
        NetworkManager.Singleton.StartHost();
        //シーンを切り替え
        NetworkManager.Singleton.SceneManager.LoadScene("MultiPlay", LoadSceneMode.Single);
    }

    public void StartClient()
    {
        //ホストに接続
        NetworkManager.Singleton.StartClient();
    }
}
