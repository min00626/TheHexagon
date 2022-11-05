using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes2D;

public class HexagonCtrl : MonoBehaviour
{
    public Color[] colors = new Color[3];

    public bool isComplete = false;
    public bool isSelected = false;
    public SpriteRenderer spriteRenderer;
    public ParticleSystem particle;
    public List<ParticleSystem> popParticle = new List<ParticleSystem>();

    public SpriteRenderer[] diamondArray = new SpriteRenderer[3];

    public Coroutine revealCoroutine = null;
    public Coroutine spinCoroutine = null;
    public Coroutine moveCoroutine = null;

    public Quaternion destinationRot = Quaternion.identity;

    public int x, y;
    public Vector3 pos;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color first, Color second, Color third)
    {
        diamondArray[0].color = first;
        diamondArray[1].color = second;
        diamondArray[2].color = third;
        colors[0] = first;
        colors[1] = second;
        colors[2] = third;
    }

    public void Complete(bool boo)
    {
        if (boo)
        {
            if (!isComplete) particle.Play();
            isComplete = true;
            transform.localScale = Vector3.one * 1.2f;
        }
        else
        {
            isComplete = false;
            transform.localScale = Vector3.one;
        }
    }

    public IEnumerator Reveal(bool complete)
    {
        Vector3 finalDestination = Vector3.one;
        if (complete) finalDestination *= 1.2f;
        Vector3 bigger = finalDestination * 1.1f;
        Vector3 initial = Vector3.one*.01f;

        transform.localScale = initial;
        while((bigger - transform.localScale).magnitude > .1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, bigger, Time.deltaTime * 15f);
            yield return null;
        }
        transform.localScale = finalDestination;
        revealCoroutine = null;
    }

    public void Spin()
    {
        destinationRot.eulerAngles = destinationRot.eulerAngles + new Vector3(0, 0, -120);
        if (spinCoroutine == null) spinCoroutine = StartCoroutine(SpinCoroutine());
        Color temp = colors[2];
        colors[2] = colors[1];
        colors[1] = colors[0];
        colors[0] = temp;
    }

    public void ReverseSpin()
    {
        destinationRot.eulerAngles = destinationRot.eulerAngles + new Vector3(0, 0, 120);
        if (spinCoroutine == null) spinCoroutine = StartCoroutine(SpinCoroutine());
        Color temp = colors[0];
        colors[0] = colors[1];
        colors[1] = colors[2];
        colors[2] = temp;
    }

    IEnumerator SpinCoroutine()
    {
        while(transform.rotation != destinationRot)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, destinationRot, Time.deltaTime * 10f);
            yield return null;
        }
        spinCoroutine = null;
    }

    public void Pop()
    {
        if (revealCoroutine != null) StopCoroutine(revealCoroutine);
        if (spinCoroutine != null) StopCoroutine(spinCoroutine);
        for (int i = 0; i < popParticle.Count; i++) popParticle[i].Play();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity = new Vector3(Random.Range(-4f, 4f), 6, 0);
        SphereCollider coll = GetComponent<SphereCollider>();
        coll.isTrigger = false;
        transform.position += Vector3.back;
        StartCoroutine(FallCoroutine(rb));
    }

    IEnumerator FallCoroutine(Rigidbody rb)
    {
        rb.AddTorque(Vector3.forward * Random.Range(-.1f, .1f)*500);
        while (transform.position.y > -20)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 1200,ForceMode.Acceleration);
            yield return null;
        }
        Destroy(gameObject, 1);
    }

    public void Move(Vector3 destination)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        GetComponent<SphereCollider>().enabled = false;
        moveCoroutine = StartCoroutine(MoveCoroutine(destination));
    }

    IEnumerator MoveCoroutine(Vector3 destination)
    {
        while ((destination - transform.position).magnitude > .1f)
        {
            transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * 15f);
            yield return null;
        }
        transform.position = destination;
        moveCoroutine = null;
        GetComponent<SphereCollider>().enabled = true;
    }
}
