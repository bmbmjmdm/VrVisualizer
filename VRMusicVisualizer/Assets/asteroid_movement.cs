using UnityEngine;
using System.Collections;

public class asteroid_movement : MonoBehaviour {
  private Vector3 target;
  float xDir = 0;
  float zDir = 0;
  float yDir = 0;
  private float clock = 0f;
  bool dirSet = false;


  void Start() {
	}

	void Update () {
    if (!dirSet) return;
    clock += Time.deltaTime;
    
    if (clock >= 0.02) {
      clock = 0f;
			// fly towards target and then self-destruct
			if (Vector3.Distance (gameObject.transform.position, target) <= 1.4f) {
				Destroy(gameObject);
			}
			else {
				gameObject.transform.position += new Vector3(xDir*0.7f, yDir*0.7f, zDir*0.7f);
			}
		}
	}

  public void setDirection(float x, float y, float z) {
    xDir = x;
    zDir = y;
    yDir = z;
		// pick destination
		target = new Vector3(gameObject.transform.position.x + 100f*xDir, gameObject.transform.position.y + 100f*yDir, gameObject.transform.position.z + 100f*zDir);
    dirSet = true;
  }
}
