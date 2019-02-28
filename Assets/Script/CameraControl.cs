using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public int boundary = 50;
    public int speed = 20;
    public bool smooth = true;

    private int screenWidth;
    private int screenHeight;

	// Use this for initialization
	void Start () {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
	}
	
	// Update is called once per frame
	void Update () {
        checkBoundary();
	}

    private void checkBoundary()
    {
        float ratio;
        if (Input.mousePosition.x > screenWidth - boundary)
        {
            ratio = 1;
            if (smooth)
                ratio = 1 - ((screenWidth - Input.mousePosition.x) / boundary);
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime * ratio, transform.position.y, transform.position.z);
        }
        if (Input.mousePosition.x < 0 + boundary)
        {
            ratio = 1;
            if (smooth)
                ratio = 1 - (Input.mousePosition.x / boundary);
            transform.position = new Vector3(transform.position.x - speed * Time.deltaTime * ratio, transform.position.y, transform.position.z);
        }
        if (Input.mousePosition.y > screenHeight - boundary)
        {
            ratio = 1;
            if (smooth)
                ratio = 1 - ((screenHeight - Input.mousePosition.y) / boundary);
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed * Time.deltaTime * ratio);
        }
        if (Input.mousePosition.y < 0 + boundary)
        {
            ratio = 1;
            if (smooth)
                ratio = 1 - (Input.mousePosition.y / boundary);
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - speed * Time.deltaTime * ratio);
        }
    }
}
