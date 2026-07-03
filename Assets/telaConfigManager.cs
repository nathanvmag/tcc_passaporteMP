using UnityEngine;

public class telaConfigManager : MonoBehaviour
{
    NetworkManager networkManager;
    public GameObject Sair, Deletar,login;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Awake()
    {
        networkManager = FindFirstObjectByType<NetworkManager>();

    }
    private async  void OnEnable()
    {
        bool auth = await Utils.checkAuth(networkManager);
        Sair.SetActive(auth);
        Deletar.SetActive(auth);
        login.SetActive(!auth);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
