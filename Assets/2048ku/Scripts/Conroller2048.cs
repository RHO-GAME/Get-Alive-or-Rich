using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conroller2048 : MonoBehaviour
{
    public GameObject two;
    public GameObject four;
    public GameObject eight;
    public GameObject sixteen;
    public GameObject thirty2;
    public GameObject sixty4;
    public GameObject one28;
    public GameObject two56;
    private GameObject[,] field;
    private int xSize = 8;
    private int ySize = 8;
    private Pattern[] potentialPatterns;
    private Vector3 fingerDown;
    private Pattern movable = null;

    // Start is called before the first frame update
    void Start()
    {
        field = new GameObject[xSize, ySize];
        for (int i = 0; i < xSize; i++)
            for (int j = 0; j < ySize; j++)
                field[i, j] = null;

        potentialPatterns = new Pattern[3];
        potentialPatterns[0] = new Angle(new Vector3(0f, -1.2f, 0f), Instantiate(two), Instantiate(two), Instantiate(two));
        potentialPatterns[1] = new Square(new Vector3(3f, -1.2f, 0f), Instantiate(two), Instantiate(two), Instantiate(two), Instantiate(two));
        potentialPatterns[2] = new VerticalTwo(new Vector3(6f, -1.2f, 0f), Instantiate(two), Instantiate(two));
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began && movable == null)
            {
                fingerDown = Camera.main.ScreenToWorldPoint(touch.position);
                Debug.Log("First touch" + fingerDown.x.ToString() + "  " + fingerDown.y);
                foreach (Pattern i in potentialPatterns)
                {
                    bool flag = false;
                    List<Vector3> tmp = i.givePosition();
                    for (int j = 0; j <tmp.Count; j++)
                    {
                        if (Mathf.Abs(tmp[j].x - fingerDown.x) < 0.5f && Mathf.Abs(tmp[j].y - fingerDown.y) < 0.5f)
                            flag = true;
                    }
                    if (flag)
                        movable = i;
                }
            }
            else if (touch.phase == TouchPhase.Moved && movable != null)
            {
                Debug.Log("Moved touch" + fingerDown.x.ToString() + "  " + fingerDown.y);
                movable.move(Camera.main.ScreenToWorldPoint(touch.position) - fingerDown);
                fingerDown = Camera.main.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                movable = null;
            }
            //ended check where to put
        }
    }
}

public interface Pattern
{
    public void move(Vector3 position);
    public List<Vector3> givePosition();
}

public class Angle: Pattern
{
    private GameObject tile1;
    private GameObject tile2;
    private GameObject tile3;

    public Angle(Vector3 position, GameObject tile1, GameObject tile2, GameObject tile3)
    {
        this.tile1 = tile1;
        this.tile2 = tile2;
        this.tile3 = tile3;
        this.tile1.transform.position = position;
        this.tile2.transform.position = new Vector3( position.x + 1, position.y, position.z);
        this.tile3.transform.position = new Vector3(position.x, position.y - 1, position.z);
    }

    public void move(Vector3 position)
    {
        tile1.transform.position += position;
        tile2.transform.position += position;
        tile3.transform.position += position;
    }

    public List<Vector3> givePosition() =>  new List<Vector3>() { tile1.transform.position, tile2.transform.position, tile3.transform.position };
}

public class Square : Pattern
{
    private GameObject tile1;
    private GameObject tile2;
    private GameObject tile3;
    private GameObject tile4;

    public Square(Vector3 position, GameObject tile1, GameObject tile2, GameObject tile3, GameObject tile4)
    {
        this.tile1 = tile1;
        this.tile2 = tile2;
        this.tile3 = tile3;
        this.tile4 = tile4;
        this.tile1.transform.position = position;
        this.tile2.transform.position = new Vector3(position.x + 1, position.y, position.z);
        this.tile3.transform.position = new Vector3(position.x, position.y - 1, position.z);
        this.tile4.transform.position = new Vector3(position.x + 1, position.y - 1, position.z);
    }

    public void move(Vector3 position)
    {
        tile1.transform.position += position;
        tile2.transform.position += position;
        tile3.transform.position += position;
        tile4.transform.position += position;
    }

    public List<Vector3> givePosition() => new List<Vector3>() { tile1.transform.position, tile2.transform.position, tile3.transform.position, tile4.transform.position };
}

public class VerticalTwo : Pattern
{
    private GameObject tile1;
    private GameObject tile2;

    public VerticalTwo(Vector3 position, GameObject tile1, GameObject tile2)
    {
        this.tile1 = tile1;
        this.tile2 = tile2;
        this.tile1.transform.position = position;
        this.tile2.transform.position = new Vector3(position.x, position.y - 1, position.z);
    }

    public void move(Vector3 position)
    {
        tile1.transform.position += position;
        tile2.transform.position += position;
    }

    public List<Vector3> givePosition() => new List<Vector3>() { tile1.transform.position, tile2.transform.position };
}