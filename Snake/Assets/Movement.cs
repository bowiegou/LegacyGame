﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour {

    //public static Queue<MovementQueueEntry> MovementQueue = new Queue<MovementQueueEntry>();
    public static MovementQueueEntry[] MovementBuffer = new MovementQueueEntry[1024];
    public static int NextActionAddIndex = 0;
	public static Camera MainCam;
    public static float SpeedMutiplier = 10f;
	//public static bool abandonNextClear;

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


    private bool _isBody = false;
    private bool _isTail = true;
    private Rigidbody2D _rigidbody;
    public MoveMentMode LastMode;
    private Movement _nextBody;
    private int nextActionIndex = 0;
	private bool addBodyToEnd = false;

    // Use this for initialization
    void Start() {
        this._rigidbody = this.GetComponent<Rigidbody2D>();
        this._rigidbody.gravityScale = 0;
        this._rigidbody.isKinematic = true;
		MainCam = Camera.main;
    }

    void SetNextActionMode(MoveMentMode mode) {
        //Debug.Log("--------" + mode);
        if(NextActionAddIndex == 0) ResetBufferIndex();
        MovementBuffer[NextActionAddIndex++] = new MovementQueueEntry(mode);
    }

    public void ResetBufferIndex() {
		lock (MovementBuffer) {
			this.nextActionIndex = 0;
			if (!_isTail)
				this._nextBody.ResetBufferIndex ();
		}
    }

    public void InitBody(int actionIndex) {
        _isBody = true;
        this.nextActionIndex = actionIndex;
    }
		

    public void NextAction() {

		if (MovementBuffer [nextActionIndex] == null) {
			if (_isTail && !addBodyToEnd) {
				NextActionAddIndex = 0;
			}
            
		} else {
        
			MovementQueueEntry entry = MovementBuffer [nextActionIndex];
			var next = entry.Mode;
			Vector3? transPosition = entry.Position;
			Vector3 thisPosition = this.transform.position;

			if (transPosition == null) {
				transPosition = thisPosition;
				entry.Position = thisPosition;
			} 

			if (thisPosition == transPosition.Value) {
				this.transform.position = transPosition.Value;
				nextActionIndex++;

				switch (next) {

				case MoveMentMode.Right:
					{
						this._rigidbody.velocity = new Vector2 (1f * SpeedMutiplier, 0.0f);
						LastMode = next;
						break;
					}
				case MoveMentMode.Left:
					{
						this._rigidbody.velocity = new Vector2 (-1f * SpeedMutiplier, 0.0f);
						LastMode = next;
						break;
					}
				case MoveMentMode.Up:
					{
						this._rigidbody.velocity = new Vector2 (0.0f, 1f * SpeedMutiplier);
						LastMode = next;
						break;
					}
				case MoveMentMode.Down:
					{
						this._rigidbody.velocity = new Vector2 (0.0f, -1f * SpeedMutiplier);
						LastMode = next;
						break;
					}


				case MoveMentMode.RightBorder:
					{
						this.transform.position = new Vector3 (MainCam.ViewportToWorldPoint (new Vector3 (1f, 0f, 0f)).x, thisPosition.y, 0.0f);
						break;
					}
				case MoveMentMode.LeftBorder:
					{
						this.transform.position = new Vector3 (MainCam.ViewportToWorldPoint (new Vector3 (0f, 0f, 0f)).x, thisPosition.y, 0.0f);
						break;
					}
				case MoveMentMode.UpBorder:
					{
						this.transform.position = new Vector3 (thisPosition.x, (MainCam.ViewportToWorldPoint (new Vector3 (0f, 1f, 0f)).y));
						break;
					}
				case MoveMentMode.DownBorder:
					{
						this.transform.position = new Vector3 (thisPosition.x, (MainCam.ViewportToWorldPoint (new Vector3 (0f, 0f, 0f)).y));
						break;
					}

				}
			} else if (_isTail) {
				//Debug.Log (MovementBuffer [nextActionIndex].Position + "" + thisPosition + this.gameObject.ToString ());
			}
		}

		if (addBodyToEnd) {
			lock (MovementBuffer) {
				//safety measure
				Time.timeScale = 0f;
				AddBodyProcess ();
				addBodyToEnd = false;
				Time.timeScale = 1f;
			}
		}

        


    }
		

	void AddBodyProcess() {

		//Debug.Log ("AddBody");
		_isTail = false;

		Vector3 positionToPlace = this.transform.position;

		switch (LastMode) {
		case MoveMentMode.Right: {
				positionToPlace.x--;
				break;
			}
		case MoveMentMode.Left: {
				positionToPlace.x++;
				break;
			}
		case MoveMentMode.Up: {
				positionToPlace.y--;
				break;
			}
		case MoveMentMode.Down: {
				positionToPlace.y++;
				break;
			}
		}

		Movement nextBody = ((GameObject)Instantiate(this.gameObject, positionToPlace, Quaternion.identity)).GetComponent<Movement>();
		nextBody.gameObject.GetComponent<Rigidbody2D>().velocity = this._rigidbody.velocity;
		nextBody.gameObject.name = "Body";
		nextBody.LastMode = this.LastMode;
		nextBody.transform.parent = this.transform.parent;
		nextBody.InitBody(this.nextActionIndex);
		_nextBody = nextBody;
	}

    public void AddBody() {
        if (!_isTail) {
            _nextBody.AddBody();
            return;
        }
		lock (MovementBuffer) {
			this.addBodyToEnd = true;
		}

    }


	void FixedUpdate() {
			NextAction();
	}
		
    // Update is called once per frame
    void Update() {
		if (_isBody) {
            return;
        }

        Vector3 thisPosition = this.transform.position;
        Vector3 viewPortPostion = MainCam.WorldToViewportPoint(thisPosition);
        //Debug.Log(viewPortPostion);
        // Vector3 position = this.transform.position;
        // if (_lastMode != MoveMentMode.DownBorder && _lastMode != MoveMentMode.RightBorder && _lastMode != MoveMentMode.LeftBorder && _lastMode != MoveMentMode.UpBorder) {
        if (viewPortPostion.x >= 1) {
            //reach right border of camera
            SetNextActionMode(MoveMentMode.LeftBorder);
        } else if (viewPortPostion.x <= 0) {
            SetNextActionMode(MoveMentMode.RightBorder);
        } else if (viewPortPostion.y <= 0) {
            SetNextActionMode(MoveMentMode.UpBorder);
        } else if (viewPortPostion.y >= 1) {
            SetNextActionMode(MoveMentMode.DownBorder);
        }
        //}

        if (Input.GetKeyDown("up")) {
            SetNextActionMode(MoveMentMode.Up);
        } else if (Input.GetKeyDown("down")) {
            SetNextActionMode(MoveMentMode.Down);
        } else if (Input.GetKeyDown("left")) {
            SetNextActionMode(MoveMentMode.Left);
        } else if (Input.GetKeyDown("right")) {
            SetNextActionMode(MoveMentMode.Right);
        }

        if (Input.GetKeyDown("space")) {
            AddBody();
        }
        // Debug.Log(position);
    }
}

public class MovementQueueEntry {
    public MovementQueueEntry(Movement.MoveMentMode mode) {
        Mode = mode;
    }

    public MovementQueueEntry(Movement.MoveMentMode mode, Vector3 position) {
        Mode = mode;
        Position = position;
    }

    public Movement.MoveMentMode Mode;
    public Vector3? Position;


}
