using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using DailyYell.Unity.ValueObjects;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAIChatCompletionService
{
    private const string OPENAI_CHAT_COMPLETION_URL = "https://api.openai.com/v1/chat/completions";
    private readonly Dictionary<string, string> _requestHeaders;
    private readonly string _gptModel;
    
    public OpenAIChatCompletionService(string apiKey, string gptModel="gpt-3.5-turbo-16k-0613")
    {
        _requestHeaders = new Dictionary<string, string>
        {
            {"Authorization", "Bearer " + apiKey},
            {"Content-type", "application/json"},
            {"X-Slack-No-Retry", "1"}
        };
        _gptModel = gptModel;
    }
    
    public async UniTask<List<ChatMessage>> ChatCompletion(List<ChatMessage> messages)
    {
        var options = new OpenAIChatCompletionRequestOption
        {
            model = _gptModel,
            messages = messages
        };
        var jsonOptions = JsonUtility.ToJson(options);
        Debug.Log(jsonOptions);
        using var request = new UnityWebRequest(OPENAI_CHAT_COMPLETION_URL, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        foreach (var requestHeader in _requestHeaders)
        {
            request.SetRequestHeader(requestHeader.Key, requestHeader.Value);
        }

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            throw new Exception();
        }
        var responseString = request.downloadHandler.text;
        var responseObject = JsonUtility.FromJson<ChatGPTResponseModel>(responseString);
        Debug.Log("ChatGPT:" + responseObject.choices[0].message.content);
        messages.Add(responseObject.choices[0].message);
        return messages;
    }
}

[Serializable]
public class OpenAIChatCompletionRequestOption
{
    public string model;
    public List<ChatMessage> messages;
}

public class ChatGPTResponseModel
{
    public string id;
    public string @object;
    public int created;
    public Choice[] choices;
    public Usage usage;

    [System.Serializable]
    public class Choice
    {
        public int index;
        public ChatMessage message;
        public string finish_reason;
    }

    [System.Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}
