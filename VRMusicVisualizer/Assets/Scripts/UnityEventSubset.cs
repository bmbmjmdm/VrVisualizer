using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
     
namespace Assets.Scripts{
    public class UnityEventSubset {
        private ArrayList arrayList = new ArrayList();

        public void AddListener(UnityAction fun) {
            arrayList.Add(fun);
        }

        public void RemoveListener(UnityAction fun) {
            arrayList.Remove(fun);
        }
        
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