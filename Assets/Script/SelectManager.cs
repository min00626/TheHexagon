using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    public Text text;
    Animator textAC;
    public ButtonCtrl leftButton, rightButton;

    public int range;
    public int idx=0;

    public enum SelectEnum { level, difficulty}
    public SelectEnum selectEnum;

    public List<string> textString = new List<string>();

    private void Awake()
    {
        leftButton.gameObject.SetActive(false);
        rightButton.Reveal();

        if (selectEnum == SelectEnum.level) PlayerPrefs.SetInt("level", idx);
        else PlayerPrefs.SetInt("difficulty", idx);
        textAC = text.GetComponent<Animator>();
    }

    public void Left()
    {
        if (idx == 0) return;
        idx--;
        textAC.SetTrigger("Reveal");
        if (idx <= textString.Count) text.text = textString[idx];
        if (idx == 0) leftButton.Hide();
        if (idx == range-2) rightButton.Reveal();
        if (selectEnum == SelectEnum.level) PlayerPrefs.SetInt("level", idx);
        else PlayerPrefs.SetInt("difficulty", idx);
    }

    public void Right()
    {
        if (idx >= range - 1) return;
        idx++;
        textAC.SetTrigger("Reveal");
        if (idx<=textString.Count) text.text = textString[idx];
        if (idx == range - 1) rightButton.Hide();
        if (idx == 1) leftButton.Reveal();
        if (selectEnum == SelectEnum.level) PlayerPrefs.SetInt("level", idx);
        else PlayerPrefs.SetInt("difficulty", idx);
    }

    public void Hide()
    {
        if (leftButton.gameObject.activeSelf) leftButton.Hide();
        if (rightButton.gameObject.activeSelf) rightButton.Hide();
        textAC.SetTrigger("Hide");
    }
}
