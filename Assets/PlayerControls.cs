using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {
	// set normal moving speed
	public float runningSpeed = 5;
	// set normal jumping speed
	public float jumpForce = 1000;
	// set maximum falling speed
	public float maxFallingSpeed = 50;

	public BoxCollider2D colliderBox;

	// the thing
	Rigidbody2D rb;

	// is the character knocked?
	private bool knocked;
	private bool canJump = false;
	private bool canPassThrough = false;
	private float platformY;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {

		// Move left/right
		if (Input.GetButton ("Horizontal")) {
			float dir = (Input.GetAxis ("Horizontal") / Mathf.Abs (Input.GetAxis ("Horizontal")));
			int wallLayer = 1 << 9;
			RaycastHit2D hitCheck = Physics2D.Raycast (transform.position, new Vector2 (dir, 0), colliderBox.size.x / 2 + 0.1f + (Mathf.Abs(Input.GetAxis("Horizontal")) * runningSpeed * Time.deltaTime), wallLayer);
			if (hitCheck == false) {
				transform.position = new Vector2 (transform.position.x + Time.deltaTime * runningSpeed * Input.GetAxis ("Horizontal"), transform.position.y);
			} else {
				transform.position = new Vector2 (transform.position.x + dir * (hitCheck.distance - colliderBox.size.x/2), transform.position.y);
			}
		}

		// Jump
		if (canJump && Input.GetButtonDown ("Jump")) {
			rb.AddForce (transform.up * jumpForce);
			canJump = false;
		}

		// Fall Down
		if (canPassThrough && Input.GetButton ("Down")) {
			canJump = false;
			gameObject.layer = 10;
		}
		if (gameObject.layer == 10) {
			if (transform.position.y < platformY - 2) {
				gameObject.layer = 0;
			}
		}
		// Limit the falling speed to prevent wtf if the character is not knocked and just free falling
		if (rb.velocity.magnitude > maxFallingSpeed && !knocked)
			rb.velocity = rb.velocity.normalized * maxFallingSpeed;
	}

	void OnTriggerEnter2D (Collider2D other) {

		// if the player object fell onto a platform doe
		if (other.gameObject.layer == LayerMask.NameToLayer("Platform") && rb.velocity.y <= 0.0f) {
			platformY = other.gameObject.transform.position.y;
			print (canJump);
			canJump = true;
			canPassThrough = true;
		}

		// if the player object fell onto a platform doe
		if (other.gameObject.layer == LayerMask.NameToLayer("HardPlatform") && rb.velocity.y <= 0.0f) {
			print (canJump);
			canJump = true;
			canPassThrough = false;
		}
	}
}
