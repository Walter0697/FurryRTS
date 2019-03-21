using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public int boundary = 50;
    public int speed = 20;
    public float scrollSpeed = 0.2f;
    public int lowerScrollThres = 5;
    public int greaterScrollThres = 20;

    public bool checkWithBoundary = false;
    public bool smooth = true;
    public Vector2 moveDir;

    private int screenWidth;
    private int screenHeight;

	// Use this for initialization
	void Start () {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        moveDir = new Vector2(0, 0);

    }
	
	// Update is called once per frame
	void Update () {
        int scroll = 0;
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // closer
        {
            scroll += 1;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // further
        {
            scroll -= 1;
        }

        if(GetComponent<Camera>().orthographicSize - scroll * scrollSpeed > greaterScrollThres)
        {
            scroll = 0;
            GetComponent<Camera>().orthographicSize = greaterScrollThres;
        }
        else if (GetComponent<Camera>().orthographicSize - scroll * scrollSpeed < lowerScrollThres)
        {
            scroll = 0;
            GetComponent<Camera>().orthographicSize = lowerScrollThres;
        }


        moveDir = new Vector2(0, 0);

        if (checkWithBoundary)
            checkBoundary();

        if (Input.anyKey)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveDir.x -= 1;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveDir.x += 1;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveDir.y += 1;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                moveDir.y -= 1;
            }
        }
        GetComponent<Camera>().orthographicSize -= scroll * scrollSpeed;
        moveDir.Normalize();
        transform.position = new Vector3(transform.position.x + speed * Time.deltaTime * moveDir.x, transform.position.y, transform.position.z + speed * Time.deltaTime * moveDir.y);
    }

    private void checkBoundary()
    {
        if (Input.mousePosition.x > screenWidth - boundary)
        {
            moveDir.x += 1;
        }
        if (Input.mousePosition.x < 0 + boundary)
        {
            moveDir.x -= 1;
        }
        if (Input.mousePosition.y > screenHeight - boundary)
        {
            moveDir.y += 1;
        }
        if (Input.mousePosition.y < 0 + boundary)
        {
            moveDir.y -= 1;
        }
    }
}
