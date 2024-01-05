using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexagonData
{
    public HexagonData(Color f, Color s, Color t)
	{
        first[0] = f.r;
        first[1] = f.g;
        first[2] = f.b;
        second[0] = s.r;
        second[1] = s.g;
        second[2] = s.b;
        third[0] = t.r;
        third[1] = t.g;
        third[2] = t.b;
    }
    public Color GetColor(int t)
	{
        if (t == 0)
        {
            return new Color(first[0], first[1], first[2]);
        }
        else if (t == 1)
        {
            return new Color(second[0], second[1], second[2]);
        }
        else
        {
            return new Color(third[0], third[1], third[2]);
        }
    }
    public float[] first = new float[3];
    public float[] second = new float[3];
    public float[] third = new float[3];
}
