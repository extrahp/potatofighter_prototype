using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {
	// set normal moving speed
	public float runningSpeed = 5;
	// set normal jumping speed
	public float jumpForce = 1000;
	// set maximum falling speed
	public float maxFallingSpeed = 50;
	// the thing
	Rigidbody2D rb;

	// is the character knocked?
	private bool knocked;
	private bool canJump = false;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {

		// Move left/right
		if (Input.GetButton ("Horizontal"))
			transform.position = new Vector2 (transform.position.x + Time.deltaTime * runningSpeed * Input.GetAxis("Horizontal"), transform.position.y);

		// Jump
		if (canJump && Input.GetButtonDown ("Jump")) {
			rb.AddForce (transform.up * jumpForce);
			canJump = false;
		}

		// Limit the falling speed to prevent wtf if the character is not knocked and just free falling
		if (rb.velocity.magnitude > maxFallingSpeed && !knocked)
			rb.velocity = rb.velocity.normalized * maxFallingSpeed;
	}

	void OnTriggerEnter2D (Collider2D other) {

		// if the player object fell onto a platform doe
		if (other.gameObject.layer == LayerMask.NameToLayer("Platform") && rb.velocity.y <= 0.0f) {
			print (canJump);
			canJump = true;
		}
	}
}
