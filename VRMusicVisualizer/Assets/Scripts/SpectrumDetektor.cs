using UnityEngine;
using System.Collections;
using Assets.WasapiAudio.Scripts.Unity;
     
namespace Assets.Scripts{
     public class SpectrumDetektor : AudioVisualizationEffect {
         
         //public GameObject cube;
         private bool Beated;
         private float[] historyBuffer = new float[43];
         private float[] channelRight;
         private float[] channelLeft;
         private int SamplesSize = 1024;
         public float InstantSpec;
         public float AverageSpec;
         public float VarianceSum;
         public float Variance;
         public GameObject Prefab;
         
         // Use this for initialization
         void Start () {
             Beated = false;
         }
         
         // Update is called once per frame
         void Update () {
             
             InstantSpec = sumStereo2(GetSpectrumData ());  //Rafa
             
             AverageSpec = sumLocalEnergy2(historyBuffer) / historyBuffer.Length;  //Rafa
             
             Variance = VarianceAdder(historyBuffer) / historyBuffer.Length;  //Rafa
             
             float[] shiftingHistoryBuffer = new float[historyBuffer.Length]; // make a new array and copy all the values to it
             
             for (int i = 0; i < (historyBuffer.Length - 1); i++) { // now we shift the array one slot to the right
                 shiftingHistoryBuffer[i+1] = historyBuffer[i]; // and fill the empty slot with the new instant sound energy
             }
             
             shiftingHistoryBuffer [0] = InstantSpec;
             
             for (int i = 0; i < historyBuffer.Length; i++) {
                 historyBuffer[i] = shiftingHistoryBuffer[i]; //then we return the values to the original array
             }
             
             if (InstantSpec > AverageSpec - Variance) { // now we check if we have a beat
                 if(!Beated) {
                     BeatCollector.detektorBeat = true;
                     Beated = true;
                 }
             } 
             else {
                 //Debug.Log(InstantSpec);
                 if(Beated) {
                     Beated = false;
                 }
                 //Debug.Log("No Beat");
             }
         }
         
         float sumStereo(float[] channel1, float[] channel2) {
             float e = 0;
             for (int i = 0; i<channel1.Length; i++) {
                 e += ((channel1[i]*channel1[i]) + (channel2[i]*channel2[i]));
             }
             
             return e;
         }
     
         float sumStereo2(float[] Channel) {
             float e = 0;
             for (int i = 0; i < Channel.Length; i++) {
                 float ToSquare = Channel[i];
                 e += (ToSquare * ToSquare);
             }
             return e;
         }
         
         float sumLocalEnergy() {
             float E = 0;
             
             for (int i = 0; i<historyBuffer.Length; i++) {
                 E += historyBuffer[i]*historyBuffer[i];
             }
             
             return E;
         }
     
         float sumLocalEnergy2(float[] Buffer) {
             float E = 0;
             for (int i = 0; i < Buffer.Length; i++) {
                 E += Buffer[i];
             }
             return E;
         }
     
         float VarianceAdder (float[] Buffer) {
             float VarSum = 0;
             for (int i = 0; i < Buffer.Length; i++) {  //Rafa
                 float ToSquare = Buffer[i] - AverageSpec;
                 VarSum += (ToSquare * ToSquare);
             }
             return VarSum;
         }
         
         string historybuffer() {
             string s = "";
             for (int i = 0; i<historyBuffer.Length; i++) {
                 s += (historyBuffer[i] + ",");
             }
             return s;
         }
     }
}