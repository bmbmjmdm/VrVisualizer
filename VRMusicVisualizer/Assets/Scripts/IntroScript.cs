using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Assets.Scripts
{
public class IntroScript : MonoBehaviour
{
    public Text text;
    private bool hasBeat = false;
    public SteamVR_Action_Boolean addEffect;
    public SteamVR_Action_Boolean confirmAdd;
    public SteamVR_Action_Boolean removeEffect;
    public SteamVR_Action_Boolean confirmRemove;
    private Hand handLeft; 
    private Hand handRight;
    private bool doneIntro = false;
    private string controller_type = "wmr_holographic";
    private float clock = 0f;

    // Start is called before the first frame update
    void Start()
    {
      handLeft = (Hand) GameObject.Find("LeftHand").GetComponent<Hand>();
      handRight = (Hand) GameObject.Find("RightHand").GetComponent<Hand>();
      BeatCollector.registerBeatListener(recieveBeat);
      
      // MY OWN CUSTOM CODE TO FIND THE CONTROLLER MODEL
      var system = OpenVR.System;
      string[] valid_controllers = {"knuckles", "oculus_touch", "vive_cosmos_controller", "vive_controller"};
      for (int i = 0; i < 100; i++) {
        var error = ETrackedPropertyError.TrackedProp_Success;
        var capacity = system.GetStringTrackedDeviceProperty((uint)i, ETrackedDeviceProperty.Prop_ExpectedControllerType_String, null, 0, ref error);
        if (capacity > 1)
        {
            var buffer = new System.Text.StringBuilder((int)capacity);
            system.GetStringTrackedDeviceProperty((uint)i, ETrackedDeviceProperty.Prop_ExpectedControllerType_String, buffer, capacity, ref error);
            try {
              var s = buffer.ToString();
              if (((IList)valid_controllers).Contains(s)) {
                controller_type = s;
              }
            }
            catch {
              // ignore errors
            }
        }
      }
    }

    // Update is called once per frame
    void Update()
    {
      // controller unsupported, skip intro
      if (!isControllerSupported()) {
        if (clock < 10f) {
          clock += Time.deltaTime;
          IntroProgress.completedIntro = true;
          text.text = "It looks like you're using an unsupported controller. Most functionality will be unavailable. If you would like, please make your own controller mapping in the Steam menu, or refund the game.";
        }
        else {
          text.text = "";
        }
      }

      // controller is supported, go through intro
      else {
        if (doneIntro) return;
        // needs to turn on music
        if (IntroProgress.needsMusic) {
          if (hasBeat) {
            // this is how all of these booleans will be set. They're all false except the current step. The rest of these will be set in the UserInput script
            IntroProgress.needsMusic = false;
            IntroProgress.needsGrip = true;
          }
          else {
            text.text = "Welcome! First, turn on some music! I'll pick it up from your speakers.";
          }
        }
        // turned on music, now needs to use the grip
        else if (IntroProgress.needsGrip) {
          text.text = "Nice music! Now try pressing the Grip Button to \"make\" a beat. Usually you don't need this, but who knows?";
        }
        // used grip, now needs to try the right trigger
        else if (IntroProgress.needsRT) {
          text.text = "Good job. Next try changing the environment by pressing the Right Trigger.";
        }
        // used right trigger, now needs to use left
        else if (IntroProgress.needsLT) {
          text.text = "Cool, there are 5 environments: Space, Nature, Neon, Random, and Custom. Now go back using the Left Trigger.";
        }
        // used left trigger, now needs to use left once more
        else if (IntroProgress.needsLT_2) {
          text.text = "Now use Left Trigger once more to get to the Custom environment! You could also use Right Trigger four times, but let's not do that now.";
        }
        // used left trigger again, they're on the custom screen, now needs to use button 1
        else if (IntroProgress.needsButton1) {
          hideAllHints();
          ControllerButtonHints.ShowButtonHint(handRight, addEffect);
          text.text = "Here you can add/subtract effects to make your own environment. Press the " + getCycleAddButton() + " to cycle through effects!";
        }
        // they brought up an effect, now get them to confirm it
        else if (IntroProgress.needsButton2) {
          hideAllHints();
          ControllerButtonHints.ShowButtonHint(handRight, confirmAdd);
          text.text = "When you found one you like, press the " + getConfirmAddButton() + " to confirm it.";
        }
        // they confirmed an effect, now get them to add another
        else if (IntroProgress.needsButton1_2) {
          hideAllHints();
          ControllerButtonHints.ShowButtonHint(handRight, addEffect);
          text.text = "Try adding one more effect! Use the " + getCycleAddButton() + " to cycle through effects.";
        }
        // they added another, now get them to confirm it
        else if (IntroProgress.needsButton2_2) {
          hideAllHints();
          ControllerButtonHints.ShowButtonHint(handRight, confirmAdd);
          text.text = "Press the " + getConfirmAddButton() + " to confirm it.";
        }
        // they confirmed a second effect, now get them to cycle through removing
        else if (IntroProgress.needsButton3) {
          hideAllHints();
          ControllerButtonHints.ShowButtonHint(handLeft, removeEffect);
          text.text = "Great! Now to remove an effect press the " + getCycleRemoveButton() + " to cycle through effects and preview removing them.";
        }
        // they previewed removing an effect, now get them to confirm it
        else if (IntroProgress.needsButton4) {
          hideAllHints();
          ControllerButtonHints.ShowButtonHint(handLeft, confirmRemove);
          text.text = "When you're happy with the one you want to remove, press the " + getConfirmRemoveButton() + " to confirm.";
        }
        // they confirmed removing an effect
        else if (IntroProgress.needsFinish) {
          hideAllHints();
          text.text = "If you ever want to cancel adding/removing effects, just press the opposite button (remove vs add), switch scenes, or press the beat button! Have fun!";
        }
        else {
          text.text = "";
          doneIntro = true;
        }
      }
    }

    void hideAllHints() {
      ControllerButtonHints.HideAllButtonHints(handRight);
      ControllerButtonHints.HideAllButtonHints(handLeft);
      ControllerButtonHints.HideAllTextHints(handRight);
      ControllerButtonHints.HideAllTextHints(handLeft);
    }

    void recieveBeat() {
      hasBeat = true;
    }

    string getCycleAddButton() {
      if (controller_type == "knuckles") {
        return "B button on the right controller";
      }
      else if (controller_type == "oculus_touch" || controller_type == "vive_cosmos_controller") {
        return "B button (right controller)";
      }
      else if (controller_type == "vive_controller" || controller_type == "wmr_holographic") {
        return "Menu button on the right controller";
      }
      else {
        return "";
      }
    }

    string getConfirmAddButton() {
      if (controller_type == "knuckles") {
        return "A button on the right controller";
      }
      else if (controller_type == "oculus_touch" || controller_type == "vive_cosmos_controller") {
        return "A button (right controller)";
      }
      else if (controller_type == "vive_controller" || controller_type == "wmr_holographic") {
        return "D-Pad Down Direction on the right controller";
      }
      else {
        return "";
      }
    }

    string getCycleRemoveButton() {
      if (controller_type == "knuckles") {
        return "B button on the left controller";
      }
      else if (controller_type == "oculus_touch" || controller_type == "vive_cosmos_controller") {
        return "Y button (left controller)";
      }
      else if (controller_type == "vive_controller" || controller_type == "wmr_holographic") {
        return "Menu button on the left controller";
      }
      else {
        return "";
      }
    }

    string getConfirmRemoveButton() {
      if (controller_type == "knuckles") {
        return "A button on the left controller";
      }
      else if (controller_type == "oculus_touch" || controller_type == "vive_cosmos_controller") {
        return "X button (left controller)";
      }
      else if (controller_type == "vive_controller" || controller_type == "wmr_holographic") {
        return "D-Pad Down Direction on the left controller";
      }
      else {
        return "";
      }
    }

    bool isControllerSupported() {
      if (controller_type == "knuckles") {
        return true;
      }
      else if (controller_type == "oculus_touch" || controller_type == "vive_cosmos_controller") {
        return true;
      }
      else if (controller_type == "vive_controller" || controller_type == "wmr_holographic") {
        return true;
      }
      else {
        return false;
      }
    }
  }
}
