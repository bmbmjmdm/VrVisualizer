using UnityEngine;
using System.Collections;

public class asteroid_movement : MonoBehaviour {
  private Vector3 target;
  int xDir = 0;
  int zDir = 0;
  int yDir = 0;
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
			if (Vector3.Distance (gameObject.transform.position, target) <= 0.1f) {
				Destroy(gameObject);
			}
			else {
				gameObject.transform.position += new Vector3(xDir*0.2f, yDir*0.2f, zDir*0.2f);
			}
		}
	}

  public void setDirection(int x, int y, int z) {
    xDir = x;
    zDir = y;
    yDir = z;
		// pick destination
		target = new Vector3(gameObject.transform.position.x + 20f*xDir, gameObject.transform.position.y + 20f*yDir, gameObject.transform.position.z + 20f*zDir);
    dirSet = true;
  }
}
