using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour {

    public static Queue<MoveMentMode> MovementQueue = new Queue<MoveMentMode>();
    public static Camera MainCam = Camera.main;
    public static float SpeedMutiplier = 10f;

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
    private MoveMentMode _lastMode;

	// Use this for initialization
	void Start () {
	    this._rigidbody = this.GetComponent<Rigidbody2D>();
	    this._rigidbody.gravityScale = 0;
	    this._rigidbody.isKinematic = true;
	}

    void SetNextActionMode(MoveMentMode mode) {
        lock (MovementQueue) {
            MovementQueue.Enqueue(mode);
        }
        _lastMode = mode;

    }

    void NextAction() {
        MoveMentMode next = MoveMentMode.Default;
        lock (MovementQueue) {
            if(MovementQueue.Count <= 0) return;
            next = MovementQueue.Peek();
        }
        Vector3 thisPosition = this.transform.position;
        

        switch (next) {

                case MoveMentMode.Right: {
                    this._rigidbody.velocity = new Vector2(1f * SpeedMutiplier, 0.0f);
                    break;
                }
                case MoveMentMode.Left: {
                    this._rigidbody.velocity = new Vector2(-1f * SpeedMutiplier, 0.0f);
                    break;
                }
                case MoveMentMode.Up: {
                    this._rigidbody.velocity = new Vector2(0.0f, 1f * SpeedMutiplier);
                    break;
                }
                case MoveMentMode.Down: {
                    this._rigidbody.velocity = new Vector2(0.0f, -1f * SpeedMutiplier);
                    break;
                }
                

                case MoveMentMode.RightBorder: {
                    this.transform.position = new Vector3(MainCam.ViewportToWorldPoint(new Vector3(1f,0f,0f)).x , thisPosition.y, 0.0f );
                    break;
                }
                case MoveMentMode.LeftBorder: {
                    this.transform.position = new Vector3(MainCam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x, thisPosition.y, 0.0f);
                    break;
                }
                case MoveMentMode.UpBorder: {
                    this.transform.position = new Vector3(thisPosition.x,(MainCam.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y));
                    break;
                }
                case MoveMentMode.DownBorder: {
                    this.transform.position = new Vector3(thisPosition.x, (MainCam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y));
                    break;
                }
                
        }


        if (_isTail) {       
            lock (MovementQueue) {
                MovementQueue.Dequeue();
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

	    Vector3 thisPosition = this.transform.position;
	    Vector3 viewPortPostion = MainCam.WorldToViewportPoint(thisPosition);
        //Debug.Log(viewPortPostion);
	   // Vector3 position = this.transform.position;
	   // if (_lastMode != MoveMentMode.DownBorder && _lastMode != MoveMentMode.RightBorder && _lastMode != MoveMentMode.LeftBorder && _lastMode != MoveMentMode.UpBorder) {
	        if (viewPortPostion.x >= 1) {
	            //reach right border of camera
	            SetNextActionMode(MoveMentMode.LeftBorder);
	        }
	        else if (viewPortPostion.x <= 0) {
	            SetNextActionMode(MoveMentMode.RightBorder);
	        }
	        else if (viewPortPostion.y <= 0) {
	            SetNextActionMode(MoveMentMode.UpBorder);
	        }
	        else if (viewPortPostion.y >= 1) {
	            SetNextActionMode(MoveMentMode.DownBorder);
	        }
	    //}

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
