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
        }

        void Update() {
          if (BeatCollector.needSave) {
            BeatCollector.needSave = false;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create (Application.persistentDataPath + "/savedEnvironment.vrmv");
            bf.Serialize(file, BeatCollector.getSelectedEffectsForSave());
            file.Close();
          }
        }
    }

}