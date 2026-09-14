using UnityEngine;

public class MobileLandscapeWebGL : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        Screen.orientation = ScreenOrientation.LandscapeLeft;
#endif
    }
}