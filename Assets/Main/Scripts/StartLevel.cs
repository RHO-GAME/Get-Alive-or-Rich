using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour
{
    private bool start = false;
    public Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        if (!start)
        {
            GetComponentInChildren<Animator>().Play("layer.CameraMotion", 0, 0f);
            start = true;
//            Instantiate(canvas);

        }

    }
    public void CreateCanvas()
    {
        Canvas canv = Instantiate(canvas);
        canv.GetComponentInChildren<Animator>().Play("layer.ButtonAppearance", 0, 0f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
