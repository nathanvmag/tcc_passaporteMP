using UnityEngine;
using TMPro;
public class DescontoManager : MonoBehaviour
{
    public TMP_Text titleText, descontoText, priceText, descricaoText;
    public UnityEngine.UI.Text tituloDesc;
    public GameObject descPopup;
    public int currentPrice;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void montDiscount(string title, string desconto, int price, string descricao)
    {
        titleText.text = title;
        descontoText.text = desconto;
        priceText.text = price.ToString()+" Pts";
        descricaoText.text = descricao.ToString();
        currentPrice = price;
    }
    public void BuyDiscount()
    {
        if(currentPrice<= AppController.Instance.myPontos)
        {
            AppController.Instance.myPontos -= currentPrice;
            AppController.Instance.saldoText.text = AppController.Instance.myPontos.ToString();
            // Implement further purchase logic here
            descPopup.SetActive(true);
            tituloDesc.text = titleText.text;
        }
        else
        {
            PopupManager.Instance.showPopUp( "Você não possui pontos suficientes para realizar esta compra.");
        }
    }
}
