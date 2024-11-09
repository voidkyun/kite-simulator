using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class TMP_InputFieldJapaneseFilter : MonoBehaviour
{
    private TMP_InputField tmpInputField;

    void Start()
    {
        tmpInputField=GetComponent<TMP_InputField>();
        // TMP_InputFieldのonValueChangedイベントにメソッドを追加
        tmpInputField.onValueChanged.AddListener(ValidateInput);
    }

    void ValidateInput(string input)
    {
        // 半角英数字、全角英数字、平仮名、片仮名を許可する正規表現
        string validInput = Regex.Replace(input, @"[^a-zA-Z0-9ａ-ｚＡ-Ｚ０-９ぁ-んァ-ヶー]", "");

        // 入力内容を修正
        if (input != validInput)
        {
            tmpInputField.text = validInput;
        }
    }
}
