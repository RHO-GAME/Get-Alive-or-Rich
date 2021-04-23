using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                switch (Random.Range(0, 7))
                {
                    case 0:
                        tmp = Instantiate(blue);
                        field[i][j].tag = "blue";
                        //Instantiate(blue).transform.parent = field[i][j].transform;
                        //field[i].Add(Instantiate(blue));
                        break;
                    case 1:
                        tmp = Instantiate(cyan);
                        field[i][j].tag = "cyan";
                        //Instantiate(cyan).transform.parent = field[i][j].transform;
                        //field[i].Add(Instantiate(cyan));
                        break;
                    case 2:
                        tmp = Instantiate(green);
                        field[i][j].tag = "green";
                        //Instantiate(green).transform.parent = field[i][j].transform;
                        //field[i].Add(Instantiate(green));
                        break;
                    case 3:
                        tmp = Instantiate(orange);
                        field[i][j].tag = "orange";
                        //Instantiate(orange).transform.parent = field[i][j].transform;
                        //field[i].Add(Instantiate(orange));
                        break;
                    case 4:
                        tmp = Instantiate(purple);
                        field[i][j].tag = "purple";
                        //Instantiate(purple).transform.parent = field[i][j].transform;
                        //field[i].Add(Instantiate(purple));
                        break;
                    case 5:
                        tmp = Instantiate(red);
                        field[i][j].tag = "red";
                        //Instantiate(red).transform.parent = field[i][j].transform;
                        //field[i].Add(Instantiate(red));
                        break;
                    case 6:
                        tmp = Instantiate(yellow);
                        field[i][j].tag = "yellow";
                        //Instantiate(yellow).transform.parent = field[i][j].transform;
                        //field[i].Add(Instantiate(yellow));
                        break;
                }
                tmp.transform.parent = field[i][j].transform;
                tmp.transform.position = field[i][j].transform.position;
            }
        }
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
}
