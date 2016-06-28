using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour {

    //public static Queue<MovementQueueEntry> MovementQueue = new Queue<MovementQueueEntry>();
    public static MovementQueueEntry[] MovementBuffer = new MovementQueueEntry[1024];
    public static int NextActionAddIndex = 0;
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


    private bool _isBody = false;
    private bool _isTail = true;
    private Rigidbody2D _rigidbody;
    public MoveMentMode LastMode;
    private Movement _nextBody;
    private int nextActionIndex = 0;

    // Use this for initialization
    void Start() {
        this._rigidbody = this.GetComponent<Rigidbody2D>();
        this._rigidbody.gravityScale = 0;
        this._rigidbody.isKinematic = true;
    }

    void SetNextActionMode(MoveMentMode mode) {
        //Debug.Log(mode);
        if(NextActionAddIndex == 0) ResetBufferIndex();
        MovementBuffer[NextActionAddIndex++] = new MovementQueueEntry(mode);
    }

    public void ResetBufferIndex() {
        this.nextActionIndex = 0;
        if(!_isTail)
            this._nextBody.ResetBufferIndex();
    }

    public void InitBody(int actionIndex) {
        _isBody = true;
        this.nextActionIndex = actionIndex;
    }

    public void NextAction() {

        if (MovementBuffer[nextActionIndex] == null) {
            if (_isTail) {
                NextActionAddIndex = 0;
            }
            return;
        }
        
        MovementQueueEntry entry = MovementBuffer[nextActionIndex];
        var next = entry.Mode;
        Vector3? transPosition = entry.Position;
        Vector3 thisPosition = this.transform.position;
        //Debug.Log(MovementBuffer[nextActionIndex].Position + "" +thisPosition +this.gameObject.ToString());
        if (transPosition == null) {
            transPosition = thisPosition;
            entry.Position = thisPosition;
        } else  if (!(Mathf.Abs(thisPosition.x - transPosition.Value.x) < 0.35f) || !(Mathf.Abs(thisPosition.y - transPosition.Value.y) < 0.35f)) {
            return;
        }
        this.transform.position = transPosition.Value;
        nextActionIndex++;

        switch (next) {

            case MoveMentMode.Right: {
                    this._rigidbody.velocity = new Vector2(1f * SpeedMutiplier, 0.0f);
                    LastMode = next;
                    break;
                }
            case MoveMentMode.Left: {
                    this._rigidbody.velocity = new Vector2(-1f * SpeedMutiplier, 0.0f);
                    LastMode = next;
                    break;
                }
            case MoveMentMode.Up: {
                    this._rigidbody.velocity = new Vector2(0.0f, 1f * SpeedMutiplier);
                    LastMode = next;
                    break;
                }
            case MoveMentMode.Down: {
                    this._rigidbody.velocity = new Vector2(0.0f, -1f * SpeedMutiplier);
                    LastMode = next;
                    break;
                }


            case MoveMentMode.RightBorder: {
                    this.transform.position = new Vector3(MainCam.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x, thisPosition.y, 0.0f);
                    break;
                }
            case MoveMentMode.LeftBorder: {
                    this.transform.position = new Vector3(MainCam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x, thisPosition.y, 0.0f);
                    break;
                }
            case MoveMentMode.UpBorder: {
                    this.transform.position = new Vector3(thisPosition.x, (MainCam.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y));
                    break;
                }
            case MoveMentMode.DownBorder: {
                    this.transform.position = new Vector3(thisPosition.x, (MainCam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y));
                    break;
                }

        }

        

        //_nextBody.NextAction();


    }

    void AddBody() {
        if (!_isTail) {
            _nextBody.AddBody();
            return;
        }
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
        nextBody.LastMode = this.LastMode;
        nextBody.InitBody(this.nextActionIndex-1 < 0 ? 0 : nextActionIndex - 1);
        _nextBody = nextBody;

    }

    // Update is called once per frame
    void Update() {
        if (_isBody) {
            NextAction();
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

        NextAction();
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
