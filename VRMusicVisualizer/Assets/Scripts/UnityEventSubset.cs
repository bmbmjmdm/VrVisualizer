using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
     
namespace Assets.Scripts{
    public class UnityEventSubset {
        private ArrayList arrayList = new ArrayList();
        private int curActive = -1;

        public void AddListener(UnityAction fun) {
            arrayList.Add(fun);
        }

        public void RemoveListener(UnityAction fun) {
            arrayList.Remove(fun);
        }
        
        // old behavior, where we take an int i and turn on/off i random listeners
        // here is where this class diverges from UnityEvent
        // invoke takes a number and will invoke a random subset of arrayList, equal to i (or less if it doesn't have that many)
        public void Invoke(int i) {
            int count = arrayList.Count;
            if (i >= count) {
                // invoke all
                foreach ( UnityAction fun in arrayList )
                    fun.Invoke();
            }
            else {
                // randomize
                Shuffle();
                // invoke first i
                for (int index = 0; index < i; index++) {
                    ((UnityAction) arrayList[index]).Invoke();
                }
            }
        }

        // new behavior where we turn off the current active one and turn on 1 random new listener
        public void Invoke() {
            // turn off old one
            if (curActive > -1) {
                ((UnityAction) arrayList[curActive]).Invoke();
            }

            // select a new one, making sure we dont select the same one twice
            int newOne = curActive;
            while (newOne == curActive) {
                newOne = UnityEngine.Random.Range(0, arrayList.Count);
            }
            curActive = newOne;
            // turn on new one
            ((UnityAction) arrayList[newOne]).Invoke();
        }

        private void Shuffle()  
        {  
            System.Random rng = new System.Random();  
            int n = arrayList.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                UnityAction value = (UnityAction) arrayList[k];  
                arrayList[k] = arrayList[n];  
                arrayList[n] = value;  
            }  
        }

    }
}