using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DailyYell.Unity.ValueObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SandboxController : MonoBehaviour
{
    private const string SYSTEM_PROMPT = "あなたは女子高生です。ユーザーが書いた日記の内容を読んで、ユーザーに共感して、100文字以内でコメントしてください。敬語は使わず親しみやすい表現を使ってください。アドバイスなどは不要です。";
    
    [SerializeField]
    private string _openAiApiKey;
    [SerializeField]
    private TMP_InputField _inputField;
    [SerializeField]
    private Button _submitButton;
    [SerializeField]
    private TMP_Text _outputText;

    private OpenAIChatCompletionService _service;
    
    private async UniTask Start()
    {
        _service = new OpenAIChatCompletionService(_openAiApiKey, "gpt-4-0613");
        await CreateConversationAsync();
    }

    private async UniTask CreateConversationAsync()
    {
        // ユーザーからのSaveボタン入力を待機
        await _submitButton.OnClickAsync();
        
        // 読み込み中表示
        _outputText.text = "日記を読んでいるよ";
        
        // インプットフィールドからユーザーの日記テキストを取得
        var userComment = _inputField.text;
        
        // OpenAIのChatCompletionAPIを呼び出し
        var messages = await _service.ChatCompletion(new List<ChatMessage>
        {
            new() { role = "system", content = SYSTEM_PROMPT },
            new() { role = "user", content = userComment }
        });
        
        // AIの回答をログに出力
        var outputMessage = messages[2].content;
        Debug.Log(outputMessage);
        _outputText.text = outputMessage;
        
        // 再び待機状態に戻る
        await CreateConversationAsync();
    }
}
