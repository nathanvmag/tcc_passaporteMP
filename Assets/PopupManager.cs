using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text popupText;
    public static PopupManager Instance { get; private set; }
    private void Awake()
    {
        // Verifica se já existe uma instância do GameManager
        if (Instance == null)
        {
            Debug.Log("Iniciando controller");
            Instance = this;
            DontDestroyOnLoad(gameObject); // Garante que o objeto não será destruído ao trocar de cena
        }
        else
        {
            Destroy(gameObject); // Se já existir uma instância, destrói a duplicada
        }
        gameObject.SetActive(false);
    }
    void Start()
    {
        //gameObject.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void showPopUp(string text)
    {
        popupText.text = text;

        gameObject.SetActive(true);
    }
}
