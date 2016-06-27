using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour {

    public static Queue<MoveMentMode> MoveMentQueue = new Queue<MoveMentMode>();


    public enum MoveMentMode {
        Up,
        Down,
        Left,
        Right,
        RightBorder,
        LeftBorder,
        UpBorder,
        DownBorder,
        Default

    }


    private bool _isBody = true;
    private bool _isTail = true;
    private Rigidbody2D _rigidbody;

	// Use this for initialization
	void Start () {
	    this._rigidbody = this.GetComponent<Rigidbody2D>();
	    this._rigidbody.gravityScale = 0;
	    this._rigidbody.isKinematic = true;
	}

    void SetNextActionMode(MoveMentMode mode) {
        lock (MoveMentQueue) {
            MoveMentQueue.Enqueue(mode);
        }

    }

    void NextAction() {
        MoveMentMode next = MoveMentMode.Default;
        lock (MoveMentQueue) {
            next = MoveMentQueue.Peek();
        }

        switch (next) {
                case MoveMentMode.Right: {
                    this._rigidbody.velocity = new Vector2(1f, 0.0f);
                    break;
                }
                case MoveMentMode.Left: {
                    this._rigidbody.velocity = new Vector2(-1f, 0.0f);
                    break;
                }
                case MoveMentMode.Up: {
                    this._rigidbody.velocity = new Vector2(0.0f, 1f);
                    break;
                }
                case MoveMentMode.Down: {
                    this._rigidbody.velocity = new Vector2(0.0f, -1f);
                    break;
                }
        }


        if (_isTail) {       
            lock (MoveMentQueue) {
                MoveMentQueue.Dequeue();
            }
        }




    }

    void AddBody() {
        _isTail = false;
    }
	
	// Update is called once per frame
	void Update () {
//	    if (_isBody) {
//	        return;
//	    }

        Camera mainCam = Camera.main;
	    Vector3 thisPosition = this.transform.position;
	    Vector3 viewPortPostion = mainCam.WorldToViewportPoint(thisPosition);
        Debug.Log(viewPortPostion);
	   // Vector3 position = this.transform.position;
	    if (viewPortPostion.x >= 1) {
	        //reach right border of camera
	        SetNextActionMode(MoveMentMode.LeftBorder);
            return;
	    } else if (viewPortPostion.x <= 0) {
	        SetNextActionMode(MoveMentMode.RightBorder);
            return;
	    } else if (viewPortPostion.y <= 0) {
	        SetNextActionMode(MoveMentMode.UpBorder);
            return;
	    } else if (viewPortPostion.y >= 1) {
	        SetNextActionMode(MoveMentMode.DownBorder);
            return;
	    }

	    if (Input.GetKey("up")) {
	        SetNextActionMode(MoveMentMode.Up);
	    } else if (Input.GetKey("down")) {
	        SetNextActionMode(MoveMentMode.Down);
	    } else if (Input.GetKey("left")) {
	        SetNextActionMode(MoveMentMode.Left);
	    } else if (Input.GetKey("right")) {
	        SetNextActionMode(MoveMentMode.Right);
	    }

        NextAction();
	    // Debug.Log(position);
	}
}
