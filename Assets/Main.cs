using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

	const float GRAVITY = 0.000000000066742f * 1000000000;
	const float MAX_MASS = 10000.0f;
	const float MIN_MASS = 100.0f;
	const float MAX_FORCE = 15.0f;
	const float MAX_VELOCITY = 12.0f;

	public GameObject Graviton;
	public GameObject Blackhole;
	public GameObject Body;
	public GameObject SpawPivot;

	private List<GameObject> bodies = new List<GameObject>();
	private Rigidbody2D gravitonRB;
	private Rigidbody2D blackholeRB;
	private CircleCollider2D blackholeCC;
	private Transform spawnPivot;

	void Start () {
		gravitonRB = Graviton.GetComponent<Rigidbody2D>();
		blackholeRB = Blackhole.GetComponent<Rigidbody2D>();
		blackholeCC = Blackhole.GetComponent<CircleCollider2D>();
		spawnPivot = SpawPivot.transform;
	}

	void Update() {
		MoveCamera();
		ProcessInput();
		BlahBlah();
		ApplyBodiesGravitation();
		ApplyVelocityLimit();
	}

	void ProcessInput() {
		if (Input.GetMouseButtonDown(0)) {
			blackholeRB.mass = MIN_MASS;
			Blackhole.GetComponent<SpriteRenderer>().enabled = true;
			Debug.Log("DOWN " + blackholeRB.mass);
		}
		if (Input.GetMouseButton(0)) {
			Blackhole.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
			ApplyGravitation(blackholeRB);
			var massAcceleration = (MAX_MASS - MIN_MASS) * 0.1f;
			blackholeRB.mass = Mathf.Min(blackholeRB.mass + massAcceleration * Time.deltaTime, MAX_MASS);
			Debug.Log("HOLD " + blackholeRB.mass);
			Debug.Log("VELO " + gravitonRB.velocity + "  " + gravitonRB.velocity.magnitude);
		} else if (Input.GetMouseButtonUp(0)) {
			blackholeRB.mass = MIN_MASS;
			Blackhole.GetComponent<SpriteRenderer>().enabled = false;
			Debug.Log("UP_0 " + blackholeRB.mass);
		}
		if (Input.GetMouseButtonUp(1)) {
			blackholeRB.mass = MIN_MASS;
			gravitonRB.AddForce(Vector2.zero, ForceMode2D.Force);
			Debug.Log("UP_1 " + blackholeRB.mass);
			gravitonRB.velocity = Vector2.zero;
			Debug.Log("VELO " + gravitonRB.velocity + "  " + gravitonRB.velocity.magnitude);
		}
	}

	void MoveCamera() {
		var target = new Vector3(Graviton.transform.position.x, Graviton.transform.position.y, Camera.main.transform.position.z);
		Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, target, 1f);
	}

	void BlahBlah() {
		var start = spawnPivot.position;
		var end = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 1));
	}

	void SpawnBody(Vector3 position) {
		var body = Object.Instantiate(Body);
		body.GetComponent<Rigidbody2D>().mass = Random.Range(MIN_MASS, MAX_MASS);
		body.transform.position = position;
		bodies.Add(body);
	}

	void ApplyVelocityLimit() {
		var multiplier = Mathf.Min(MAX_VELOCITY / gravitonRB.velocity.magnitude, 1);
		gravitonRB.velocity = new Vector2(gravitonRB.velocity.x * multiplier, gravitonRB.velocity.y * multiplier);
	}

	void ApplyBodiesGravitation() {
		for (var i = 0; i < bodies.Count; i++) {
			ApplyGravitation(bodies[i].GetComponent<Rigidbody2D>());
		}
	}

	void ApplyGravitation(Rigidbody2D body) {
		var p0 = body.transform.position;
		var p1 = gravitonRB.transform.position;
		var m0 = body.mass;
		var m1 = gravitonRB.mass;
		var r = Vector3.Distance(p0, p1);
		var force = m0 * m1 / (r * r) * GRAVITY * Time.deltaTime;
		force = Mathf.Clamp(force, -MAX_FORCE, MAX_FORCE);
		var vector = (p0 - p1).normalized * force;
		gravitonRB.AddForce(vector, ForceMode2D.Force);
	}
}
