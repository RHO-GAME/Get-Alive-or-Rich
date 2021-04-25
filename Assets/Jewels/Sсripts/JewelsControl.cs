using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

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
    
    private Vector2 fingerUp;
    private Vector2 fingerDown;
    private enum State { right = 0, left = 1, up = 2, down = 3 };
    private List<List<int>> queue;
    private int a;
    void Start()
    {
        field = new List<List<GameObject>>();
        GameObject tmp = null;
        for (int i = 0; i < 9; i++)
        {
            field.Add(new List<GameObject>());
            for (int j = 0; j < 9; j++)
            {
                field[i].Add(Instantiate(empty));
                field[i][j].transform.position = new Vector3(i, j, 0f);
                spawnNew(i, j);
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
        //findTriples();*/
    }

    void Update()
    {
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
        switch(state)
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

    void findTriples()
    {
        List<List<int>> forDestroy = new List<List<int>>();
        for (int i = 1; i < field.Count - 1; i++)
        {
           for (int j = 0; j < field[i].Count; j++)
            {
                if (field[i][j].tag == field[i + 1][j].tag && field[i][j].tag == field[i - 1][j].tag)
                {
                    forDestroy.Add(new List<int>() { i, j });
                    forDestroy.Add(new List<int>() { i + 1, j });
                    forDestroy.Add(new List<int>() { i - 1, j });
                }
            }
        }

        for (int i = 0; i < field.Count; i++)
        {
            for (int j = 1; j < field[i].Count - 1; j++)
            {
                if (field[i][j].tag == field[i][j + 1].tag && field[i][j].tag == field[i][j - 1].tag)
                {
                    forDestroy.Add(new List<int>() { i, j });
                    forDestroy.Add(new List<int>() { i, j + 1 });
                    forDestroy.Add(new List<int>() { i, j - 1 });
                }
            }
        }

        foreach (List<int> i in forDestroy)
        {
            if (field[i[0]][i[1]] != null)
            {
                Destroy(field[i[0]][i[1]]);
                field[i[0]][i[1]] = null;
            }
        }
    }

    public void spawnNew(int i1, int j1)
    {
        GameObject tmp = null;
        switch (Random.Range(0, 7))
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
}
