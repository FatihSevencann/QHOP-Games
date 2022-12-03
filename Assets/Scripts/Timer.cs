using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour
{

    public Text timerText;
    private float startTime;
    bool isMoving = false;
    bool kazandi = false;

    public float t = 10;

    void Start()
    {
        startTime = Time.time;

       
    }

    void Update()
    {
        isMoving = GameObject.FindWithTag("Player").GetComponent<PlayerController>().moving;
        string seconds = "";
        kazandi = GameObject.FindWithTag("Player").GetComponent<PlayerController>().kazandik;
        if (t > 0)
        {
            if (isMoving)

            {
                t = 10;
                t -= Time.deltaTime;
                seconds = t.ToString("f0");
            }
            //Debug.Log("hareket ettim");
            if (!isMoving && !kazandi)
            {

                t -= Time.deltaTime;
                seconds = t.ToString("f0");

            }

            else if(kazandi==true )
            {
                seconds = "0";

            }


        }
        else
        {
            seconds = "x";
            GameObject.Find("Player").GetComponent<PlayerController>().moveForward();
            t = 10;
        }
            
        timerText.text = seconds;
    }
}
