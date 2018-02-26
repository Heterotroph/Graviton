using UnityEngine;

public class StarsParticleSystem : MonoBehaviour {

	public GameObject Graviton;
	public float Size;
	public int Count;

	private Rigidbody2D gravitonRB;
	private ParticleSystem.Particle[] points;

	void Start() {
		gravitonRB = Graviton.GetComponent<Rigidbody2D>();
		MakeParticles();
	}

	private void MakeParticles() {
		points = new ParticleSystem.Particle[Count];
		for (int i = 0; i < Count; i++) {
			points[i].position = getRandomPosition(false);
			points[i].startColor = new Color(1, 1, 1, Random.value + 0.5f);
			points[i].startSize = Size;
		}
	}

	void Update() {
		UpdateParticles();
	}

	void UpdateParticles() {
		var velo = gravitonRB.velocity;
		var va = velo.x < 0;
		var vb = velo.y < 0;
		var vc = velo.x > 0;
		var vd = velo.y > 0;
		for (int i = 0; i < Count; i++) {
			var position = Camera.main.WorldToViewportPoint(points[i].position);
			var pa = position.x >= 1.2f;
			var pb = position.y >= 1.2f;
			var pc = position.x <= -0.2f;
			var pd = position.y <= -0.2f;
			var pe = position.z <= 0;
			if (va && pa || vb && pb || vc && pc || vd && pd || pe) {
				points[i].position = getRandomPosition(true);
			}
		}
		gameObject.GetComponent<ParticleSystem>().SetParticles(points, points.Length);
	}

	Vector3 getRandomPosition(bool isOutsideViewport) {
		var velo = gravitonRB.velocity.normalized;
		Vector3 point = new Vector3(
			Random.Range(velo.x - 0.33f, 1.33f + velo.x),
			Random.Range(velo.y - 0.33f, 1.33f + velo.y),
			(Mathf.Log(Random.Range(0.05f, 0.99f), 2) / 10 + 1) * Camera.main.farClipPlane
		);
		var px = point.y > 0 && point.y < 1;
		var py = point.x > 0 && point.x < 1;
		if (px && py && isOutsideViewport) return getRandomPosition(isOutsideViewport);
		return Camera.main.ViewportToWorldPoint(point);
	}
}