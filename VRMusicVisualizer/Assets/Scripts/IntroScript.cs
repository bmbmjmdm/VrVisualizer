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

    // Start is called before the first frame update
    void Start()
    {
      handLeft = (Hand) GameObject.Find("LeftHand").GetComponent<Hand>();
      handRight = (Hand) GameObject.Find("RightHand").GetComponent<Hand>();
      BeatCollector.registerBeatListener(recieveBeat);
    }

    // Update is called once per frame
    void Update()
    {
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
        text.text = "Here you can add/subtract effects to make your own environment. Press the indicated button to cycle through effects!";
      }
      // they brought up an effect, now get them to confirm it
      else if (IntroProgress.needsButton2) {
        hideAllHints();
        ControllerButtonHints.ShowButtonHint(handRight, confirmAdd);
        text.text = "When you found one you like, press the indicated button to confirm it.";
      }
      // they confirmed an effect, now get them to add another
      else if (IntroProgress.needsButton1_2) {
        hideAllHints();
        ControllerButtonHints.ShowButtonHint(handRight, addEffect);
        text.text = "Try adding one more effect! Use the indicated button to cycle through effects.";
      }
      // they added another, now get them to confirm it
      else if (IntroProgress.needsButton2_2) {
        hideAllHints();
        ControllerButtonHints.ShowButtonHint(handRight, confirmAdd);
        text.text = "Press the indicated button to confirm it.";
      }
      // they confirmed a second effect, now get them to cycle through removing
      else if (IntroProgress.needsButton3) {
        hideAllHints();
        ControllerButtonHints.ShowButtonHint(handLeft, removeEffect);
        text.text = "Great! Now to remove an effect press the indicated button to cycle through effects and preview removing them.";
      }
      // they previewed removing an effect, now get them to confirm it
      else if (IntroProgress.needsButton4) {
        hideAllHints();
        ControllerButtonHints.ShowButtonHint(handLeft, confirmRemove);
        text.text = "When you're happy with the one you want to remove, press the indicated button to confirm.";
      }
      // they confirmed removing an effect
      else if (IntroProgress.needsFinish) {
        hideAllHints();
        text.text = "If you ever want to cancel adding/removing effects, just press the opposite button (remove vs add) or switch scenes! Have fun!";
      }
      else {
        text.text = "";
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
}
}
