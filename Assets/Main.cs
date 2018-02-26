using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

	const float GRAVITY = 0.000000000066742f * 1000000000;
	const float MAX_MASS = 10000.0f;
	const float MIN_MASS = 2000.0f;
	const float MAX_FORCE = 20f;
	const float MAX_VELOCITY = 3.0f;
	const float CEIL_SIZE = 20f;

	public GameObject Graviton;
	public GameObject Blackhole;
	public GameObject Body;
	public GameObject SpawnRect;

	private List<GameObject> bodies = new List<GameObject>();
	private Hashtable bodiesData = new Hashtable();
	private Rigidbody2D gravitonRB;
	private Rigidbody2D blackholeRB;
	private CircleCollider2D blackholeCC;
	private RectTransform spawnPivotT;

	private float timer = 0;

	void Start () {
		Blackhole.GetComponent<SpriteRenderer>().enabled = false;
		gravitonRB = Graviton.GetComponent<Rigidbody2D>();
		blackholeRB = Blackhole.GetComponent<Rigidbody2D>();
		blackholeCC = Blackhole.GetComponent<CircleCollider2D>();
		spawnPivotT = SpawnRect.GetComponent<RectTransform>();
	}

	void Update() {
		timer += Time.deltaTime;
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
		}
		if (Input.GetMouseButton(0)) {
			Blackhole.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
			ApplyGravitation(blackholeRB, gravitonRB);
			var massAcceleration = (MAX_MASS - MIN_MASS) * 0.1f;
			blackholeRB.mass = Mathf.Min(blackholeRB.mass + massAcceleration * Time.deltaTime, MAX_MASS);
		} else if (Input.GetMouseButtonUp(0)) {
			blackholeRB.mass = MIN_MASS;
			Blackhole.GetComponent<SpriteRenderer>().enabled = false;
		}
		if (Input.GetMouseButtonUp(1)) {
			blackholeRB.mass = MIN_MASS;
			gravitonRB.AddForce(Vector2.zero, ForceMode2D.Force);
			gravitonRB.velocity = Vector2.zero;
		}
	}

	void MoveCamera() {
		var target = new Vector3(Graviton.transform.position.x, Graviton.transform.position.y, Camera.main.transform.position.z);
		Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, target, 1f);
	}

	void BlahBlah() {
		var corners = new Vector3[4];
		spawnPivotT.GetWorldCorners(corners);
		var startX = corners[0].x - corners[0].x % CEIL_SIZE;
		var endX = corners[2].x - corners[2].x % CEIL_SIZE;
		var startY = corners[0].y - corners[0].y % CEIL_SIZE;
		var endY = corners[2].y - corners[2].y % CEIL_SIZE;
		for (var x = startX; x <= endX; x += CEIL_SIZE) {
			for (var y = startY; y <= endY; y += CEIL_SIZE) {
				var key = x + "_" + y;
				var position = Camera.main.WorldToViewportPoint(new Vector3(x, y, 0));
				var pa = position.x > 1f;
				var pb = position.y > 1f;
				var pc = position.x < 0f;
				var pd = position.y < 0f;
				var isOutsideViewport = pa || pb || pc || pd;
				if (bodiesData[key] == null && isOutsideViewport) {
					var rand = Random.Range(0, CEIL_SIZE);
					bodiesData[key] = new BodyData {
						mass = 6000,
						position = new Vector3(x + rand, y + rand, 0)
					};
					SpawnBody((BodyData)bodiesData[key]);
				}
			}
		}

	}

	void SpawnBody(BodyData data) {
		var body = Object.Instantiate(Body);
		body.GetComponent<Rigidbody2D>().mass = data.mass;
		body.transform.position = data.position;
		bodies.Add(body);
	}

	void ApplyVelocityLimit() {
		var multiplier = Mathf.Min(MAX_VELOCITY / gravitonRB.velocity.magnitude, 1);
		gravitonRB.velocity = new Vector2(gravitonRB.velocity.x * multiplier, gravitonRB.velocity.y * multiplier);
	}

	void ApplyBodiesGravitation() {
		for (var i = 0; i < bodies.Count; i++) {
			ApplyGravitation(bodies[i].GetComponent<Rigidbody2D>(), gravitonRB);
		}
	}

	void ApplyGravitation(Rigidbody2D source, Rigidbody2D target) {
		var p0 = source.transform.position;
		var p1 = target.transform.position;
		var m0 = source.mass;
		var m1 = target.mass;
		var r = Vector3.Distance(p0, p1);
		var force = m0 * m1 / (r * r) * GRAVITY * Time.deltaTime;
		force = Mathf.Clamp(force, -MAX_FORCE, MAX_FORCE);
		var vector = (p0 - p1).normalized * force;
		target.AddForce(vector, ForceMode2D.Force);
	}


	class BodyData {
		public float mass { get; set; }
		public Vector3 position { get; set; }
	}

}
