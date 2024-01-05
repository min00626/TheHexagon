using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int level = 0;
    public int difficulty = 0;
    public List<HexagonData> hexagonDatas = new List<HexagonData>();
}
