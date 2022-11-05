using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes2D;

public class EllipseCtrl : MonoBehaviour
{
    Shape shape;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        shape = GetComponent<Shape>();
        animator = GetComponent<Animator>();
        StartCoroutine(Ellipse());
    }

    IEnumerator Ellipse()
    {
        shape.settings.startAngle = 359.9f;
        yield return null;
        float tim = 0;
        while (shape.settings.startAngle > 0)
        {
            shape.settings.startAngle -= (Mathf.Cos(tim) + 1.2f) * Time.deltaTime * 200;
            tim += Time.deltaTime * 6;
            yield return null;
        }
        shape.settings.startAngle = 0;
        animator.SetTrigger("Enable");
    }

    public void End()
    {
        StartCoroutine(EllipseClose());
    }

    IEnumerator EllipseClose()
    {
        yield return null;
        float tim = 0;
        while (shape.settings.endAngle > .01f)
        {
            shape.settings.endAngle -= (Mathf.Cos(tim) + 1.2f) * Time.deltaTime * 200;
            if (shape.settings.endAngle < .1f) break;
            tim += Time.deltaTime * 6;
            yield return null;
        }
        shape.settings.endAngle = .01f;
        animator.SetTrigger("Toggle");
    }
}
