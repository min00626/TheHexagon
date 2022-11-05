using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class PlaystoreCtrl : MonoBehaviour
{
    
    private static PlaystoreCtrl instance = null;
    public static PlaystoreCtrl Instance
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

        AuthenticateUser();
    }

    void AuthenticateUser()
    {
        
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        
        Social.Active.localUser.Authenticate(success=> {
            Debug.Log(Social.Active.localUser.authenticated);
        });
    }

    void SignInCallback(bool success)
    {
        if (success)
        {
            Debug.Log("dfdf");
        }
        else
        {
            if (!PlayGamesPlatform.Instance.localUser.authenticated)
            {
                PlayGamesPlatform.Instance.Authenticate((result)=> { }, false);
            }
            else
            {
                PlayGamesPlatform.Instance.SignOut();
            }
        }
    }

    public static void SetAchivement()
    {
        int i = PlayerPrefs.GetInt("level", 0) * 2 + PlayerPrefs.GetInt("difficulty", 0);
        switch (i)
        {
            case 0: 
                Social.ReportProgress(GPGSIds.achievement_level_1_normal,100f,null);
                break;
            case 1:
                Social.ReportProgress(GPGSIds.achievement_level_1_expert, 100f, null);
                break;
            case 2:
                Social.ReportProgress(GPGSIds.achievement_level_2_normal, 100f, null);
                break;
            case 3:
                Social.ReportProgress(GPGSIds.achievement_level_2_expert, 100f, null);
                break;
            case 4:
                Social.ReportProgress(GPGSIds.achievement_level_3_normal, 100f, null);
                break;
            case 5:
                Social.ReportProgress(GPGSIds.achievement_level_3_expert, 100f, null);
                break;
            default:
                break;
        }

        PlayerPrefs.SetInt("count", PlayerPrefs.GetInt("count", 0) + 1);
        i = PlayerPrefs.GetInt("count", 1);
        Social.ReportScore(i, GPGSIds.leaderboard_clear_count, success=> {
            if (success) Debug.Log("Reported score on leaderboard");
        });

        if (i == 1)
        {
            Social.ReportProgress(GPGSIds.achievement_hexagon_novice, 100f, null);
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_hexagon_expert, i, null);
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_hexagon_master, i, null);
        }
        else if (i <= 10)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_hexagon_expert, i, null);
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_hexagon_master, i, null);
        }
        else if (i <= 100)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_hexagon_master, i, null);
        }

        PlayerPrefs.Save();
    }

    public void ShowAchievement()
    {
        Social.ShowAchievementsUI();
    }

    public void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }
}
