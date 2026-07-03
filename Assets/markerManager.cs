using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

[System.Serializable]
public class MarkerData
{
    public string data;
    public string localizacao;
    public string texto;
    public double latitude;
    public double longitude;
    public string imagemPath;
    public int pontos;
}

[System.Serializable]
public class MarkerDataList
{
    public List<MarkerData> markers;
}

public class markerManager : MonoBehaviour
{
    public Texture2D markerTexture;   // Assign your marker texture in Inspector
    public GameObject info, originalBack;
    public Text titulo, desc,ptsTx;
    public Image Bg;

    private void Start()
    {
    }
  
    public void LoadMarkers(MarkerDataList markerList)
    {
       

        // Clear previous markers
        OnlineMapsMarkerManager.RemoveAllItems();

        // Create markers
        foreach (MarkerData data in markerList.markers)
        {
            OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(
                data.longitude,
                data.latitude,
                markerTexture,
                data.localizacao
            );

            marker.scale = 0.22f;
            marker.enabled = true;

            // Attach OnClick
            marker.OnClick += delegate { OnMarkerClick(data); };
        }
    }

    private void OnMarkerClick(MarkerData data)
    {
        ShowInfos(data);
    }

    public void ShowInfos(MarkerData data)
    {
        originalBack.SetActive(false);
        info.SetActive(true);
        titulo.text = data.localizacao + ", " + data.data;
        desc.text = data.texto;
        ptsTx.text = data.pontos.ToString() ;
        if (!string.IsNullOrEmpty(data.imagemPath))
        {
            StartCoroutine(LoadImageFromURL(data.imagemPath));
        }
        else
        {
            // Se não houver imagem, limpar o background ou usar uma imagem padrão
            Bg.sprite = null;
            Debug.Log("Nenhuma imagem disponível para este marcador");
        }
    }
    
    private IEnumerator LoadImageFromURL(string imageUrl)
    {
        string fullUrl = imageUrl;
        if (!imageUrl.StartsWith("http"))
        {
          
            fullUrl = NetworkManager.BaseUrl + imageUrl;
        }
        
        
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(fullUrl))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                
                if (Bg != null)
                {
                    Bg.sprite = sprite;
                }
            }
            else
            {
                Debug.LogError($"Erro ao carregar imagem: {webRequest.error}");
                Debug.LogError($"URL: {fullUrl}");
                
                // Opcional: definir uma imagem padrão em caso de erro
                Bg.sprite = null;
            }
        }
    }
}
