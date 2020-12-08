using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
     
namespace Assets.Scripts{
    public class UnityEventSubset {
        private ArrayList arrayList = new ArrayList();
        private int curActive1 = -1;
        private int curActive2 = -1;
        private int curActive3 = -1;
        private int curSelecting = 0;
        private int curDeleting = -1;
        private ArrayList selectedEffects = new ArrayList();

        public void reset () {
            arrayList = new ArrayList();
            curActive1 = -1;
            curActive2 = -1;
            curActive3 = -1;
        }

        public void AddListener(UnityAction fun) {
            arrayList.Add(fun);
        }

        public void RemoveListener(UnityAction fun) {
            arrayList.Remove(fun);
        }

        // invoke type where we turn off the current active ones and turn on 3 random new listener
        public void Invoke() {
            // turn off old ones
            if (curActive1 > -1) {
                ((UnityAction) arrayList[curActive1]).Invoke();
                ((UnityAction) arrayList[curActive2]).Invoke();
                ((UnityAction) arrayList[curActive3]).Invoke();
            }

            // select a new ones
            curActive1 = UnityEngine.Random.Range(0, arrayList.Count);
            curActive2 = UnityEngine.Random.Range(0, arrayList.Count);
            curActive3 = UnityEngine.Random.Range(0, arrayList.Count);
            // make sure none of them are the same one
            while (curActive1 == curActive2 || curActive1 == curActive3 || curActive2 == curActive3) {
                curActive1 = UnityEngine.Random.Range(0, arrayList.Count);
                curActive2 = UnityEngine.Random.Range(0, arrayList.Count);
                curActive3 = UnityEngine.Random.Range(0, arrayList.Count);
            }
            // its possible for the new ones to include the old ones, thats ok
            
            // turn on new one
            ((UnityAction) arrayList[curActive1]).Invoke();
            ((UnityAction) arrayList[curActive2]).Invoke();
            ((UnityAction) arrayList[curActive3]).Invoke();
        }

        //  Invoke type where we turn off the "current selecting" effect and turn on the next one in the arrayList list that isn't in the selectedEffects list
        public void InvokeOrdered(bool firstCall) {
            // we maxed out effects
            if (selectedEffects.Count == arrayList.Count) return;
            
            // turn off old one
            if (!firstCall) {
                ((UnityAction) arrayList[curSelecting]).Invoke();
            }

            // select a new one, making sure we dont go over the edge of the array or select one from our already selected list
            if (!firstCall) {
                curSelecting++;
                if (curSelecting > arrayList.Count-1) curSelecting = 0;
            }
            while (selectedEffects.Contains(curSelecting)) {
                curSelecting++;
                if (curSelecting > arrayList.Count-1) curSelecting = 0;
            }
            
            // turn on new one
            ((UnityAction) arrayList[curSelecting]).Invoke();
        }

        // set the currently selected invoked effect to be a permanant one and stop cycling
        public void FinishInvokedOrdered() {
            // we maxed out effects
            if (selectedEffects.Count == arrayList.Count) return;

            selectedEffects.Add(curSelecting);
        }

        // stop cycling and undo the one were currently looking at
        public void AbandonInvokedOrdered() {
            // we never had a chance to add one in the first place, dont abandon
            if (selectedEffects.Count == arrayList.Count) return;

            // turn off the one we were thinking about keeping
            ((UnityAction) arrayList[curSelecting]).Invoke();
        }

        // this mirrors our InvokeOrdered behavior, but turns off already turned on effects rather than adding new ones
        public void AntiInvokeOrdered(bool firstCall) {
            // we have no effects to turn on
            if (selectedEffects.Count == 0) return;

            // start with the most recently added effect
            if (firstCall) curDeleting = selectedEffects.Count - 1;

            // turn on old one
            if (!firstCall) {
                ((UnityAction) arrayList[(int) selectedEffects[curDeleting]]).Invoke();
            }

            // select a new one, making sure we dont go over the edge of the array
            if (!firstCall) {
                curDeleting--;
                if (curDeleting < 0) curDeleting = selectedEffects.Count - 1;
            }
            // turn off new one
            ((UnityAction) arrayList[(int) selectedEffects[curDeleting]]).Invoke();
        }

        public void FinishAntiInvokedOrdered() {
            // we have no effects
            if (selectedEffects.Count == 0) return;

            selectedEffects.RemoveAt(curDeleting);
        }

        public void AbandonAntiInvokedOrdered() {
            // we never had a chance to remove one in the first place, dont abandon
            if (selectedEffects.Count == 0) return;

            // turn on the one we were thinking about deleting
            ((UnityAction) arrayList[(int) selectedEffects[curDeleting]]).Invoke();
        }

        public ArrayList getSelectedEffectsForSave () {
            return selectedEffects;
        }

        public void setSelectedEffectsFromLoad (ArrayList effects) {
            selectedEffects = effects;
            // turn on all effects listed. we dont have to worry about resetting things because this is loaded at the very beginning 
            foreach (int x in effects)
            {
                 ((UnityAction) arrayList[x]).Invoke();
            }
        }

    }
}