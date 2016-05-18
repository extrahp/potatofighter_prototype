using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {
	// set normal moving speed
	public float runningSpeed = 5;
	// set normal jumping speed
	public float jumpForce = 1000;
	// set maximum falling speed
	public float maxFallingSpeed = 50;

	// the box collider object thing
	public BoxCollider2D colliderBox;

	// the thing
	Rigidbody2D rb;

	Animator anim;

	// is the character knocked?
	private bool knocked = false;
	// can the character jump
	private bool canJump = false;
	// can it pass through the platforms
	private bool canPassThrough = false;
	// the y position of the current platform the character is standing on
	private float platformY;
	//object direction
	private float dir = 1;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		anim.SetBool("run", Input.GetButton("Horizontal") && rb.velocity.y == 0);
		anim.SetBool ("onground", canJump);
		if (rb.velocity.y < 0f)
			anim.SetTrigger ("falling");
		// Move left/right
		if (Input.GetButton ("Horizontal")) {
			float input = (Input.GetAxis ("Horizontal") / Mathf.Abs (Input.GetAxis ("Horizontal")));
			if (input > 0 || input < 0)
				dir = input;
			transform.localScale = new Vector3 (dir, transform.localScale.y, transform.localScale.z);
			int wallLayer = 1 << 9;
			RaycastHit2D hitCheck = Physics2D.Raycast (transform.position, new Vector2 (dir, 0), colliderBox.size.x / 2 + 0.1f + (Mathf.Abs(Input.GetAxis("Horizontal")) * runningSpeed * Time.deltaTime), wallLayer);
			if (hitCheck == false) {
				transform.position = new Vector2 (transform.position.x + Time.deltaTime * runningSpeed * Input.GetAxis ("Horizontal"), transform.position.y);
			} else {
				transform.position = new Vector2 (transform.position.x + dir * (hitCheck.distance - colliderBox.size.x/2), transform.position.y);
			}
		}

		// Jump
		if (canJump  && !Input.GetButton ("Down") && Input.GetButtonDown ("Jump")) {
			anim.SetTrigger ("jump");
			rb.AddForce (transform.up * jumpForce);
			canJump = false;
			canPassThrough = false;
		}

//		// stop going up when release button
//		if (Input.GetButtonUp ("Jump")) {
//			if (rb.velocity.y > 0 && !knocked) {
//				rb.velocity = new Vector2 (rb.velocity.x, 0);
//			}
//		}

		// Fall Down
		if (canPassThrough && Input.GetButton ("Down") && Input.GetButtonDown ("Jump")) {
			canJump = false;
			anim.SetTrigger ("passthrough");
			canPassThrough = false;
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
		if (other != null) { 
			if (other.gameObject.layer == LayerMask.NameToLayer ("Platform") && rb.velocity.y <= 0.0f) {
				platformY = other.gameObject.transform.position.y;
				canJump = true;
				canPassThrough = true;
			}

			// if the player object fell onto a platform doe
			if (other.gameObject.layer == LayerMask.NameToLayer ("HardPlatform") && rb.velocity.y <= 0.0f) {
				canJump = true;
				canPassThrough = false;
			}
		}
	}
}
