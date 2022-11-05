using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public List<GameObject> hive = new List<GameObject>();

    public GameObject mainText;
    public SelectManager levelSel;
    public SelectManager difficultySel;
    public GameObject start;
    public GameObject panel;
    public GameObject achievementButton;
    public GameObject leaderboardButton;

    bool isStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Initial());
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted)
        {
            for(int i=0; i < 3; i++)
            {
                if (i == levelSel.idx) hive[i].SetActive(true);
                else hive[i].SetActive(false);
            }
        }
    }

    IEnumerator Initial()
    {
        achievementButton.GetComponent<Button>().onClick.AddListener(PlaystoreCtrl.Instance.ShowAchievement);
        leaderboardButton.GetComponent<Button>().onClick.AddListener(PlaystoreCtrl.Instance.ShowLeaderboard);

        yield return null;
        mainText.SetActive(false);
        panel.SetActive(false);
        for (int i = 0; i < 3; i++) hive[i].SetActive(false);
        levelSel.gameObject.SetActive(false);
        difficultySel.gameObject.SetActive(false);
        start.SetActive(false);
        achievementButton.SetActive(false);
        leaderboardButton.SetActive(false);

        yield return new WaitForSeconds(.5f);
        mainText.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        levelSel.gameObject.SetActive(true);
        levelSel.rightButton.Reveal();
        difficultySel.gameObject.SetActive(true);
        difficultySel.rightButton.Reveal();
        achievementButton.SetActive(true);
        leaderboardButton.SetActive(true);
        start.SetActive(true);
        isStarted = true;
    }

    public void LMGMountedAndLoad()
    {
        PlayerPrefs.Save();
        panel.SetActive(true);
        panel.GetComponent<Image>().color = Color.clear;
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        start.GetComponent<Animator>().SetTrigger("Press");
        for (int i = 0; i < 3; i++) hive[i].GetComponent<Animator>().SetTrigger("Hide");
        levelSel.Hide();
        difficultySel.Hide();
        mainText.GetComponent<Animator>().SetTrigger("Hide");
        achievementButton.GetComponent<Animator>().SetTrigger("Hide");
        leaderboardButton.GetComponent<Animator>().SetTrigger("Hide");

        yield return new WaitForSeconds(.2f);
        start.GetComponent<Animator>().SetTrigger("Hide");

        yield return new WaitForSeconds(.4f);
        SceneManager.LoadScene(1);

    }
}
