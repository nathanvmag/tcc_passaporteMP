using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class LoginManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int mode;
    public GameObject  senhaField, senhaText, esqueceuBt,cadastraAgrText,voltarTX,collectionScene,resetPassOBJ;
    public TMP_InputField emailField,codeField,newPassField;
    public TMP_Text crieSuacontaTx;
    public Text entraBTText;
    private NetworkManager networkManager;
    public Image buttonBtImage;
    public Sprite entrabtImg, criaBtImg, enviaBtImg;
    private int nextScene = 0;
    void OnEnable()
    {
        mode = 0;
    }
    private void Start()
    {
        networkManager = FindFirstObjectByType<NetworkManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if(mode == 0)
        {
            senhaField.SetActive(true);
            senhaText.SetActive(true);
            esqueceuBt.SetActive(true);
            cadastraAgrText.SetActive(true);
            entraBTText.text = "ENTRAR";
            buttonBtImage.sprite= entrabtImg;
            crieSuacontaTx.text = "<b>Entre na sua conta</b> para preencher seu passaporte";
            voltarTX.SetActive(false);
        }
        else if(mode == 1)
        {
            senhaField.SetActive(false);
            senhaText.SetActive(false);
            esqueceuBt.SetActive(false);
            cadastraAgrText.SetActive(false);
            entraBTText.text = "ENVIAR";
            buttonBtImage.sprite = enviaBtImg;

            crieSuacontaTx.text = "<b>Recupere a sua conta</b> e continue sua jornada";
            voltarTX.SetActive(true);

        }
        else if (mode == 2)
        {
            senhaField.SetActive(true);
            senhaText.SetActive(true);
            esqueceuBt.SetActive(false);
            cadastraAgrText.SetActive(false);
            entraBTText.text = "CRIAR";
            buttonBtImage.sprite = criaBtImg;

            crieSuacontaTx.text = "<b>Crie a sua conta</b> para preencher seu passaporte";
            voltarTX.SetActive(true);

        }

    }
    public async void submitBt()
    {
        string email = emailField.text;
        string pass = senhaField.GetComponent<TMP_InputField>().text;

       if(!Utils.ValidateEmail(email))
        {
            PopupManager.Instance.showPopUp("Email inválido!");
            return;
        }
        if (!Utils.ValidatePass(pass)&&mode!=1)
        {
            PopupManager.Instance.showPopUp("A senha deve ter 8 ou mais caracteres!");
            return;
        }
        NetworkResponse response = null;

        JObject keyValuePairs = new JObject();

        switch (mode)
        {
            case 0:
                keyValuePairs.Add(new JProperty("identifier", email));
                keyValuePairs.Add(new JProperty("password", pass));
                response= await networkManager.CallWebMethodAsync("/api/auth/local", "", "POST", true, keyValuePairs.ToString());
                
                break;
            case 1:
                keyValuePairs.Add(new JProperty("email", email));
              
                response = await networkManager.CallWebMethodAsync("/api/auth/forgot-password", "", "POST", true, keyValuePairs.ToString());


                break;
            case 2:
                keyValuePairs.Add(new JProperty("email", email));
                keyValuePairs.Add(new JProperty("password", pass));
                keyValuePairs.Add("project", AppController.Instance.getProjectID());
                response = await networkManager.CallWebMethodAsync("/api/auth/local/register", "", "POST", true, keyValuePairs.ToString());


                break;
        }
        if (response.Success)
        {
            if (mode == 0 || mode == 2)
            {
                try
                {
                    PlayerPrefs.SetString("auth", response.responseJson["jwt"].Value<string>());
                    PlayerPrefs.SetInt("myid", response.responseJson["user"].Value<int>("id"));
                    PlayerPrefs.Save();
                    PlayerPrefs.Save();
                }
                catch
                {

                }
                if (nextScene == 1)
                {
                    collectionScene.SetActive(true);
                    nextScene= 0;
                }
                gameObject.SetActive(false);
            }
            else if(mode == 1)
            {
                PopupManager.Instance.showPopUp("Foi enviado um email com o código para a recuperação da sua senha!");
                resetPassOBJ.SetActive(true);
            }
        }
    }
    public async void resetSenha()
    {
        string code = codeField.text;
        string newpass = newPassField.text;
        NetworkResponse response = null;
        JObject keyValuePairs = new JObject
        {
            new JProperty("code", code),
            new JProperty("password", newpass),
            new JProperty("passwordConfirmation", newpass)
        };

        response = await networkManager.CallWebMethodAsync("/api/auth/reset-password", "", "POST", true, keyValuePairs.ToString());
        if(response.Success)
        {
            PopupManager.Instance.showPopUp("Sucesso ao resetar senha!!");
            mode = 0;
            resetPassOBJ.SetActive(false);

        }

    }
    public void esqueceuSenhaBt()
    {
        print("clickEsquece");
        mode = 1;
    }
    public void cadastrarBt()
    {
        print("clickCadastra");
        mode = 2;

    }
    public void voltarBt()
    {
        mode = 0;
    }
    public void setNextScene(int scene)
    {
        nextScene= scene;
    }
}
