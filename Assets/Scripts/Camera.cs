using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject controller;
    void Update()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Vector3 white = new Vector3(0, 0, 0);
        Vector3 black = new Vector3(0, 0, 180);
        if (controller.GetComponent<Game>().GetCurrentPlayer() == "white")
            transform.eulerAngles = white;
        else
            transform.eulerAngles = black;
    }
}
