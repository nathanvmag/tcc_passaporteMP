using Newtonsoft.Json.Linq;
using UnityEngine;

public class TelaColecaoManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    string lastCollectedJson = "";
    JArray objList;
    bool initilizedTargets;
    private NetworkManager networkManager;

    private void Awake()
    {
        initilizedTargets = false;
        networkManager = FindFirstObjectByType<NetworkManager>();
        
    }
    private async void OnEnable()
    {
        

        updateCards();

    }
    public async void updateCards()
    {
        objList = AppController.Instance.getCollectionData();
        if (objList == null || objList.Count == 0)
        {
            PopupManager.Instance.showPopUp("Não é possível acessar os locais colecionados sem conexão com a internet!");
            gameObject.SetActive(false);
            return;
        }
        var Allfigurinhas = FindObjectsByType<figurinhaManager>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

        if (!initilizedTargets)
        {
            foreach (JObject artefato in objList)
            {
                foreach (figurinhaManager target in Allfigurinhas)
                {
                    if (artefato.GetValue("name").Value<string>() == target.gameObject.name)
                    {
                        target.personagemID = ((JArray)artefato["artifacts"]["data"])[0]["id"].Value<int>();
                        target.personagemName = ((JArray)artefato["artifacts"]["data"])[0]["attributes"]["name"].Value<string>();

                        try
                        {
                            var linkarray = ((JArray)((JArray)artefato["artifacts"]["data"])[0]["attributes"]["description"])[0]["children"];
                            target.personagemURL = linkarray[0]["text"].Value<string>();
                        }
                        catch (System.Exception ex) { 
                        }
                        target.finded = false;
                        break;
                    }

                }
            }
            initilizedTargets = true;
        }

        var collectResponse = await networkManager.CallWebMethodAsync("/api/collectibles?populate=artifact",
           Utils.getAutentication()
           , "GET", true, null);
        if (!collectResponse.Success)
        {
            gameObject.SetActive(false);
            return;
        }
        JArray colletados = (JArray)collectResponse.responseJson["data"];
        foreach (JObject colletado in colletados)
        {
            int artefatoID = colletado["attributes"]["artifact"]["data"]["id"].Value<int>();
            foreach (figurinhaManager target in Allfigurinhas)
            {
                
                if (target.personagemID == artefatoID)
                {
                    target.finded = true;
                }

            }
            Utils.saveCollectPersonagem(artefatoID);
        }

        foreach (figurinhaManager target in Allfigurinhas)
        {
            target.setCardImage();
        }
    }
    void Start()
    {

    }
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
