using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    public enum ColorEnum { Red, Green, Blue, Yellow, Magenta, Cyan }
    public Color[] ColorArray = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan };

    HexagonCtrl[,] hexagons = new HexagonCtrl[23, 23];
    int level = 1;
    int difficulty = 0;
    int hexagonCount;

    float y = .85f;
    float x = 1;

    public GameObject hexagon;
    public GameObject rainbowHexagon;
    public Camera mainCamera;
    public GameObject ellipse;

    public HexagonCtrl selectedHex = null, movedHex=null;

    LayerMask hexagonLayer;

    public ButtonCtrl spinButton, cancelButton, undoButton, menuButton;
    public GameObject panel;
    public GameObject ellipsePanel;
    public Animator menu;

    List<UndoStruct> undoStack = new List<UndoStruct>();

    bool isMenu = false;
    bool dragEnabled = false;
    bool snap = false;

    AudioCtrl audioCtrl;

    struct UndoStruct
    {
        public UndoStruct(bool boo, Vector2Int f, Vector2Int s){
            isSpin = boo;
            first = f;
            second = s;
        }
        public bool isSpin;
        public Vector2Int first;
        public Vector2Int second;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Input.mousePosition;
            RaycastHit hit;
            Physics.Raycast(mainCamera.ScreenToWorldPoint(pos), Vector3.forward, out hit, 20, 1 << hexagonLayer);

            if (hit.collider != null)
            {
                if (selectedHex == null)
                {
                    selectedHex = hit.collider.gameObject.GetComponent<HexagonCtrl>();
                    selectedHex.isSelected = true;

                    spinButton.Reveal();
                    cancelButton.Reveal();
                }
                else
                {
                    HexagonCtrl hexagonCtrl = hit.collider.gameObject.GetComponent<HexagonCtrl>();
                    ChangePosition(selectedHex, hexagonCtrl, false);
                    selectedHex.isSelected = false;
                    selectedHex.spriteRenderer.color = Color.white;
                    selectedHex = null;

                    spinButton.Hide();
                    cancelButton.Hide();
                }
            }
        }
        */
        if (!isMenu && Input.touchCount > 0)
        {
            Vector3 pos = Input.touches[0].position;
            RaycastHit hit;
            Physics.Raycast(mainCamera.ScreenToWorldPoint(pos), Vector3.forward, out hit, 20, 1 << hexagonLayer);

            Vector3 tempP = mainCamera.ScreenToWorldPoint(pos);
            tempP.z = 0;
            if (selectedHex!=null&& (selectedHex.pos -tempP).magnitude > .5f) snap = true;

            HexagonCtrl hexagonCtrl = null;
            if (hit.collider != null) hexagonCtrl = hit.collider.GetComponent<HexagonCtrl>();

            if (Input.touches[0].phase == TouchPhase.Began)
            {
                snap = false;
                if (hit.collider != null)
                {
                    dragEnabled = true;

                    if (selectedHex != null)
                    {
                        selectedHex.isSelected = false;
                        selectedHex.spriteRenderer.color = Color.white;
                        selectedHex.GetComponent<SphereCollider>().enabled = true;
                        selectedHex = null;
                    }

                    selectedHex = hexagonCtrl;
                    selectedHex.isSelected = true;
                    selectedHex.GetComponent<SphereCollider>().enabled = false;

                    Vector3 temp = selectedHex.transform.position;
                    temp.z = -.1f;
                    selectedHex.transform.position = temp;
                }
                else
                {
                    dragEnabled = false;
                }
            }
            else if (selectedHex!=null&& Input.touches[0].phase == TouchPhase.Ended)
            {

                if (movedHex == null)
                {
                    if((selectedHex.pos -selectedHex.transform.position).magnitude < .5f)
                    {
                        selectedHex.GetComponent<SphereCollider>().enabled = true;

                        Vector3 temp = selectedHex.pos;
                        temp.z = -.1f;
                        selectedHex.transform.position = temp;

                        if (!spinButton.gameObject.activeSelf) spinButton.Reveal();
                        if (!cancelButton.gameObject.activeSelf) cancelButton.Reveal();
                    }
                    else
                    {
                        selectedHex.isSelected = false;
                        selectedHex.spriteRenderer.color = Color.white;
                        selectedHex.spriteRenderer.sortingOrder = 0;
                        selectedHex.GetComponent<SphereCollider>().enabled = true;
                        selectedHex.transform.position = selectedHex.pos;
                        selectedHex = null;
                    }
                }
                else
                {
                    Vector3 temp = selectedHex.transform.position;
                    temp.z = 0;
                    selectedHex.transform.position = temp;

                    ChangePosition(selectedHex, movedHex, false);
                    if (selectedHex != null)
                    {
                        selectedHex.isSelected = false;
                        selectedHex.spriteRenderer.color = Color.white;
                        selectedHex.spriteRenderer.sortingOrder = 0;
                        selectedHex.GetComponent<SphereCollider>().enabled = true;
                        selectedHex = null;
                    }

                    if(spinButton.gameObject.activeSelf) spinButton.Hide();
                    if(cancelButton.gameObject.activeSelf) cancelButton.Hide();
                }
                if (movedHex != null)
                {
                    movedHex.spriteRenderer.color = Color.white;
                    movedHex.Move(movedHex.pos);
                    movedHex = null;
                }
            }
            else if(dragEnabled)
            {
                Vector3 temp = mainCamera.ScreenToWorldPoint(pos);
                temp.z = -.1f;
                if (selectedHex != null && (snap || (selectedHex.pos - temp).magnitude > .5f))
                {
                    selectedHex.transform.position = temp;
                }
                if (hit.collider != null&&selectedHex!=null)
                {
                    if (movedHex != null)
                    {
                        movedHex.spriteRenderer.color = Color.white;
                        movedHex.Move(movedHex.pos);
                        if (movedHex == hexagonCtrl)
                        {
                            movedHex = null;
                        }
                        else movedHex = hexagonCtrl;
                    }
                    else
                    {
                        movedHex = hexagonCtrl;
                        hexagonCtrl.Move(selectedHex.pos);
                    }
                }
            }
        

            /*
            if(hit.collider != null)
            {
                if (selectedHex == null)
                {
                    selectedHex = hit.collider.gameObject.GetComponent<HexagonCtrl>();
                    selectedHex.isSelected = true;

                    Vector3 temp = selectedHex.transform.position;
                    temp.z = -.1f;
                    selectedHex.transform.position = temp;

                    spinButton.Reveal();
                    cancelButton.Reveal();
                }
                else
                {
                    Vector3 temp = selectedHex.transform.position;
                    temp.z = 0;
                    selectedHex.transform.position = temp;

                    HexagonCtrl hexagonCtrl = hit.collider.gameObject.GetComponent<HexagonCtrl>();
                    ChangePosition(selectedHex, hexagonCtrl, false);
                    selectedHex.isSelected = false;
                    selectedHex.spriteRenderer.color = Color.white;
                    selectedHex = null;

                    spinButton.Hide();
                    cancelButton.Hide();
                }
            }
            */
        }

        if(selectedHex != null)
        {
            selectedHex.spriteRenderer.color = Color.black * (Mathf.Sin(Time.time*6) + 1) * .5f;
            selectedHex.spriteRenderer.sortingOrder = 1;
        }
        if (movedHex != null)
        {
            movedHex.spriteRenderer.color = Color.black * (Mathf.Sin(Time.time * 6) + 1) * .5f;
        }
    }

    void Initialize()
    {
        level = PlayerPrefs.GetInt("level",0) + 1;
        difficulty = PlayerPrefs.GetInt("difficulty",0);



        audioCtrl = AudioCtrl.Instance;

        isMenu = false;
        hexagonLayer = LayerMask.NameToLayer("Hexagon");
        mainCamera.orthographicSize += 2f * (level - 1);
        ellipse.transform.localScale += Vector3.one * (level - 1) * 2f;
        Application.targetFrameRate = 60;
        //Screen.SetResolution(Screen.width, Screen.width * 2, true);

        int colorCount = (difficulty + 1) * 3;

        int hexaCount = 2 + (level - 1);
        Vector2 pos = new Vector2();
        pos.y = level * y;
        hexagonCount = 0;


        Serializer serializer = GetComponent<Serializer>();     //load
        FileInfo fileInfo = new FileInfo(Path.Combine(Application.persistentDataPath, "GameData.dat"));
        GameData gameData=null;
        if (fileInfo.Exists)
        {
            serializer.DeSerialization("GameData", Application.persistentDataPath, out gameData); 
        }
        if (gameData!=null&& gameData.level == level && gameData.difficulty == difficulty)      //load game
        {
            Debug.Log("load");
            for (int i = 0; i < 1 + level * 2; i++)
            {
                pos.x = -x / 2.0f * (hexaCount - 1);

                for (int j = 0; j < hexaCount; j++)
                {
                    HexagonData hexagonData = gameData.hexagonDatas[0];
                    gameData.hexagonDatas.RemoveAt(0);

                    hexagons[i, j] = Instantiate(hexagon, pos, Quaternion.identity).GetComponent<HexagonCtrl>();
                    hexagons[i, j].pos = pos;

                    hexagons[i, j].SetColor(hexagonData.GetColor(0), hexagonData.GetColor(1), hexagonData.GetColor(2));
                    hexagons[i, j].y = i;
                    hexagons[i, j].x = j;
                    pos.x += x;
                    hexagonCount++;
                }
                if (i < level) hexaCount++;
                else hexaCount--;

                pos.y -= y;
            }

        }
        else
        {           // new game
            Debug.Log("new");
            gameData = new GameData();
            gameData.level = level;
            gameData.difficulty = difficulty;

            Color first, second, third;
            for (int i = 0; i < 1 + level * 2; i++)
            {
                pos.x = -x / 2.0f * (hexaCount - 1);

                for (int j = 0; j < hexaCount; j++)
                {
                    hexagons[i, j] = Instantiate(hexagon, pos, Quaternion.identity).GetComponent<HexagonCtrl>();
                    hexagons[i, j].pos = pos;
                    if (i == 0)
                    {
                        if (j == 0)
                        {
                            third = ColorArray[Random.Range(0, colorCount)];
                        }
                        else
                        {
                            third = hexagons[i, j - 1].colors[1];
                        }
                        second = ColorArray[Random.Range(0, colorCount)];
                        first = ColorArray[Random.Range(0, colorCount)];
                    }
                    else if (j == 0)
                    {
                        if (i <= level)
                        {
                            first = hexagons[i - 1, j].colors[2];
                        }
                        else
                        {
                            first = hexagons[i - 1, j].colors[1];
                        }
                        second = ColorArray[Random.Range(0, colorCount)];
                        third = ColorArray[Random.Range(0, colorCount)];
                    }
                    else
                    {
                        if (i <= level)
                        {
                            first = hexagons[i - 1, j - 1].colors[1];
                        }
                        else
                        {
                            first = hexagons[i - 1, j].colors[1];
                        }
                        second = ColorArray[Random.Range(0, colorCount)];
                        third = hexagons[i, j - 1].colors[1];
                    }
                    hexagons[i, j].SetColor(first, second, third);
                    HexagonData hexagonData = new HexagonData(first, second, third);
                    gameData.hexagonDatas.Add(hexagonData);
                    hexagons[i, j].y = i;
                    hexagons[i, j].x = j;
                    pos.x += x;
                    hexagonCount++;
                }
                if (i < level) hexaCount++;
                else hexaCount--;

                pos.y -= y;
            }

            //save game
            serializer.Serialization(gameData, "GameData", Application.persistentDataPath);


        }

        hexaCount = 2 + (level - 1);
        int tempX, tempY;
        for (int i = 0; i < 1 + level * 2; i++)
        {
            for (int j = 0; j < hexaCount; j++)
            {
                tempX = Random.Range(0, level + 1);
                tempY = Random.Range(0, 1 + level * 2);
                Vector3 tempVec = hexagons[tempY, tempX].transform.position;
                hexagons[tempY, tempX].transform.position = hexagons[i, j].transform.position;
                hexagons[i, j].transform.position = tempVec;
                HexagonCtrl tempHex = hexagons[tempY, tempX];
                hexagons[tempY, tempX] = hexagons[i, j];
                hexagons[i, j] = tempHex;
                int temp = hexagons[tempY, tempX].y;
                hexagons[tempY, tempX].y = hexagons[i, j].y;
                hexagons[i, j].y = temp;
                temp = hexagons[tempY, tempX].x;
                hexagons[tempY, tempX].x = hexagons[i, j].x;
                hexagons[i, j].x = temp;
                Vector3 tempVec3 = hexagons[i, j].pos;
                hexagons[i, j].pos = hexagons[tempY, tempX].pos;
                hexagons[tempY, tempX].pos = tempVec3;
            }
            if (i < level) hexaCount++;
            else hexaCount--;
        }



        Vector3 tempPos = hexagons[level, level].transform.position;
        Destroy(hexagons[level, level].gameObject);
        hexagons[level, level] = Instantiate(rainbowHexagon, tempPos, Quaternion.identity).GetComponent<HexagonCtrl>();
        hexagons[level, level].y = level;
        hexagons[level, level].x = level ;

        hexaCount = 2 + (level - 1);
        for (int i = 0; i < 1 + level * 2; i++)
        {
            for (int j = 0; j < hexaCount; j++)
            {
                hexagons[i, j].Complete(Check(hexagons[i, j]));
                hexagons[i, j].gameObject.SetActive(false);
            }
            if (i < level) hexaCount++;
            else hexaCount--;
        }

        if (IsEndGame()) SceneManager.LoadScene(1);


        StartCoroutine(RevealHexagons());
    }

    IEnumerator RevealHexagons()
    {
        yield return null;
        panel.SetActive(false);
        ellipsePanel.SetActive(false);
        spinButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        undoButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);

        int[,] temp = new int[23, 23];
        temp.Initialize();
        temp[level, level] = 1;
        yield return new WaitForSeconds(1.5f);
        for(int i=0; i<level+1; i++)
        {
            int hexaCount = 2 + (level - 1);
            for (int j = 0; j < 1 + level * 2; j++)
            {
                for (int k = 0; k < hexaCount; k++)
                {
                    if (temp[j, k] == 1+i)
                    {
                        HexagonCtrl hexagon = hexagons[j, k];
                        int up = 0, down = 0;
                        if (hexagon.y < level)
                        {
                            up--;
                        }
                        else if (hexagon.y == level)
                        {
                            up--;
                            down--;
                        }
                        else
                        {
                            down--;
                        }
                        if (!OutOfBound(hexagon.y - 1, hexagon.x + up))
                        {
                            if(temp[hexagon.y - 1, hexagon.x + up]==0) temp[hexagon.y - 1, hexagon.x + up] = i + 2;
                        }
                        if (!OutOfBound(hexagon.y - 1, hexagon.x + up + 1))
                        {
                            if (temp[hexagon.y - 1, hexagon.x + up+1] == 0) temp[hexagon.y - 1, hexagon.x + up+1] = i + 2;
                        }
                        if (!OutOfBound(hexagon.y, hexagon.x - 1))
                        {
                            if (temp[hexagon.y, hexagon.x - 1] == 0) temp[hexagon.y, hexagon.x - 1] = i + 2;
                        }
                        if (!OutOfBound(hexagon.y, hexagon.x + 1))
                        {
                            if (temp[hexagon.y, hexagon.x + 1] == 0) temp[hexagon.y, hexagon.x + 1] = i + 2;
                        }
                        if (!OutOfBound(hexagon.y + 1, hexagon.x + down))
                        {
                            if (temp[hexagon.y + 1, hexagon.x + down] == 0) temp[hexagon.y + 1, hexagon.x + down] = i + 2;
                        }
                        if (!OutOfBound(hexagon.y + 1, hexagon.x + down + 1))
                        {
                            if (temp[hexagon.y + 1, hexagon.x + down + 1] == 0) temp[hexagon.y + 1, hexagon.x + down + 1] = i + 2;
                        }
                        hexagon.gameObject.SetActive(true);
                        hexagon.revealCoroutine = StartCoroutine(hexagons[j, k].Reveal(hexagons[j,k].isComplete));
                    }
                }
                if (j < level) hexaCount++;
                else hexaCount--;
            }
            yield return new WaitForSeconds(.5f);
        }
        menuButton.gameObject.SetActive(true);
        menuButton.Reveal();
    }

    void ChangePosition(HexagonCtrl a, HexagonCtrl b, bool isUndo)
    {
        if (movedHex != null)
        {
            movedHex.spriteRenderer.color = Color.white;
            movedHex.GetComponent<SphereCollider>().enabled = true;
            movedHex = null;
        }

        a.transform.position = b.pos;
        b.transform.position = a.pos;
        HexagonCtrl tempHex = hexagons[a.y, a.x];
        hexagons[a.y, a.x] = hexagons[b.y, b.x];
        hexagons[b.y, b.x] = tempHex;
        int temp = a.y;
        a.y = b.y;
        b.y = temp;
        temp = a.x;
        a.x = b.x;
        b.x = temp;
        Vector3 tempVec3 = a.pos;
        a.pos = b.pos;
        b.pos = tempVec3;

        a.Complete(Check(a));
        CheckBound(a);
        b.Complete(Check(b));
        CheckBound(b);
        if (IsEndGame()) EndGame();
        if(!isUndo) AddToStack(new UndoStruct(false, new Vector2Int(a.y, a.x), new Vector2Int(b.y, b.x)));

        audioCtrl.PlayClip(1);

        if (a.revealCoroutine != null) StopCoroutine(a.revealCoroutine);
        a.revealCoroutine = StartCoroutine(a.Reveal(a.isComplete));
        if (b.revealCoroutine != null) StopCoroutine(b.revealCoroutine);
        b.revealCoroutine = StartCoroutine(b.Reveal(b.isComplete));
    }

    bool Check(HexagonCtrl hexagon)
    {
        if (hexagon.colors[0] == Color.white) return true;
        int up = 0, down = 0;
        if (hexagon.y < level)
        {
            up--;
        }
        else if(hexagon.y == level)
        {
            up--;
            down--;
        }
        else
        {
            down--;
        }
        if(!OutOfBound(hexagon.y-1, hexagon.x+up))
        {
            if (hexagons[hexagon.y - 1, hexagon.x + up].colors[1] != hexagon.colors[0] && hexagons[hexagon.y - 1, hexagon.x + up].colors[1] != Color.white) return false;
        }
        if (!OutOfBound(hexagon.y - 1, hexagon.x + up+1))
        {
            if (hexagons[hexagon.y - 1, hexagon.x + up+1].colors[2] != hexagon.colors[0] && hexagons[hexagon.y - 1, hexagon.x + up + 1].colors[2] != Color.white) return false;
        }
        if (!OutOfBound(hexagon.y, hexagon.x -1))
        {
            if (hexagons[hexagon.y, hexagon.x - 1].colors[1] != Color.white && hexagons[hexagon.y, hexagon.x -1].colors[1] != hexagon.colors[2]) return false;
        }
        if (!OutOfBound(hexagon.y, hexagon.x + 1))
        {
            if (hexagons[hexagon.y, hexagon.x + 1].colors[2] != Color.white && hexagons[hexagon.y, hexagon.x + 1].colors[2] != hexagon.colors[1]) return false;
        }
        if (!OutOfBound(hexagon.y + 1, hexagon.x + down))
        {
            if (hexagons[hexagon.y + 1, hexagon.x + down].colors[0] != Color.white && hexagons[hexagon.y + 1, hexagon.x + down].colors[0] != hexagon.colors[2]) return false;
        }
        if (!OutOfBound(hexagon.y + 1, hexagon.x + down+1))
        {
            if (hexagons[hexagon.y + 1, hexagon.x + down + 1].colors[0] != Color.white && hexagons[hexagon.y + 1, hexagon.x + down+1].colors[0] != hexagon.colors[1]) return false;
        }
        return true;
    }

    void CheckBound(HexagonCtrl hexagon)
    {
        int up = 0, down = 0;
        if (hexagon.y < level)
        {
            up--;
        }
        else if (hexagon.y == level)
        {
            up--;
            down--;
        }
        else
        {
            down--;
        }
        if (!OutOfBound(hexagon.y - 1, hexagon.x + up))
        {
            hexagons[hexagon.y - 1, hexagon.x + up].Complete(Check(hexagons[hexagon.y - 1, hexagon.x + up]));
        }
        if (!OutOfBound(hexagon.y - 1, hexagon.x + up + 1))
        {
            hexagons[hexagon.y - 1, hexagon.x + up+1].Complete(Check(hexagons[hexagon.y - 1, hexagon.x + up+1]));
        }
        if (!OutOfBound(hexagon.y, hexagon.x - 1))
        {
            hexagons[hexagon.y, hexagon.x -1].Complete(Check(hexagons[hexagon.y, hexagon.x -1]));
        }
        if (!OutOfBound(hexagon.y, hexagon.x + 1))
        {
            hexagons[hexagon.y, hexagon.x + 1].Complete(Check(hexagons[hexagon.y, hexagon.x + 1]));
        }
        if (!OutOfBound(hexagon.y + 1, hexagon.x + down))
        {
            hexagons[hexagon.y + 1, hexagon.x + down].Complete(Check(hexagons[hexagon.y + 1, hexagon.x + down]));
        }
        if (!OutOfBound(hexagon.y + 1, hexagon.x + down + 1))
        {
            hexagons[hexagon.y + 1, hexagon.x + down+1].Complete(Check(hexagons[hexagon.y + 1, hexagon.x + down+1]));
        }
    }

    bool OutOfBound(int y, int x)
    {
        if (y < 0 || y >= 1 + level * 2) return true;
        else if (x < 0) return true;
        int temp = level+1;
        if (y <= level) temp += y;
        else
        {
            temp += level;
            temp -= (y - level);
        }
        if (x >= temp) return true;
        return false;
    }

    public void Spin()
    {
        if (selectedHex != null) {
            audioCtrl.PlayClip(0);

            selectedHex.Spin();
            selectedHex.Complete(Check(selectedHex));
            CheckBound(selectedHex);
            if (IsEndGame()) EndGame();
            else AddToStack(new UndoStruct(true, new Vector2Int(selectedHex.y, selectedHex.x), Vector2Int.zero));
        }
    }

    public void ReverseSpin(HexagonCtrl hexagonCtrl)
    {
        audioCtrl.PlayClip(0);

        hexagonCtrl.ReverseSpin();
        hexagonCtrl.Complete(Check(hexagonCtrl));
        CheckBound(hexagonCtrl);
        if (IsEndGame()) EndGame();
    }

    public void Cancel()
    {
        if (selectedHex != null)
        {
            selectedHex.isSelected = false;
            selectedHex.spriteRenderer.color = Color.white;
            selectedHex.spriteRenderer.sortingOrder = 0;
            selectedHex.GetComponent<SphereCollider>().enabled = true;
            selectedHex = null;
        }
        spinButton.Hide();
        cancelButton.Hide();
    }

    void AddToStack(UndoStruct undoStruct)
    {
        if(undoStack.Count==0) undoButton.Reveal();

        undoStack.Add(undoStruct);
        if (undoStack.Count > 50)
        {
            undoStack.RemoveAt(0);
        }
    }

    public void Undo()
    {
        Debug.Log(undoStack.Count);
        if (undoStack.Count == 0) return;
        UndoStruct undoStruct = undoStack[undoStack.Count - 1];
        undoStack.RemoveAt(undoStack.Count - 1);
        if (undoStruct.isSpin)
        {
            ReverseSpin(hexagons[undoStruct.first.x, undoStruct.first.y]);
        }
        else
        {
            ChangePosition(hexagons[undoStruct.first.x, undoStruct.first.y], hexagons[undoStruct.second.x, undoStruct.second.y], true);
        }
        if (undoStack.Count == 0) undoButton.Hide();
    }

    public void Menu()
    {
        isMenu = true;
        menu.SetTrigger("On");
    }

    public void CloseMenu()
    {
        isMenu = false;
        menu.SetTrigger("Off");
    }

    public void Restart()
    {
        panel.SetActive(true);
        panel.GetComponent<Image>().color = Color.clear;
        ellipsePanel.SetActive(true);
        Invoke("RestartInvoker", 1.5f);
    }

    void RestartInvoker()
    {
        SceneManager.LoadScene(1);
    }

    public void ToMainMenu()
    {
        panel.SetActive(true);
        panel.GetComponent<Image>().color = Color.clear;
        ellipsePanel.SetActive(true);
        Invoke("ToMainMenuInvoker", 1.2f);
    }

    void ToMainMenuInvoker()
    {
        SceneManager.LoadScene(0);
    }

    bool IsEndGame()
    {
        int hexaCount = 2 + (level - 1);
        for (int i = 0; i < 1 + level * 2; i++)
        {
            for (int j = 0; j < hexaCount; j++)
            {
                if (!hexagons[i, j].isComplete) return false;
            }
            if (i < level) hexaCount++;
            else hexaCount--;
        }
        return true;
    }

    void EndGame()
    {
        isMenu = true;

        if (selectedHex != null)
        {
            selectedHex.isSelected = false;
            selectedHex.spriteRenderer.color = Color.white;
            selectedHex = null;
        }

        if (spinButton.gameObject.activeSelf) spinButton.Hide();
        if (cancelButton.gameObject.activeSelf) cancelButton.Hide();
        if (undoButton.gameObject.activeSelf) undoButton.Hide();
        if (menuButton.gameObject.activeSelf) menuButton.Hide();

        PlaystoreCtrl.SetAchivement();

        StartCoroutine(EndGameCoroutine());
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        int hexaCount = 2 + (level - 1);
        for (int i = 2*level; i >= 0; i--)
        {
            for (int j = 0; j < hexaCount; j++)
            {
                hexagons[i, j].Pop();
            }
            if (i > level) hexaCount++;
            else hexaCount--;
            yield return new WaitForSeconds(.8f);
        }
        ellipse.GetComponent<EllipseCtrl>().End();
    }
}



