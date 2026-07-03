using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mainScene, galeriaScene, collectBt,gridPrefab;
    public List<string> figurinhas= new List<string> { "target1", "target2", "target3", "target4" };
    public List<Sprite> figurinhasImg;
    List<Boolean> figurinhasFind= new List<Boolean>();
    public int currentItem = -1;
    public GridLayoutGroup figurinhaGrid;
    void Start()
    {
        mainScene.SetActive(true);
        galeriaScene.SetActive(false);
     

        currentItem = -1;
        int cnt = 0;
        foreach (string f in figurinhas)
        {
            figurinhasFind.Add(false);
            GameObject figurinha= Instantiate(gridPrefab, figurinhaGrid.transform);
            figurinhaObj fobj= figurinha.GetComponent<figurinhaObj>();
            fobj.figImage = figurinhasImg[cnt];
            fobj.myNum= cnt+1;
            fobj.idCode = f;
            fobj.Start();
            cnt++;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    public void CollectButton()
    {
        if(currentItem>=0)
        {
            var fig = findFigurinhaById(figurinhas[currentItem]);
            if(fig!=null)
            {
                fig.collected = true;
            }
        }
        collectBt.gameObject.SetActive(false);

    }

    public figurinhaObj findFigurinhaById(string id)
    {
        foreach (figurinhaObj f in FindObjectsOfType<figurinhaObj>(true))
        {
           if(f.idCode==id)
                return f;
        }
        return null;
    }

    public void OpenAlbum()
    {
        foreach (figurinhaObj f in FindObjectsOfType<figurinhaObj>(true)) {
            f.updateImages();
        }
        galeriaScene.SetActive(true);

    }
    public void CloseAlbum()
    {
        galeriaScene.SetActive(false);

    }
    public void TargetDetected(string String)
    {
        collectBt.gameObject.SetActive(true);
        currentItem = figurinhas.IndexOf(String);
    }
    public void TargetLost(string String)
    {
        collectBt.gameObject.SetActive(false);
        currentItem = -1;


    }
}
