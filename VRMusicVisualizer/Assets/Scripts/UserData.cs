using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Assets.Scripts {

    public class UserData : MonoBehaviour 
    {
        void Start() {
          if (File.Exists(Application.persistentDataPath + "/savedEnvironment.vrmv")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedEnvironment.vrmv", FileMode.Open);
            BeatCollector.setSelectedEffectsFromLoad( (ArrayList) bf.Deserialize(file) );
            file.Close();
          }
          if (File.Exists(Application.persistentDataPath + "/savedIntro.vrmv")) {
            BinaryFormatter bf2 = new BinaryFormatter();
            FileStream file2 = File.Open(Application.persistentDataPath + "/savedIntro.vrmv", FileMode.Open);
            setIntroProgress( (SerializedIntroProgress) bf2.Deserialize(file2) );
            file2.Close();
          }
        }

        void Update() {
          if (BeatCollector.needSave) {
            BeatCollector.needSave = false;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create (Application.persistentDataPath + "/savedEnvironment.vrmv");
            bf.Serialize(file, BeatCollector.getSelectedEffectsForSave());
            file.Close();

            BinaryFormatter bf2 = new BinaryFormatter();
            FileStream file2 = File.Create (Application.persistentDataPath + "/savedIntro.vrmv");
            bf2.Serialize(file2, getIntroProgress());
            file2.Close();
          }
        }

        SerializedIntroProgress getIntroProgress() {
          SerializedIntroProgress sip = new SerializedIntroProgress();
          sip.needsMusic = IntroProgress.needsMusic;
          sip.needsGrip = IntroProgress.needsGrip;
          sip.needsRT = IntroProgress.needsRT;
          sip.needsLT = IntroProgress.needsLT;
          sip.needsLT_2 = IntroProgress.needsLT_2;
          sip.needsButton1 = IntroProgress.needsButton1;
          sip.needsButton2 = IntroProgress.needsButton2;
          sip.needsButton1_2 = IntroProgress.needsButton1_2;
          sip.needsButton2_2 = IntroProgress.needsButton2_2;
          sip.needsButton3 = IntroProgress.needsButton3;
          sip.needsButton4 = IntroProgress.needsButton3;
          sip.needsFinish = IntroProgress.needsFinish;
          sip.completedIntro = IntroProgress.completedIntro;
          return sip;
        }

        void setIntroProgress(SerializedIntroProgress sip) {
          IntroProgress.needsMusic = sip.needsMusic;
          IntroProgress.needsGrip = sip.needsGrip;
          IntroProgress.needsRT = sip.needsRT;
          IntroProgress.needsLT = sip.needsLT;
          IntroProgress.needsLT_2 = sip.needsLT_2;
          IntroProgress.needsButton1 = sip.needsButton1;
          IntroProgress.needsButton2 = sip.needsButton2;
          IntroProgress.needsButton1_2 = sip.needsButton1_2;
          IntroProgress.needsButton2_2 = sip.needsButton2_2;
          IntroProgress.needsButton3 = sip.needsButton3;
          IntroProgress.needsButton4 = sip.needsButton3;
          IntroProgress.needsFinish = sip.needsFinish;
          IntroProgress.completedIntro = sip.completedIntro;
        }
    }

}