using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyMovement : MonoBehaviour
{
    public Keylocation[] keys;
    public float speed = 2f;
    private float counter;

    // Start is called before the first frame update
    void Start()
    {
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        counter += speed * Time.deltaTime;
        if (counter >= keys.Length - 1) counter = 0;
        Vector3 nextlocation = HermiteSpline(keys[(int)counter].transform.position, keys[(int)counter + 1].transform.position, keys[(int)counter].tangent, keys[(int)counter + 1].tangent, counter - (int)counter);
        transform.position = new Vector3(nextlocation.x, 0, nextlocation.z);
    }

    private Vector3 HermiteSpline(Vector3 p0, Vector3 p1, Vector3 m0, Vector3 m1, float t)
    {
        return (2 * t * t * t - 3 * t * t + 1) * p0 +
               (t * t * t - 2 * t * t + t) * m0 +
               (-2 * t * t * t + 3 * t * t) * p1 +
               (t * t * t - t * t) * m1;
    }
}
