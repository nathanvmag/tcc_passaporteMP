using distriqt.plugins.share;

using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class AppController : MonoBehaviour
{
    public GameObject SplashScreen, MainScene, CollectionScene,showingTargetsBts,photoTakenBts,collectBt,saveImageBt,popupController,loginMenu,pickPersonagem,telaPersonagem,menuJogo,configScene,menuScene,mapScene,pontosScene,jogoBts,descontoPopup,descontoTitle,descontoContent,descontoPrefab;
    public Image pictureImage;
    public int currentTargetID = -1,lastTargetID;
    private string figurinhasContent;
    private JArray figurinhasContentJson;
    private List<MarkerData> markerDataList = new List<MarkerData>();
    private static int projectID = 3;
    private NetworkManager networkManager;
    private LoginManager loginManager;
    public TMPro.TMP_Text saldoText;
    public int myPontos;
    // Start is called before the first frame update
    public static AppController Instance { get; private set; }
    private void Awake()
    {

        popupController.SetActive(true);
        // Verifica se j� existe uma inst�ncia do GameManager
        if (Instance == null)
        {
            Debug.Log("Iniciando controller");
            Instance = this;
            DontDestroyOnLoad(gameObject); // Garante que o objeto n�o ser� destru�do ao trocar de cena
        }
        else
        {
            Destroy(gameObject); // Se j� existir uma inst�ncia, destr�i a duplicada
        }
    }
    void Start()
    {
       
        networkManager = FindFirstObjectByType<NetworkManager>();
        loginManager = FindAnyObjectByType<LoginManager>(FindObjectsInactive.Include);
        getColletionData();
        currentTargetID = -1;
        SplashScreen.SetActive(true);
        MainScene.SetActive(false);
        CollectionScene.SetActive(false);
        menuScene.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.M))
        {
            menuJogo.SetActive(!menuJogo.activeSelf);
        }
    }

    async void getColletionData()
    {
        var response =await networkManager.CallWebMethodAsync("/api/projects/" + projectID,
            "9fd3f71917a2aa3cabe0384cc654eba529b8d17a3973539f7152235ef5e743da84be88f6e47ff2e7b099bf4d72e78b7c29d5e7d057d0216dbe85993fd01fcf7a17e02be474aa8ab68331e8eac324f3a1bdb832b13bdf12c41cbb0f05b6b9dd5c3b1d68cfbb2ae965b3f3f84ed9d558697b4923d6e7eeb2571cd1d79995f9d1e4"
            , "GET", false, null);
       
        if(response.Success)
        {
            figurinhasContent= response.Body;
            figurinhasContentJson = (JArray) ((JArray) response.responseJson["data"]["attributes"]["seasons"])[0]["collections"] ;
            var AllTargets = FindObjectsByType<TargetController>(FindObjectsInactive.Include,FindObjectsSortMode.InstanceID);
            
            markerDataList.Clear();
            
            foreach( JObject artefato in figurinhasContentJson)
            {
                MarkerData markerData = new MarkerData();
                
                markerData.localizacao = artefato.GetValue("name")?.Value<string>() ?? "";
                markerData.data = artefato.GetValue("description")?.Value<string>() ?? "";
                
                var artifactsData = artefato["artifacts"]?["data"] as JArray;
                if (artifactsData != null && artifactsData.Count > 0)
                {
                    var firstArtifact = artifactsData[0];
                    var attributes = firstArtifact["attributes"];
                    
                    markerData.pontos = attributes?["points"]?.Value<int>() ?? 0;
                    
                    var descriptionArray = attributes?["description"] as JArray;
                    if (descriptionArray != null && descriptionArray.Count > 0)
                    {
                        var firstDesc = descriptionArray[0];
                        var children = firstDesc["children"] as JArray;
                        if (children != null && children.Count > 0)
                        {
                            markerData.texto = children[0]["text"]?.Value<string>() ?? "";
                        }
                    }
                    
                    var location = attributes?["location"];
                    if (location != null)
                    {
                        markerData.latitude = location["lat"]?.Value<double>() ?? 0.0;
                        markerData.longitude = location["lng"]?.Value<double>() ?? 0.0;
                    }

                    var imagesData = attributes?["images"]?["data"] as JArray;
                    if (imagesData != null && imagesData.Count > 0)
                    {
                        var firstImage = imagesData[0];
                        var imageAttributes = firstImage["attributes"];
                        var imageUrl = imageAttributes?["url"]?.Value<string>();

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            markerData.imagemPath = imageUrl;
                        }
                        else
                        {
                            markerData.imagemPath = "";
                        }
                    }
                    else
                    {
                        markerData.imagemPath = "";
                    }
                }
                
                if (markerData.latitude != 0.0 && markerData.longitude != 0.0)
                {
                    markerDataList.Add(markerData);
                }

                foreach ( TargetController target in AllTargets)
                {
                    if (artefato.GetValue("name").Value<string>() == target.targetText)
                    {
                        target.targetID = ((JArray)artefato["artifacts"]["data"])[0]["id"].Value<int>();
                        try
                        {
                            if (((JArray)artefato["artifacts"]["data"])[0]["attributes"]["description"].Values<JArray>() != null)
                            {
                                var linkarray = ((JArray)((JArray)artefato["artifacts"]["data"])[0]["attributes"]["description"])[0]["children"];
                                target.targetLink = linkarray[0]["text"].Value<string>();
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        // target.targetLink

                    }

                }
               
            }
            var markerlist= new MarkerDataList();
            markerlist.markers = markerDataList;

            mapScene.GetComponentInChildren<markerManager>().LoadMarkers( markerlist);

        }
    }
    
   
    public void SeenTarget(int targetName)
    {
        currentTargetID = targetName;
        lastTargetID= targetName;
        showingTargetsBts.SetActive(true);
        if(targetName==-1)
        {
            pickPersonagem.SetActive(false);
        }
        else
        {
            
            pickPersonagem.SetActive(!Utils.checkCollectedPersonagem(targetName));

        }
    }
    public void UnSeenTarget(int targetName)
    {
        currentTargetID = -1;
        showingTargetsBts.SetActive(false);
    }
    public JArray getCollectionData()
    {
        return figurinhasContentJson;
    }
    public int getProjectID()
    {
        return projectID;
    }
    public void TakePhoto()
    {
        StartCoroutine(PictureEnum());
    }
    public void SairBtn()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        CollectionScene.SetActive(false);
        configScene.SetActive(false);
        var fig = FindObjectsByType<figurinhaManager>( FindObjectsInactive.Include,FindObjectsSortMode.InstanceID);
        foreach (figurinhaManager f in fig)
        {
            f.finded = false;
            f.setCardImage();
        }
    }
    public async void deleteAccount()
    {
        int myid = PlayerPrefs.GetInt("myid", 0);
        var response = await networkManager.CallWebMethodAsync("/api/users/" + myid,
             Utils.getAutentication()
            , "DELETE", false, null) ;
        print(response.responseJson);
        SairBtn();
    }
    public void backPhoto()
    {
        photoTakenBts.SetActive(false);
        pictureImage.sprite = null;
        showingTargetsBts.SetActive(currentTargetID != -1);
    }
    public void SharePhoto()
    {
        print("Tentou share " + Share.isSupported);
       
        if (Share.isSupported)
        {
            if (lastTargetID != -1)
            {
                doShareMethod();

            }
            Texture2D image = pictureImage.sprite.texture;
            Share.Instance.share(
            "Foto em Miguel Pereira!!",
            image
            );
        }
    }
    public async void doShareMethod()
    {
        if (await Utils.checkAuth(networkManager))
        {
            NetworkResponse response = null;
            JObject artifact = new JObject();
            artifact.Add("artifact", lastTargetID);
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("data", artifact);
            
                        

            response = await networkManager.CallWebMethodAsync("/api/shares", Utils.getAutentication(), "POST", false, keyValuePairs.ToString());
            

        }

    }
    public void SaveImage()
    {
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(pictureImage.sprite.texture, "MuseuDarcy", "Image"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".png", (success, path) => {
            Debug.Log("Media save result: " + success + " " + path)
            ;saveImageBt.SetActive(false);
        });

    }
    IEnumerator PictureEnum()
    {
        collectBt.SetActive(false);
        showingTargetsBts.SetActive(false);
        jogoBts.SetActive(false);
        yield return new WaitForEndOfFrame();
        var printPicture = Utils.TakeScreenshotAndReturnSprite();
        pictureImage.sprite= printPicture;
        yield return new WaitForEndOfFrame();
        jogoBts.SetActive(true);
        photoTakenBts.SetActive(true);
        collectBt.SetActive(true) ;
        saveImageBt.SetActive(true);


    }
    
    public void OpenURLBt(string url)
    {
        Application.OpenURL(url);

    }
    
    public async  void  showPremiosMenu()
    {
        var objList = AppController.Instance.getCollectionData();
        if (objList == null || objList.Count == 0)
        {
            PopupManager.Instance.showPopUp("Não é possível acessar os locais colecionados sem conexão com a internet!");
            gameObject.SetActive(false);
            return;
        }
        if (!await Utils.checkAuth(networkManager))
        {
            loginMenu.SetActive(true);
            loginManager.setNextScene(1);
        }
        else
        {
            pontosScene.SetActive(true);
            saldoText.text = myPontos.ToString();
            
            // Limpar descontos anteriores
            foreach (Transform child in descontoContent.transform)
            {
                Destroy(child.gameObject);
            }
            
            var response = await networkManager.CallWebMethodAsync("/api/rewards/project/" + projectID,
           "9fd3f71917a2aa3cabe0384cc654eba529b8d17a3973539f7152235ef5e743da84be88f6e47ff2e7b099bf4d72e78b7c29d5e7d057d0216dbe85993fd01fcf7a17e02be474aa8ab68331e8eac324f3a1bdb832b13bdf12c41cbb0f05b6b9dd5c3b1d68cfbb2ae965b3f3f84ed9d558697b4923d6e7eeb2571cd1d79995f9d1e4"
           , "GET", false, null);
            
            
            if (response.Success)
            {
                
                JArray descontosArray = response.responseArrayJson;
                    
                foreach (JObject desconto in descontosArray)
                {
                    // Verificar se o desconto é válido
                    bool isValid = desconto.GetValue("valid")?.Value<bool>() ?? false;
                        
                    if (isValid)
                    {
                        // Instanciar o prefab do desconto
                        GameObject descontoInstance = Instantiate(descontoPrefab, descontoContent.transform);
                            
                        // Obter o componente DescontoManager
                        DescontoManager descontoManager = descontoInstance.GetComponent<DescontoManager>();
                            
                        if (descontoManager != null)
                        {
                            // Extrair dados do JSON
                            string title = desconto.GetValue("title")?.Value<string>() ?? "";
                            string description = desconto.GetValue("description")?.Value<string>() ?? "";
                            int price = desconto.GetValue("price")?.Value<int>() ?? 0;
                            float discount = desconto.GetValue("discouint")?.Value<float>() ?? 0f; // Note: "discouint" parece ser um typo na API

                            descontoManager.tituloDesc = descontoTitle.GetComponent<Text>();
                            descontoManager.descPopup = descontoPopup;
                            // Chamar o método para montar o desconto
                            descontoManager.montDiscount(title, discount.ToString() + "%", price, ((description.Length > 0 ? description : "Sem descrição")));
                                
                            Debug.Log($"Desconto criado: {title} - R$ {price} - {discount}%");
                        }
                        else
                        {
                            Debug.LogError("DescontoManager não encontrado no prefab!");
                            Destroy(descontoInstance);
                        }
                    }
                }
                
            }
            else
            {
                Debug.LogError("Falha ao buscar descontos: " + response.Body);
                PopupManager.Instance.showPopUp("Erro ao carregar descontos. Verifique sua conexão.");
            }
        }
    }
    public async void showCollectionMenu()
    {
        var objList = AppController.Instance.getCollectionData();
        if (objList == null || objList.Count == 0)
        {
            PopupManager.Instance.showPopUp("Não é possível acessar os prêmios sem conexão com a internet!");
            gameObject.SetActive(false);
            return;
        }
        if (!await Utils.checkAuth(networkManager))
        {
            loginMenu.SetActive(true);
            loginManager.setNextScene(1);
        }
        else
            CollectionScene.SetActive(true);
    }
    public async void collectItem()
    {
        if (currentTargetID == -1)
            return;

        var objList = AppController.Instance.getCollectionData();
        if (objList == null || objList.Count == 0)
        {
            PopupManager.Instance.showPopUp("Não é possível coletar locais sem conexão com a internet!");
            gameObject.SetActive(false);
            return;
        }

        if (!await Utils.checkAuth(networkManager))
        {
            loginMenu.SetActive(true);
            loginManager.setNextScene(0);
        }
        else
        {
            pickPersonagem.SetActive(false);

            NetworkResponse response = null;
            JObject artifact = new JObject();
            artifact.Add("artifact", lastTargetID);
            JObject keyValuePairs = new JObject();
            keyValuePairs.Add("data", artifact);
            keyValuePairs.Add("notes", "collected via app");



            response = await networkManager.CallWebMethodAsync("/api/collectibles", Utils.getAutentication(), "POST", false, keyValuePairs.ToString());

            if (response.Success)
            {
                Utils.saveCollectPersonagem(currentTargetID);
                PopupManager.Instance.showPopUp("Local coletado com sucesso!!");

            }
            else
            {
                PopupManager.Instance.showPopUp("Falha ao coletar locais por favor tente novamente!");
            }
        }
            
    }
    public void setPersonagemInfo(string name,string link,Sprite img)
    {
        telaPersonagem.GetComponent<telaPersonagemManager>().setPersonagem(name,link,img);
        telaPersonagem.SetActive(true);

    }
}
