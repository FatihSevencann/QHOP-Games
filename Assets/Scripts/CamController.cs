using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public GameObject block;
    private Vector3 dist;

    void Start()
    {
        dist = transform.position - block.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(block.transform.position.x + dist.x,transform.position.y, block.transform.position.z + dist.z);
    }
}
