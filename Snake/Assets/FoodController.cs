﻿using UnityEngine;
using System.Collections;

public class FoodController : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log(collision);
    }
}
