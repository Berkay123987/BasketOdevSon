using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Shoot : MonoBehaviour
{
	//-----------------------------------------------------------------------------
	public int kicksCount = 5; // numbers of turns that the user has
	//====================================================================

	#region U N W A N T E D

	public static Shoot share;

	public static Action EventShoot = delegate {
	};
	public static Action<float> EventChangeSpeedZ = delegate {
	};
	public static Action<float> EventChangeBallZ = delegate {
	};
	public static Action<float> EventChangeBallX = delegate {
	};
	public static Action<float> EventChangeBallLimit = delegate {
	};
	public static Action<Collision> EventOnCollisionEnter = delegate {
	};
	public static Action EventDidPrepareNewTurn = delegate {
	};


	public float _ballControlLimit;

	public Transform _ballTarget;
	protected Vector3 beginPos;
	protected bool _isShoot = false;

	public float minDistance = 100;

	public Rigidbody _ball;
	public float factorUp = 0.012f;
	// 10f
	public float factorDown = 0.003f;
	// 1f
	public float factorLeftRight = 0.095f;
	// 2f
	public float factorLeftRightMultiply = 0.8f;
	// 2f
	public float _zVelocity = 34f;

	public AnimationCurve _curve;

	protected float factorUpConstant = 0.025f * 960f;
	// 0.015f * 960f;
	public float factorDownConstant ;//0.006f * 960f;
	// 0.005f * 960f;
	protected float factorLeftRightConstant = 0.0235f * 640f;
	// 0.03f * 640f; // 0.03f * 640f;
	public float _speedMin = 5;
	// 20f;
	public float _speedMax = 10;
	// 36f;


	public Transform _ballShadow;





	public float _distanceMinZ;
	public float _distanceMaxZ;

	public float _distanceMinX;
	public float _distanceMaxX;

	public bool _isShooting = false;
	public bool _canControlBall = false;

	public Transform _cachedTrans;

	public bool _enableTouch = false;
	public float screenWidth;
	public float screenHeight;

	Vector3 _prePos, _curPos;
	public float angle;
	protected ScreenOrientation orientation;

	protected Transform _ballParent;

	protected RaycastHit _hit;
	public bool _isInTutorial = false;
	public Vector3 ballVelocity;

	private float _ballPostitionZ = -22f;
	private float _ballPostitionX = 0f;

	public float BallPositionZ {
		get { return _ballPostitionZ; }
		set { _ballPostitionZ = value; }
	}

	public float BallPositionX {
		get { return _ballPostitionX; }
		set { _ballPostitionX = value; }
	}

	public TrailRenderer _effect;

	public UiManager UiM;

	protected virtual void Awake ()
	{
		//get the instance of UI manager to set UI
		UiM = GetComponent<UiManager> ();

		// setting all the factors of ball
		share = this;
		_cachedTrans = transform;
		_isShooting = true;
		_ballParent = _ball.transform.parent;
			_distanceMinX = 0f;
			_distanceMaxX = 0f;
			_distanceMinZ = -14f;
			_distanceMaxZ = -14f;

	}

	public virtual void goalEvent (bool isGoal)
	{
		_canControlBall = false;
		_isShooting = false;
	}

	public void calculateFactors () // egt the screen diamentions and set the factors accordingly
	{
		screenHeight = Screen.height;
		screenWidth = Screen.width;

		minDistance = (100 * screenHeight) / 960f;

		factorUp = factorUpConstant / screenHeight;
		factorDown = factorDownConstant / screenHeight;
		factorLeftRight = factorLeftRightConstant / screenWidth;



	}

	protected void LateUpdate ()
	{
		if (screenHeight != Screen.height) {
			orientation = Screen.orientation;
			calculateFactors ();
		}
	}
	public void enableTouch ()
	{
		_enableTouch = true;
	}

	public void disableTouch ()
	{
		StartCoroutine (_disableTouch ());
	}

	private IEnumerator _disableTouch ()
	{
		yield return new WaitForEndOfFrame ();
		_enableTouch = false;
	}
	void FixedUpdate ()
	{
		ballVelocity = _ball.velocity;

		Vector3 pos = _ball.transform.position;
		pos.y = 0.015f;
	}
	#endregion

	//====================================================================

	// Use this for initialization
	protected virtual void Start ()
	{
		// enable the toucher at the start
		enableTouch ();

		
#if UNITY_WP8 || UNITY_ANDROID
		Time.maximumDeltaTime = 0.2f;
		Time.fixedDeltaTime = 0.008f;
#else
		Time.maximumDeltaTime = 0.1f;
		Time.fixedDeltaTime = 0.005f;
#endif

		orientation = Screen.orientation;
		calculateFactors ();

		EventChangeBallLimit (_ballControlLimit);

		reset ();
	}

	protected virtual void Update ()
	{

		if (_enableTouch) {		
				if (Input.GetMouseButtonDown (0)) {			// touch phase began
					mouseBegin (Input.mousePosition);

				} else if (Input.GetMouseButton (0)) {			
					mouseMove (Input.mousePosition); // touch moved


				} 
				else if (Input.GetMouseButtonUp (0)) 
				{	// touch ended
					if (kicksCount > 0)// check if the player has remaining turns
					{
						kicksCount--;
						//handle score here
					}
					mouseEnd ();

				}
			if (_isShoot) {
				Vector3 speed = _ballParent.InverseTransformDirection (_ball.velocity);
				speed.z = _zVelocity;
				_ball.velocity = _ballParent.TransformDirection (speed);
			}
		}
	}

	public void mouseBegin (Vector3 pos)
	{
		_prePos = _curPos = pos;
		beginPos = _curPos;

	}

	public void mouseEnd ()
	{
		if (_isShoot == true) {		// when the touch is ended disable the ball control and reset the ball
			_canControlBall = false;
			_isShoot = false;
			disableTouch ();
			StartCoroutine (resetCoroutine ());
			
		}
	}

	IEnumerator resetCoroutine ()
	{// reset after e seconds
		yield return new WaitForSeconds (3f);
		reset ();
		enableTouch ();
	}


	public void mouseMove (Vector3 pos)
	{
		if (_curPos != pos) {		// touch phase moved
			_prePos = _curPos;
			_curPos = pos;

			
			Vector3 distance = _curPos - beginPos;
			
			if (_isShoot == false) {				// NOT ALONE
				if (distance.y > 0 && distance.magnitude >= minDistance) {		
					if (_hit.transform != _cachedTrans) {
						_isShoot = true;
						
						Vector3 point1 = _hit.point;		// contact point
						point1.y = 0;
						point1 = _ball.transform.InverseTransformPoint (point1);        // With point1 about the ball's prescription, consider the ball to be its own prescription
						point1 -= Vector3.zero;         // vector created by point and origin 'coordinates

						Vector3 diff = point1;
						diff.Normalize ();              // normalized is very important when calculating the angle

						float angle = 90 - Mathf.Atan2 (diff.z, diff.x) * Mathf.Rad2Deg;
						//	Debug.Log("angle = " + angle);
						
						float x = _zVelocity * Mathf.Tan (angle * Mathf.Deg2Rad);				
						
						//							float x = distance.x * factorLeftRight;
						_ball.velocity = _ballParent.TransformDirection (new Vector3 (x, distance.y * factorUp, _zVelocity));
						_ball.angularVelocity = new Vector3 (0, x, 0f);
						
						if (EventShoot != null) {
							EventShoot ();
						}
					}
				}
			} else {				//has ~ shot 'already', depending on current and previous touch frame deflection 'will make the ball' left ', right ~, up and down' respectively '
				if (true) {	// neu nhac ngon tay len khoi man hinh roi thi ko cho dieu khien banh nua

					if (_cachedTrans.position.z < -_ballControlLimit) {
						// If the ball is more than 6m from the basket, then 'let the ball spin, go within 6m, no more ~ to ~ make the game balanced`

						distance = _curPos - _prePos;

						Vector3 speed = _ballParent.InverseTransformDirection (_ball.velocity);
						speed.y += distance.y * ((distance.y > 0) ? factorUp : factorDown);
						speed.x += distance.x * factorLeftRight * factorLeftRightMultiply;
						_ball.velocity = _ballParent.TransformDirection (speed);

						speed = _ball.angularVelocity;
						speed.y += distance.x * factorLeftRight;
						_ball.angularVelocity = speed;
						//**************************************************************************************
			
					} else {
						_canControlBall = false;

					}
				}
			}
		}
	}


	private void enableEffect (){}

	public virtual void reset ()
	{
		reset (Random.Range (_distanceMinX, _distanceMaxX), Random.Range (_distanceMinZ, _distanceMaxZ));
		GameObject.Find ("Main Camera").GetComponent<CameraManager> ().follow = true;
	}

	public virtual void reset (float x, float z)
	{
		Invoke ("enableEffect", 0.1f);

		BallPositionX = x;
		EventChangeBallX (x);
		BallPositionZ = z;
		EventChangeBallZ (z);


		_canControlBall = true;
		_isShoot = false;
		_isShooting = true;

		// reset ball
		_ball.velocity = Vector3.zero;
		_ball.angularVelocity = Vector3.zero;
		_ball.transform.localEulerAngles = Vector3.zero;


		Vector3 pos = new Vector3 (BallPositionX, 0f, BallPositionZ);
		Vector3 diff = -pos;
		diff.Normalize ();
		float angleRadian = Mathf.Atan2 (diff.z, diff.x);       // calculate the 'angle' of deviation
		float angle = 90 - angleRadian * Mathf.Rad2Deg;

		_ball.transform.parent.localEulerAngles = new Vector3 (0, angle, 0);        // parent set of ball rotate 1 due to truc y = angle deviation

		_ball.transform.position = new Vector3 (BallPositionX, 0.16f, BallPositionZ);

		pos.x = 0;

		float val = (Mathf.Abs (_ball.transform.localPosition.z) - _distanceMinZ) / (_distanceMaxZ - _distanceMinZ);
		_zVelocity = Mathf.Lerp (_speedMin, _speedMax, val);

		EventChangeSpeedZ (_zVelocity);

		EventDidPrepareNewTurn ();

	}
}
