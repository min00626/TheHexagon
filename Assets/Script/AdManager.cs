using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    string gameId = "3804557";
    string placementId = "BannerAd";


    private static AdManager instance = null;
    public static AdManager Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        Advertisement.Initialize(gameId);
        StartCoroutine(ShowBannerWhenReady());
    }


    IEnumerator ShowBannerWhenReady()
    {
        yield return new WaitForSeconds(2);
        while (!Advertisement.IsReady(placementId))
        {
            Debug.Log("Not Ready");
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Ready");
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        Advertisement.Banner.Show(placementId);
    }
}
