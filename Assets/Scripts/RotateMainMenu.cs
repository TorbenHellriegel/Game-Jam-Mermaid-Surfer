using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMainMenu : MonoBehaviour
{
    private float hight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > 0)
        {
            transform.Rotate(new Vector3(0, -1, 0));
        }
        else
        {
            transform.Rotate(new Vector3(0, 1, 0));
        }

        hight += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, Mathf.Sin(hight)-2, transform.position.z);
    }
}
