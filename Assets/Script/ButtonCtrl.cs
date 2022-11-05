using Shapes2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCtrl : MonoBehaviour
{
    public Coroutine moveCoroutine = null;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public void Reveal()
    {
        gameObject.SetActive(true);

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(RevealCoroutine());

    }

    public void Hide()
    {
        animator.enabled = false;
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(HideCoroutine());
    }

    IEnumerator RevealCoroutine()
    {

        Vector3 finalDestination = Vector3.one;
        Vector3 bigger = finalDestination * 1.1f;
        Vector3 initial = Vector3.one * .01f;

        transform.localScale = initial;
        while ((bigger - transform.localScale).magnitude > .1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, bigger, Time.deltaTime * 15f);
            yield return null;
        }
        transform.localScale = finalDestination;
        moveCoroutine = null;
        animator.enabled = true;
    }

    IEnumerator HideCoroutine()
    {
        Vector3 finalDestination = Vector3.one * .01f;
        Vector3 initial = Vector3.one;

        transform.localScale = initial;
        while ((finalDestination - transform.localScale).magnitude > .1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, finalDestination, Time.deltaTime * 15f);
            yield return null;
        }
        transform.localScale = finalDestination;
        moveCoroutine = null;
        gameObject.SetActive(false);
    } 
}
