using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.XR;

namespace Assets.Scripts
{
public class AsteroidReaction : MonoBehaviour
{
    public GameObject[] asteroids;
    private GameObject player;
    public int numAsteroids;
    private bool hasBeat = false;
    public bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        BeatCollector.registerBeatListener(recieveBeat);
        BeatCollector.registerVerseListener(toggleActive);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasBeat && active) {
            // this gives you a new "forward" vector based on where the user is looking
            Quaternion playerDirection = InputTracking.GetLocalRotation(XRNode.CenterEye);
            playerDirection = new Quaternion(0, playerDirection.y, playerDirection.z, playerDirection.w);
            playerDirection = playerDirection * player.transform.rotation;
            Vector3 fwd = playerDirection * Vector3.forward;
            for (int i = 0; i < numAsteroids; i++) {
                // select the direction the asteroid's flying in so we can make each group fly together
                float x = 0;
                float y = 0;
                float z = 0;
                while (x == 0 && z == 0 && y == 0) {
                    x = UnityEngine.Random.Range(-1.0f, 2.0f);
                    z = UnityEngine.Random.Range(-1.0f, 2.0f);
                    y = UnityEngine.Random.Range(-1.0f, 2.0f);
                }
                // select the relative location of the group so they spawn together
                float ranDistance = UnityEngine.Random.Range(30.0f, 60.0f);
                // This places a random point directly in front of the user, up to 20 units away
                // It then finds another point within (+-40,+-40,+-40)/1.25 of that first point 
                Vector3 pointAlongVisionLine = player.transform.position + fwd * ranDistance;
                pointAlongVisionLine += Vector3.up * UnityEngine.Random.Range(-ranDistance/1.25f, ranDistance/1.25f);
                pointAlongVisionLine += Vector3.right * UnityEngine.Random.Range(-ranDistance/1.25f, ranDistance/1.25f);
                pointAlongVisionLine += Vector3.forward * UnityEngine.Random.Range(-ranDistance/1.25f, ranDistance/1.25f);
                // spawn a group of 3-7
                for (int ii = 0; ii < UnityEngine.Random.Range(1, 8); ii++) {
                    // exclusive so dont have to do Length-1
                    int ranObj = UnityEngine.Random.Range(0, asteroids.Length);
                    // Spawns an asteroid within (+-2, +-2, +-2) of that second point we selected, where it flies in asteroid_movement.cs
                    Vector3 oneOfManyPosition = new Vector3(pointAlongVisionLine.x, pointAlongVisionLine.y, pointAlongVisionLine.z);
                    oneOfManyPosition += Vector3.up * UnityEngine.Random.Range(-2, 3);
                    oneOfManyPosition += Vector3.right * UnityEngine.Random.Range(-2, 3);
                    oneOfManyPosition += Vector3.forward * UnityEngine.Random.Range(-2, 3);
                    GameObject prefab = asteroids[ranObj];
                    GameObject newMeteor = Instantiate(prefab, oneOfManyPosition, Quaternion.identity);
                    // fly in a random direction
                    asteroid_movement movement = newMeteor.GetComponent<asteroid_movement>();
                    movement.setDirection(x, y, z);
                }
            }
            hasBeat = false;
        }
    }

    void recieveBeat() {
        hasBeat = true;
    }

    void toggleActive() {
        active = !active;
    }
}
}
