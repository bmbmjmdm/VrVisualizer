using UnityEngine;
using System.Collections;

public class lb_Bird_simple : MonoBehaviour {
	Animator anim;
	int flyAnimationHash;
	int flyingBoolHash;

    void Start() {
		anim = gameObject.GetComponent<Animator>();
		anim.applyRootMotion = false;
		anim.SetBool (flyingBoolHash,true);
		flyAnimationHash = Animator.StringToHash ("Base Layer.fly");
		anim.Play(flyAnimationHash);
	}

    void Update() {

    }
}
