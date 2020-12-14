using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class FireflyReaction : MonoBehaviour
{
    public ParticleSystem particleEffect;
    public int numBunches;
    private ParticleSystem[] realObjs;
    public bool active = true;
    private bool destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerVerseListener(toggleActive);
        CreateObjs();
    }

    void CreateObjs() {
        realObjs = new ParticleSystem[numBunches];
        for (int i = 0; i < numBunches; i++) {
            // exclusive so dont have to do Length-1
            Vector3 t = new Vector3();
            t += Vector3.right * UnityEngine.Random.Range(-150.0f, 150.0f);
            t += Vector3.forward * UnityEngine.Random.Range(-150.0f, 150.0f);
            t += Vector3.up * UnityEngine.Random.Range(0.5f, 5.0f);
            realObjs[i] = (ParticleSystem) Instantiate(particleEffect, t, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // we're active
        if (active) {
            destroyed = false;
            // create a burst proportional to the current frequency aplitude percent
            for (int i = 0; i < numBunches; i++) {
                ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
                realObjs[i].Emit(emitOverride, (int) (BeatCollector.getHighPer() * 2));
            }
        }
        // we're not active
        else {
            if (destroyed) return;
            // turn our particle emission off
            for (int i = 0; i < numBunches; i++) {
                ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
                realObjs[i].Emit(emitOverride, 0);
            }
            destroyed = true;
        }
    }

    void toggleActive() {
        active = !active;
    }
}
}
