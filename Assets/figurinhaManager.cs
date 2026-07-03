using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class figurinhaManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Image myimg;
    public Sprite cardSprite,oldsprite;
    public int personagemID;
    public string personagemName,personagemURL;
    public bool finded;
    private Outline outline;
    private void Awake()
    {
        myimg = GetComponent<Image>();
        outline = GetComponent<Outline>();
        oldsprite=myimg.sprite;
    }
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);

    }

    // Update is called once per frame
    void Update()
    {
       
        
    }
    private void OnButtonClick()
    {
        if (finded)
        {
            AppController.Instance.setPersonagemInfo(personagemName, personagemURL, myimg.sprite);
        }
    }
    public void setCardImage()
    {
        if(myimg == null)
        {
            myimg = GetComponent<Image>();
        }
        if(outline == null)
        {
            outline = GetComponent<Outline>();

        }
        myimg.sprite = finded? cardSprite : oldsprite;
        outline.enabled = false;
    }
}
