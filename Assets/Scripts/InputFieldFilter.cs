using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class InputFieldFilter : MonoBehaviour
{
    public TMP_InputField tmpInputField;

    void Start()
    {
        tmpInputField=GetComponent<TMP_InputField>();
        // TMP_InputFieldのonValueChangedイベントにメソッドを追加
        tmpInputField.onValueChanged.AddListener(ValidateInput);
    }

    void ValidateInput(string input)
    {
        // 数字とドットのみ許可する正規表現
        string validInput = Regex.Replace(input, @"[^0-9.]", "");

        // ドットが複数存在しないように制限（最初のドット以降のドットを削除）
        int dotIndex = validInput.IndexOf('.');
        if (dotIndex != -1)
        {
            validInput = validInput.Substring(0, dotIndex + 1) + validInput.Substring(dotIndex + 1).Replace(".", "");
        }

        // 入力内容を修正
        if (input != validInput)
        {
            tmpInputField.text = validInput;
        }
    }
}
