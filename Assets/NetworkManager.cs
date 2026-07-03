using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Compression;

public class NetworkResponse
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public string Body { get; set; }
    public bool ExceptionOccurred { get; set; }
    public JObject responseJson;
    public JArray responseArrayJson;
}

public class NetworkManager : MonoBehaviour
{
    // Static base URL
    public static string BaseUrl = "https://tcc.digitalnvm.com";
    public GameObject loadingObject;
    public PopupManager popupManager;
    public async Task<NetworkResponse> CallWebMethodAsync(string path,string bearerToken, string method, bool showLoading, string jsonData = null )
    {
        NetworkResponse response = new NetworkResponse();
        UnityWebRequest webRequest;

        // Combine base URL and path
        string fullUrl = $"{BaseUrl}{path}";

        if (method.ToUpper() == "POST"|| method.ToUpper() == "DELETE")
        {
            webRequest = new UnityWebRequest(fullUrl, method.ToUpper() == "POST"? UnityWebRequest.kHttpVerbPOST:UnityWebRequest.kHttpVerbDELETE);
            if (jsonData != null)
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Accept", "application/json; charset=utf-8");


            }
        }
        else // Defaults to GET if the method is not POST
        {
            webRequest = UnityWebRequest.Get(fullUrl);
        }

        webRequest.downloadHandler = new DownloadHandlerBuffer();
        // Add Bearer token if provided
        if (!string.IsNullOrEmpty(bearerToken))
        {
            webRequest.SetRequestHeader("Authorization", $"Bearer {bearerToken}");
        }
        loadingObject.SetActive(showLoading); 
        try
        {
            await webRequest.SendWebRequest();


            bool isAndroid = Application.platform == RuntimePlatform.Android || Application.platform==RuntimePlatform.OSXEditor;


            byte[] responseData = webRequest.downloadHandler.data;

            // Attempt decoding using UTF-8
            string responseBody = System.Text.Encoding.UTF8.GetString(responseData);
            print(responseBody);
            // Check if it contains encoding issues (garbled characters like "Ã£")
            if (responseBody.Contains("?") || responseBody.Contains("Ã") || responseBody.Contains("�"))
            {
                // Fallback to re-interpret the bytes as if they were incorrectly encoded in ISO-8859-1
                responseBody = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(responseData);
            }
            if (HasJsonErrorFlag(responseBody, out string errorMessage))
            {
                response.Success = false;
                response.Error = errorMessage;
                response.Body = responseBody;
                if(showLoading)
                popupManager.showPopUp(errorMessage);
                print("deu erro ");
                print(responseBody);
            }
            else
            {
                try
                {
                    response.Success = true;
                    response.Body = responseBody;
                    response.responseJson = JObject.Parse(responseBody);
                }
                catch
                {
                    response.Success = true;
                    response.Body = responseBody;
                    response.responseArrayJson = JArray.Parse(responseBody);
                }
            }
            
            /*else
            {
                response.Success = false;
                response.Error = $"Erro : {webRequest.responseCode}";
                popupManager.showPopUp(response.Error);
            }*/
        }
        catch (System.Exception ex)
        {
            print(ex);
            response.Success = false;
            response.ExceptionOccurred = true;
            response.Error = "Erro Desconhecido, por favor tente novamente!";
            if (showLoading)
                popupManager.showPopUp("Falha de conexão com servidor, por favor verifique sua conexão com a internet!");
            print(response.Error);
        }
        if (showLoading)
            loadingObject.SetActive(false);
        return response;
    }

    private bool HasJsonErrorFlag(string json, out string errorMessage)
    {
        errorMessage = null;

        try
        {
            var jsonObject = JObject.Parse(json);
            if (jsonObject["error"]!=null)
            {
                errorMessage = JObject.Parse( jsonObject["error"].ToString()).Value<string>("message");
                return true;
            }
        }
        catch
        {
            errorMessage = "Erro no servidor!.";
        }

        return false;
    }
    public string CorrigirEncoding(string entrada)
    {
        // Obtem os bytes da string incorreta usando a codificação atual (ISO-8859-1 ou Windows-1252)
        byte[] bytes = System.Text.Encoding.Default.GetBytes(entrada);

        // Converte os bytes para uma string em UTF-8 (codificação correta para acentos)
        string corrigida = System.Text.Encoding.UTF8.GetString(bytes);

        return corrigida;
    }
}
