using System.Collections;
using System.Collections.Generic;
using System; 

namespace Assets.Scripts
{
[Serializable]
public class SerializedIntroProgress
{
  public bool needsMusic = true;
  public bool needsGrip = false;
  public bool needsRT = false;
  public bool needsLT = false;
  public bool needsLT_2 = false;
  public bool needsButton1 = false;
  public bool needsButton2 = false;
  public bool needsButton1_2 = false;
  public bool needsButton2_2 = false;
  public bool needsButton3 = false;
  public bool needsButton4 = false;
  public bool needsFinish = false;
  public bool completedIntro = false;
}
}