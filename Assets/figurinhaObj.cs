using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class figurinhaObj : MonoBehaviour
{
    // Start is called before the first frame update
    public int myNum;
    public string idCode;

    public Sprite figImage;
    public bool collected;
    Image image;
    Text text;

   public void Start()
    {
        image= GetComponent<Image>();
        text=GetComponentInChildren<Text>(true);
        
    }
   

    // Update is called once per frame
    void Update()
    {
        
    }
    public void updateImages()
    {
        if(collected)
        {
            image.sprite = figImage;
            image.color= Color.white;
            text.gameObject.SetActive(false);
        }
        else
        {
            image.sprite = null;
            image.color = Color.gray;
            text.gameObject.SetActive(true);
            text.text = myNum.ToString();
        }
    }
}
