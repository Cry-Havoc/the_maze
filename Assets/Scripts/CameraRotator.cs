using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour {

	float m_cameraSpeed = 10f;
	
	// Update is called once per frame
	void Update () {

		transform.RotateAround (Vector3.zero, Vector3.up, Time.deltaTime * m_cameraSpeed);

	}
}
