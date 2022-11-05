using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonCtrl : MonoBehaviour
{
    public Text text;
    public Animator animator;
    string prevText;
    int size;

    public ButtonCtrl confirm, cancel;

    public RectTransform holder;

    private void Start()
    {
        prevText = text.text;
        size = text.fontSize;
        Invoke("HideButton", .2f);
    }

    void HideButton()
    {
        confirm.gameObject.SetActive(false);
        cancel.gameObject.SetActive(false);
    }

    public void Press()
    {
        text.text = "Really?\n";
        text.fontSize = 100;
        confirm.Reveal();
        cancel.Reveal();
    }

    public void Confirm()
    {
        transform.SetParent(holder);
        confirm.Hide();
        cancel.Hide();
        animator.SetTrigger("Load");
    }

    public void Cancel()
    {
        text.text = prevText;
        text.fontSize = size;
        confirm.Hide();
        cancel.Hide();
    }
}
