using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour 
{
	//public variables to control camera
	public bool follow = true;
	public GameObject ball;
	public GameObject basket;
	public float cameraDistance = 2;
	public float smoothTime = 0.5f;

	Vector3 vel;
	Vector3 baskertPos;
	Vector3 initialPos;
	Vector3 ballStartPosition;

	void Start ()
	{

		baskertPos = basket.transform.position; // get the basket position to strict camera movement
		initialPos = transform.position; // initial camera position
		ballStartPosition = ball.transform.position ; // strat position of ball
		cameraDistance = ball.transform.position.z - transform.position.z; // diatance of camera from the ball
		follow = true; // variable is turned to true so the camera follows the ball
		ChangeCameraAngle ();
	}
	

	void Update () {

		if (follow) {// if the ball is shot
			if (transform.position.z > (baskertPos.z - 4f)) { // if the camera is ahead of ball then reset the camera
				transform.position = initialPos; 
			}	
			Vector3 target = ball.transform.position;
			if (ball.transform.position.y > this.transform.position.y)
			{// if the ball is above the camera then reset the positions
				target.y = ball.transform.position.y;// + 0.61f;
			} else {
				target.y = transform.position.y;// + 0.61f;
			}
			target.z = ball.transform.position.z - (cameraDistance + 1); // set the z position of ball

			transform.position = Vector3.SmoothDamp (transform.position, target, ref vel, smoothTime);// smothen the camera movement
		}
        else
        { // of ball is not shot yet
            Vector3 target1 = ball.transform.position;// set ball position variable
            if (ball.transform.position.y > this.transform.position.y)
            {// set the position of camera according to ball 
                target1.y = ball.transform.position.y;// + 0.61f;
                target1.x = transform.position.x;
                target1.z = transform.position.z;
                transform.position = Vector3.SmoothDamp(transform.position, target1, ref vel, smoothTime); // smothen and reposition the camera
            }
            else
			{// set the position of camera according to ball 
				target1.y = initialPos.y;// + 0.61f;
                target1.x = transform.position.x;
                target1.z = transform.position.z;
                transform.position = Vector3.SmoothDamp(transform.position, target1, ref vel, smoothTime); // smothen and reposition the camera
			}
        }
        if (transform.position.z > baskertPos.z - 4f) {// if camera exceed the basket pole position then set the follow to false
			follow = false;
		}



	}

	public void ChangeCameraAngle()
	{
		// this method is implemented to change the rendom angle and position
		//pos = x [-3 , 2] y [1.3 , 3.76]
		//rot = x [ 0 , 5] y [-4.81 , 5.83]

		// set the rotation of camera
		float randPosX = Random.Range (-3f, 2f);
		float randPosY = Random.Range (1.3f, 3.76f);
		float randRotX = Random.Range (0f, 5f);
		float randRotY = Random.Range (-4.81f, 5.83f);

		Vector3 position = new Vector3(randPosX,randPosY,transform.position.z);
		Vector3 rotation = new Vector3 (randRotX,randRotY,transform.rotation.z);

		transform.position = position;
		transform.rotation = Quaternion.Euler (rotation);
		ResetState ();
	}
	void ResetState()
	{
		// reset the ball for another shot
		transform.position = initialPos;
		follow = true;
	}
}
