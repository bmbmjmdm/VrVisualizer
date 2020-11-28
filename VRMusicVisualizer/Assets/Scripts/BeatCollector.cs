using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


namespace Assets.Scripts
{
    public static class BeatCollector {
        public static UnityEvent beatEventsLow = new UnityEvent();
        private static int lowListeners = 0;
        public static UnityEvent beatEventsMid = new UnityEvent();
        private static int midListeners = 0;
        public static UnityEvent beatEventsHigh = new UnityEvent();
        private static int highListeners = 0;
        public static UnityEventSubset toggleEvents = new UnityEventSubset();
        private static float lowAv = 0;
        private static float midAv = 0;
        private static float highAv = 0;
        private static float allAv = 0;
        private static float lowMax = 0;
        private static float midMax = 0;
        private static float highMax = 0;
        private static float allMax = 0;
        private static float pastLow = 0;
        private static float pastMid = 0;
        private static float pastHigh = 0;
        private static float pastAll = 0;

        // use these variables to see if theres been a significant change in spectrum percent (useful for prevent jittery animations)
        public static bool lowSig = false;
        public static bool midSig = false;
        public static bool highSig = false;
        public static bool allSig = false;


        public static bool rainBeatBass = false;
        public static bool rainBeatMid = false;
        public static bool rainBeatTreb = false;
        public static bool rainBeatTotal = false;
        public static bool audioBeat = false;
        public static bool detektorBeat = false;
        public static bool bigBeatBass = false;
        public static bool bigBeatMid = false;
        public static bool bigBeatTreb = false;
        public static bool bigBeatTotal = false;
        public static bool spikeBeat = false;
        public static bool verseChange = false;

        // This adds the listener to a "random" frequency bucket, keeping all buckets as even as possible
        // whichever bucket (beatEventsLow, beatEventsMid, beatEventsHigh) has the lowest number of listeners, give this current listener to it and increment its bucket size 1
        public static void registerBeatListener (UnityAction fun) {
            // find lowest bucket size
            int lowest = Math.Min(lowListeners, Math.Min(midListeners, highListeners));
            // find wich bucket that relates to
            if (lowListeners == lowest) {
                // add listener
                beatEventsLow.AddListener(fun);
                //increment bucket size
                lowListeners++;
            }
            else if (midListeners == lowest) {
                beatEventsMid.AddListener(fun);
                midListeners++;
            }
            else if (highListeners == lowest) {
                beatEventsHigh.AddListener(fun);
                highListeners++;
            }
        }

        public static void registerVerseListener (UnityAction fun) {
            toggleEvents.AddListener(fun);
        }

        private static float lowSum = 0f;
        private static int lowNum = 0;

        // low/mid/high/all frequency ranges follow the same formula for setting their current average frequency:
        public static void setLowAv (float i) {
            // first, store the previous average freqency amplitude as a percent of 0.0 to 1.0
            float tempPastLow = getLowPer();
            // set our current raw average
            lowAv = i;
            // keep track of the highest average we've seen
            if (lowAv > lowMax) {
                lowMax = lowAv;
            }
            // now take the difference between our past frequency average percent and our current (new) one
            // add it to all the previous ones we've taken and how many we've taken
            lowSum += Math.Abs(tempPastLow - getLowPer());
            lowNum ++;
            // compare our current difference to the average of all previous differences
            lowSig = Math.Abs(tempPastLow - getLowPer()) > lowSum/lowNum;
            // if we're higher, this is a significant change in average amplitude
            if (lowSig) pastLow = tempPastLow;
        }

        private static float midSum = 0f;
        private static int midNum = 0;

        public static void setMidAv (float i) {
            float tempPastMid = getMidPer();
            midAv = i;
            if (midAv > midMax) {
                midMax = midAv;
            }
            midSum += Math.Abs(tempPastMid - getMidPer());
            midNum ++;
            midSig = Math.Abs(tempPastMid - getMidPer()) > midSum/midNum;
            if (midSig) pastMid = tempPastMid;
        }

        private static float highSum = 0f;
        private static int highNum = 0;

        public static void setHighAv (float i) {
            float tempPastHigh = getHighPer();
            highAv = i;
            if (highAv > highMax) {
                highMax = highAv;
            }
            highSum += Math.Abs(tempPastHigh - getHighPer());
            highNum ++;
            highSig = Math.Abs(tempPastHigh - getHighPer()) > highSum/highNum;
            if (highSig) pastHigh = tempPastHigh;
        }

        private static float allSum = 0f;
        private static int allNum = 0;

        public static void setAllAv (float i) {
            float tempPastAll = getAllPer();
            allAv = i;
            if (allAv > allMax) {
                allMax = allAv;
            }
            allSum += Math.Abs(tempPastAll - getAllPer());
            allNum ++;
            allSig = Math.Abs(tempPastAll - getAllPer()) > allSum/allNum;
            if (allSig) pastAll = tempPastAll;
        }

        // use this to make sure our average differences never get too stuck in their ways
        // this makes sure we can adapt to changing songs/volumes
        public static void resetAverageDifferences () {
            allSum = allSum/allNum;
            allNum = 1;
            lowSum = lowSum/lowNum;
            lowNum = 1;
            midSum = midSum/midNum;
            midNum = 1;
            highSum = highSum/highNum;
            highNum = 1;
        }

        // use this to make sure our average maxes dont get stale
        // this makes sure we can adapt to changing songs/volumes
        // we only reduce by 1.33 since we currently do this every X seconds, which isn't smart enough to make big changes (can divide by 2 if we can detect song/volum change)
        public static void resetAverageMaxes () {
            allMax = allMax/2f;
            lowMax = lowMax/2f;
            midMax = midMax/2f;
            highMax = highMax/2f;
        }

        public static void songChange() {
            resetAverageDifferences();
            resetAverageMaxes();
        }

        // for effects that ebb in real time with the spectrum, we supply the current spectrum average as a percent of the max it can be
        // we determine the max simply by looking at our past and seeing what the user's max ever has been
        // this means the program learns the user's max volume/spectrum amplitude average over time
        // this works with the user changing their volume up, but NOT them changing it down! TODO warn user at start of application
        public static float getLowPer () {
            if (lowMax == 0) return 0;
            return lowAv/lowMax;
        }

        public static float getMidPer () {
            if (midMax == 0) return 0;
            return midAv/midMax;
        }

        public static float getHighPer () {
            if (highMax == 0) return 0;
            return highAv/highMax;
        }

        public static float getAllPer () {
            if (allMax == 0) return 0;
            return allAv/allMax;
        }
    }
}
