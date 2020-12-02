using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class CrossBeats : MonoBehaviour
    {
        public GameObject Prefab;
        private bool clearBeats = false;
        private int cooldownLow = 0;
        private int cooldownMid = 0;
        private int cooldownHigh = 0;

        void Start()
        {
        }

        void Update() {
            assessVerse();
            assessBeats();
        }

        // assessVerse actively watches a single spectrum analyzor, VerseChange, to see if the verse has changed
        // if so, toggle current effect off and a random effect on
        void assessVerse() {
            if (BeatCollector.verseChange) {
                BeatCollector.toggleEvents.Invoke();
                BeatCollector.verseChange = false;
            }
        }


        // assessBeats actively watches BeatCollector, which accumulates beats from the various spectrum analyzors (often called XSpectrum), and assesses the data
        // If it has enough beats in a bucket, it will invoke all listeners for that bucket
        // This invokation has a cooldown
        // It clears BeatCollector of beats every other frame
        void assessBeats() {
            int numLow = 0;
            int numMid = 0;
            int numHigh = 0;
            int numAll = 0;
            if (BeatCollector.rainBeatBass) numLow++;
            if (BeatCollector.rainBeatMid) numMid++;
            if (BeatCollector.rainBeatTreb) numHigh++;
            if (BeatCollector.rainBeatTotal) numAll++;
            if (BeatCollector.audioBeat) numAll++;
            if (BeatCollector.detektorBeat) numAll++;
            if (BeatCollector.bigBeatBass) numLow+=3;
            if (BeatCollector.bigBeatMid) numMid+=3;
            if (BeatCollector.bigBeatTreb ) numHigh+=3;
            if (BeatCollector.bigBeatTotal) numAll+=3;
            if (BeatCollector.spikeBeat) numAll++;
            numLow = numLow + numAll;
            numMid = numMid + numAll;
            numHigh = numHigh + numAll;

            if (numLow > 3 && cooldownLow <= 0) {
                BeatCollector.beatEventsLow.Invoke();
                cooldownLow = 30;
            }
            else {
                cooldownLow--;
            }

            if (numMid > 3 && cooldownMid <= 0) {
                BeatCollector.beatEventsMid.Invoke();
                cooldownMid = 30;
            }
            else {
                cooldownMid--;
            }

            if (numHigh > 3 && cooldownHigh <= 0) {
                BeatCollector.beatEventsHigh.Invoke();
                cooldownHigh = 30;
            }
            else {
                cooldownHigh--;
            }

            
            if (clearBeats) {
                clearBeats = false;
                resetBeats();
            }
            else {
                clearBeats = true;
            }
        }
        void resetBeats() {
            BeatCollector.rainBeatBass = false;
            BeatCollector.rainBeatMid = false;
            BeatCollector.rainBeatTreb = false;
            BeatCollector.rainBeatTotal = false;
            BeatCollector.audioBeat = false;
            BeatCollector.detektorBeat = false;
            BeatCollector.spikeBeat = false;
            BeatCollector.bigBeatBass = false;
            BeatCollector.bigBeatMid = false;
            BeatCollector.bigBeatTreb = false;
            BeatCollector.bigBeatTotal = false;
        }
    }
}
