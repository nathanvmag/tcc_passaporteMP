using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TargetController : MonoBehaviour
{
    // Start is called before the first frame update
    ImageTargetBehaviour imgTarget;
    public string slugText,targetText,targetLink;
    public int targetID;
    void Start()
    {
        targetID = -1;
        imgTarget = GetComponent<ImageTargetBehaviour>();
        targetText = imgTarget.TargetName;
        slugText= (Utils.Slugify(imgTarget.TargetName));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Seen()
    {
        if(transform.childCount>0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        AppController.Instance.SeenTarget(targetID);


    }
    public void UnSeen()
    {
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        try
        {
            AppController.Instance.UnSeenTarget(targetID);
        }
        catch
        {

        }
    }
}
