using UnityEngine;
using System.Collections;

public class laser_movement : MonoBehaviour {
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
				gameObject.transform.position += new Vector3(xDir*0.8f, yDir*0.8f, zDir*0.8f);
			}
		}
	}

  public void setDirection(int x, int y, int z) {
    xDir = x;
    zDir = y;
    yDir = z;
		// pick destination
		target = new Vector3(gameObject.transform.position.x + 30f*xDir, gameObject.transform.position.y + 30f*yDir, gameObject.transform.position.z + 30f*zDir);
    // turn to face destination
		Vector3 vectorDirectionToTarget = (target-gameObject.transform.position).normalized;
		Quaternion finalRotation = Quaternion.LookRotation(vectorDirectionToTarget);
		gameObject.transform.rotation = finalRotation;
    dirSet = true;
  }
}
