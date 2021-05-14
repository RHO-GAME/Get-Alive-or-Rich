using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class JewelsControl : MonoBehaviour
{
    public GameObject blue;
    public GameObject cyan;
    public GameObject green;
    public GameObject orange;
    public GameObject purple;
    public GameObject red;
    public GameObject yellow;
    public GameObject empty;
    private List<List<GameObject>> field;
    public bool level1;
    public bool level2;
    public bool level3;
    public bool level4;
    public bool level5;
    public bool level6;
    private int counter;
    public Text victory;
    private Vector2 fingerUp;
    private Vector2 fingerDown;

    private enum State { right = 0, left = 1, up = 2, down = 3 };
    private List<List<int>> queue;
    private int a;
    void Start()
    {
        if (level1)
        {
            counter = 0;
            field = new List<List<GameObject>>();
            GameObject tmp = null;
            for (int i = 0; i < 9; i++)
            {
                field.Add(new List<GameObject>());
                for (int j = 0; j < 9; j++)
                {
                    /*field[i].Add(Instantiate(empty));
                    field[i][j].transform.position = new Vector3(i, j, 0f);*/
                    field[i].Add(null);
                    //spawnNew(i, j);
                }
            }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    spawnNew(i, j);
                }
            }
        }
        else if (level2)
        {
            counter = 0;
            field = new List<List<GameObject>>();
            GameObject tmp = null;
            for (int i = 0; i < 9; i++)
            {
                field.Add(new List<GameObject>());
                for (int j = 0; j < 9; j++)
                {
                    field[i].Add(null);
                }
            }

            #region заглушки
            field[0][0] = Instantiate(empty);
            field[0][0].tag = "nothing";

            field[1][0] = Instantiate(empty);
            field[1][0].tag = "nothing";

            field[2][0] = Instantiate(empty);
            field[2][0].tag = "nothing";

            field[0][1] = Instantiate(empty);
            field[0][1].tag = "nothing";

            field[0][2] = Instantiate(empty);
            field[0][2].tag = "nothing";

            field[1][1] = Instantiate(empty);
            field[1][1].tag = "nothing";

            field[8][8] = Instantiate(empty);
            field[8][8].tag = "nothing";

            field[8][7] = Instantiate(empty);
            field[8][7].tag = "nothing";

            field[8][6] = Instantiate(empty);
            field[8][6].tag = "nothing";

            field[7][8] = Instantiate(empty);
            field[7][8].tag = "nothing";

            field[6][8] = Instantiate(empty);
            field[6][8].tag = "nothing";

            field[7][7] = Instantiate(empty);
            field[7][7].tag = "nothing";

            field[0][8] = Instantiate(empty);
            field[0][8].tag = "nothing";

            field[0][7] = Instantiate(empty);
            field[0][7].tag = "nothing";

            field[0][6] = Instantiate(empty);
            field[0][6].tag = "nothing";

            field[1][8] = Instantiate(empty);
            field[1][8].tag = "nothing";

            field[2][8] = Instantiate(empty);
            field[2][8].tag = "nothing";

            field[1][7] = Instantiate(empty);
            field[1][7].tag = "nothing";

            field[8][0] = Instantiate(empty);
            field[8][0].tag = "nothing";

            field[7][0] = Instantiate(empty);
            field[7][0].tag = "nothing";

            field[6][0] = Instantiate(empty);
            field[6][0].tag = "nothing";

            field[8][1] = Instantiate(empty);
            field[8][1].tag = "nothing";

            field[8][2] = Instantiate(empty);
            field[8][2].tag = "nothing";

            field[7][1] = Instantiate(empty);
            field[7][1].tag = "nothing";
            #endregion

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (field[i][j] == null)
                        spawnNew(i, j);
                }
            }
        }
        else if (level6)
        {
            counter = 0;
            field = new List<List<GameObject>>();
            GameObject tmp = null;
            for (int i = 0; i < 9; i++)
            {
                field.Add(new List<GameObject>());
                for (int j = 0; j < 9; j++)
                {
                    /*field[i].Add(Instantiate(empty));
                    field[i][j].transform.position = new Vector3(i, j, 0f);*/
                    field[i].Add(null);
                    //spawnNew(i, j);
                }
            }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if ((i == 1 && j == 1) || (i == 4 && j == 4) || (i == 7 && j == 1) || (i == 1 && j == 7) || (j == 7 && i == 7))
                    {
                        field[i][j] = Instantiate(empty);
                        field[i][j].tag = "nothing";
                    }    
                    else
                    {
                        spawnNew(i, j);
                    }
                }
            }
        }
        /*queue = new List<List<int>>();
        int i1 = -1;
        int j1 = 0;
        int sign = 1;
        int length = 9;
        int count;
        while (i1 != 4 && j1 != 4)
        {
            count = 0;
            i1 += sign;
            while (count < length)
            {
                spawnNew(i1, j1);
                queue.Add(new List<int>() { i1, j1 });
                i1 += sign;
                count++;
            }
            count = 0;
            length--;
            i1 -= sign;
            j1 += sign;
            while (count < length)
            {
                spawnNew(i1, j1);
                queue.Add(new List<int>() { i1, j1 });
                j1 += sign;
                count++;
            }
            j1 -= sign;
            sign *= -1;
        }
        spawnNew(4, 4);
        queue.Add(new List<int>() { 4, 4 });*/
        while (findTriples()) { }
    }

    void Update()
    {//в свапах надо, наверное, к нуллам еще добавить и нофинг
        if (counter >= 27)
        {
            Debug.Log("Victory");
            //victory.text = "You win!!! \nYour score: " + counter.ToString();
            Preferences prefs = new Preferences(counter);
            if (level1)
                prefs.level = 1;
            else if (level2)
                prefs.level = 2;
            else if (level3)
                prefs.level = 3;
            else if (level4)
                prefs.level = 4;
            else if (level5)
                prefs.level = 5;
            else if (level6)
                prefs.level = 6;
            PlayerPrefs.SetInt("level", prefs.level);
            PlayerPrefs.SetInt("counter" + prefs.level.ToString(), prefs.counter);
            if (prefs.level == 1)
                SceneManager.LoadScene(3);
            else
                SceneManager.LoadScene(4);
            //надо подумать про файлы на сцене с колодцем
            //bool[] levels_done = {true, true, false
            //int levels_done = 2
        }
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerDown = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                fingerUp = touch.position;

                Debug.Log(touch.position.x + " " + touch.position.y);
                Vector2 pos = Camera.main.ScreenToWorldPoint(fingerDown);
                for (int i = 0; i < field.Count; i++)
                {
                    for (int j = 0; j < field.Count; j++)
                    {
                        if (Mathf.Abs(i - pos.x) <= 0.5f && Mathf.Abs(j - pos.y) <= 0.5f)
                            swapObjects(i, j, getSwipeState(fingerDown, fingerUp));
                    }
                }
            }
        }
    }

    void swapObjects(int i, int j, State state)
    {
        Vector3 pos = field[i][j].transform.position;
        GameObject obj = field[i][j];
        switch (state)
        {
            case State.up:
                field[i][j].GetComponentInChildren<Animator>().Play("layer.Up", 0, 0f);
                field[i][j + 1].GetComponentInChildren<Animator>().Play("layer.Down", 0, 0f);
                field[i][j].transform.position = field[i][j + 1].transform.position;
                field[i][j + 1].transform.position = pos;
                field[i][j] = field[i][j + 1];
                field[i][j + 1] = obj;
                break;
            case State.down:
                field[i][j].GetComponentInChildren<Animator>().Play("layer.Down", 0, 0f);
                field[i][j - 1].GetComponentInChildren<Animator>().Play("layer.Up", 0, 0f);
                field[i][j].transform.position = field[i][j - 1].transform.position;
                field[i][j - 1].transform.position = pos;
                field[i][j] = field[i][j - 1];
                field[i][j - 1] = obj;
                break;
            case State.right:
                field[i][j].GetComponentInChildren<Animator>().Play("layer.Right", 0, 0f);
                field[i + 1][j].GetComponentInChildren<Animator>().Play("layer.Left", 0, 0f);
                field[i][j].transform.position = field[i + 1][j].transform.position;
                field[i + 1][j].transform.position = pos;
                field[i][j] = field[i + 1][j];
                field[i + 1][j] = obj;
                break;
            case State.left:
                field[i][j].GetComponentInChildren<Animator>().Play("layer.Left", 0, 0f);
                field[i - 1][j].GetComponentInChildren<Animator>().Play("layer.Right", 0, 0f);
                field[i][j].transform.position = field[i - 1][j].transform.position;
                field[i - 1][j].transform.position = pos;
                field[i][j] = field[i - 1][j];
                field[i - 1][j] = obj;
                break;
        }
        if (findTriples())
            while (findTriples()) {  }
        else
        {
            StartCoroutine(moveBack(state, i, j));
        }
    }

    State getSwipeState(Vector2 fingerDown, Vector2 fingerUp)
    {
        if (Mathf.Abs(fingerDown.x - fingerUp.x) > Mathf.Abs(fingerDown.y - fingerUp.y))
        {
            if ((fingerDown.x - fingerUp.x) > 0)
            {
                Debug.Log("left");
                return State.left;
            }
            else
            {
                Debug.Log("Right");
                return State.right;
            }
        }
        else
        {
            if ((fingerDown.y - fingerUp.y) > 0)
            {
                Debug.Log("down");
                return State.down;
            }
            else
            {
                Debug.Log("up");
                return State.up;
            }
        }
    }

    bool findTriples()
    {
        bool flag = false;
        List<List<int>> forDestroy = new List<List<int>>();
        for (int i = 1; i < field.Count - 1; i++)
        {
            for (int j = 0; j < field[i].Count; j++)
            {
                if (field[i][j] != null && field[i + 1][j] != null && field[i - 1][j] != null)
                    if (field[i][j].tag == field[i + 1][j].tag && field[i][j].tag == field[i - 1][j].tag)
                    {
                        forDestroy.Add(new List<int>() { i, j });
                        forDestroy.Add(new List<int>() { i + 1, j });
                        forDestroy.Add(new List<int>() { i - 1, j });
                        flag = true;
                    }
            }
        }

        for (int i = 0; i < field.Count; i++)
        {
            for (int j = 1; j < field[i].Count - 1; j++)
            {
                if (field[i][j] != null && field[i][j + 1] != null && field[i][j - 1] != null)
                    if (field[i][j].tag == field[i][j + 1].tag && field[i][j].tag == field[i][j - 1].tag)
                    {
                        forDestroy.Add(new List<int>() { i, j });
                        forDestroy.Add(new List<int>() { i, j + 1 });
                        forDestroy.Add(new List<int>() { i, j - 1 });
                        flag = true;
                    }
            }
        }
        if (forDestroy.Count > 0)
            foreach (List<int> i in forDestroy)
            {
                if (field[i[0]][i[1]] != null)
                {
                    //Destroy(field[i[0]][i[1]]);
                    StartCoroutine(slowDestroy(field[i[0]][i[1]]));
                    counter += 1;
                    field[i[0]][i[1]] = null;
                }
            }
        if (!flag)
        {
            for (int i = 0; i < field.Count; i++)
                for (int j = 0; j < field[0].Count; j++)
                {
                    if (field[i][j] == null)
                    {
                        //fall on empty place
                        int tmp = j + 1;
                        if (tmp == field[0].Count)
                            tmp--;
                        while ((field[i][tmp] == null || field[i][tmp].tag == "nothing") && tmp < field[0].Count - 1)
                            tmp++;
                        if (tmp == field[0].Count - 1)
                        {
                            if (field[i][tmp] != null && field[i][tmp].tag != "nothing")
                            {
                                field[i][j] = field[i][tmp];
                                //field[i][j].transform.position = new Vector3(i, j, 0f);
                                StartCoroutine(moveDown(field[i][j], i, j, tmp));
                                field[i][tmp] = null;
                                //field[i][j].GetComponentInChildren<Animator>().Play("layer.MoveDown" + (tmp - j), 0, 0f);
                            }
                            else
                            {
                                spawnNew(i, j);
                                field[i][j].GetComponentInChildren<Animator>().Play("layer.None" , 0, 0f);
                                StartCoroutine(slowAppear(field[i][j]));
                            }
                        }
                        else
                        {
                            field[i][j] = field[i][tmp];
                            //field[i][j].transform.position = new Vector3(i, j, 0f);
                            StartCoroutine(moveDown(field[i][j], i, j, tmp));
                            field[i][tmp] = null;
                            //field[i][j].GetComponentInChildren<Animator>().Play("layer.MoveDown" + (tmp - j), 0, 0f);
                        }
                        flag = true;
                    }
                }
            if (flag)
                findTriples();
        }
        return flag;
    }

    public void spawnNew(int i1, int j1)
    {
        GameObject tmp = null;
        field[i1][j1] = Instantiate(empty);
        field[i1][j1].transform.position = new Vector3(i1, j1, 0f);
        switch (UnityEngine.Random.Range(0, 7))
        {
            case 0:
                tmp = Instantiate(blue);
                field[i1][j1].tag = "blue";
                break;
            case 1:
                tmp = Instantiate(cyan);
                field[i1][j1].tag = "cyan";
                break;
            case 2:
                tmp = Instantiate(green);
                field[i1][j1].tag = "green";
                break;
            case 3:
                tmp = Instantiate(orange);
                field[i1][j1].tag = "orange";
                break;
            case 4:
                tmp = Instantiate(purple);
                field[i1][j1].tag = "purple";
                break;
            case 5:
                tmp = Instantiate(red);
                field[i1][j1].tag = "red";
                break;
            case 6:
                tmp = Instantiate(yellow);
                field[i1][j1].tag = "yellow";
                break;
        }
        tmp.transform.parent = field[i1][j1].transform;
        tmp.transform.position = field[i1][j1].transform.position;
        tmp.GetComponentInChildren<Animator>().Play("layer.Spawn", 0, 0f);
        //findTriples();
    }

    void startAppearing()
    {
        a = 0;
        field[0][0].GetComponentInChildren<Animator>().Play("layer.Spawn", 0, 0f);
    }

    public void appearNext()
    {
        a++;
        field[queue[a][0]][queue[a][1]].GetComponentInChildren<Animator>().Play("layer.Spawn", 0, 0f);
    }

    IEnumerator moveBack(State state, int i, int j)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 pos = field[i][j].transform.position;
        GameObject obj = field[i][j];
        switch (state)
        {
            case State.up:
                field[i][j].GetComponentInChildren<Animator>().Play("layer.Up", 0, 0f);
                field[i][j + 1].GetComponentInChildren<Animator>().Play("layer.Down", 0, 0f);
                field[i][j].transform.position = field[i][j + 1].transform.position;
                field[i][j + 1].transform.position = pos;
                field[i][j] = field[i][j + 1];
                field[i][j + 1] = obj;
                break;
            case State.down:
                field[i][j].GetComponentInChildren<Animator>().Play("layer.Down", 0, 0f);
                field[i][j - 1].GetComponentInChildren<Animator>().Play("layer.Up", 0, 0f);
                field[i][j].transform.position = field[i][j - 1].transform.position;
                field[i][j - 1].transform.position = pos;
                field[i][j] = field[i][j - 1];
                field[i][j - 1] = obj;
                break;
            case State.right:
                field[i][j].GetComponentInChildren<Animator>().Play("layer.Right", 0, 0f);
                field[i + 1][j].GetComponentInChildren<Animator>().Play("layer.Left", 0, 0f);
                field[i][j].transform.position = field[i + 1][j].transform.position;
                field[i + 1][j].transform.position = pos;
                field[i][j] = field[i + 1][j];
                field[i + 1][j] = obj;
                break;
            case State.left:
                field[i][j].GetComponentInChildren<Animator>().Play("layer.Left", 0, 0f);
                field[i - 1][j].GetComponentInChildren<Animator>().Play("layer.Right", 0, 0f);
                field[i][j].transform.position = field[i - 1][j].transform.position;
                field[i - 1][j].transform.position = pos;
                field[i][j] = field[i - 1][j];
                field[i - 1][j] = obj;
                break;
        }
    }

    IEnumerator moveDown(GameObject moveable, int i, int j, int tmp)
    {
        yield return new WaitForSeconds(0.7f);
        moveable.transform.position = new Vector3(i, j, 0f);
        moveable.GetComponentInChildren<Animator>().Play("layer.MoveDown" + (tmp - j), 0, 0f);
    }

    IEnumerator slowAppear(GameObject tmp)
    {
        yield return new WaitForSeconds(0.7f);
        tmp.GetComponentInChildren<Animator>().Play("layer.Spawn", 0, 0f);
    }

    IEnumerator slowDestroy(GameObject tmp)
    {
        yield return new WaitForSeconds(0.7f);
        tmp.GetComponentInChildren<Animator>().Play("layer.Destroy", 0, 0f);
        yield return new WaitForSeconds(0.5f);
        Destroy(tmp);
    }

}

[Serializable]
public class Preferences
{
    public int counter;
    public int level;

    public Preferences(int counter)
    {
        this.counter = counter;
    }
}