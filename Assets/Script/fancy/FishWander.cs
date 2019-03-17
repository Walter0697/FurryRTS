using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishWander : MonoBehaviour
{
    public SphereCollider collider;
    public float maxAngleChange = 5;
    public float changingAngle = 1;
    public float speed = 0.8f;

    private float angle = 180;
    private float counter = 0;

    private float lookAtangle = 180;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        angle = Random.Range(20, 340);
        lookAtangle = angle;
    }

    // Update is called once per frame
    void Update()
    {
        //getting the lookat angle smoothly
        Vector3 original = transform.position;
        if (lookAtangle - angle >= 0.5f) lookAtangle -= 0.5f * Time.deltaTime;
        else if (angle - lookAtangle >= 0.5f)lookAtangle += 0.5f * Time.deltaTime;
        
        //counting down to change angle
        counter += Time.deltaTime;
        if (counter >= changingAngle)
        {
            angle = angle + Random.Range(-maxAngleChange, maxAngleChange) * Time.deltaTime;
            counter = 0;
        }
        Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        transform.position += direction * Time.deltaTime * Random.Range(speed / 2, speed);

        //checking which way to look at
        Vector3 lookAtDirection = new Vector3(Mathf.Cos(lookAtangle), 0, Mathf.Sin(lookAtangle));
        lookAtDirection += transform.position;
        Quaternion lookat = Quaternion.LookRotation(lookAtDirection - original, Vector3.up);
        transform.rotation = lookat;
    }

    void OnTriggerExit(Collider other)
    {
        SphereCollider sphere = other.gameObject.GetComponent<SphereCollider>();
        if (sphere == collider)
        {
            Vector3 direction = startPos - transform.position;
            angle = Mathf.Atan2(direction.z, direction.x);
            lookAtangle = angle;
        }
    }
}
