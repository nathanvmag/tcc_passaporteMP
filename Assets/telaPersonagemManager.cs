using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class telaPersonagemManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Image personaImage;
    public TMP_Text personaText;
    public string personaLink;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setPersonagem(string persona,string personaLink,Sprite sprite)
    {
        personaImage.sprite = sprite;
        personaText.text= persona;
        this.personaLink = personaLink;
    }
    public void openPersonaLink()
    {
        Application.OpenURL(personaLink);
    }

}
