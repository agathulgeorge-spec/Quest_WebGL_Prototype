using System.Runtime.InteropServices;
using UnityEngine;

public class MobileHUD : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern int IsMobileBrowser();
#endif

    private void Awake()
    {
        bool mobile = IsMobileDevice();

        gameObject.SetActive(mobile);
    }

    private bool IsMobileDevice()
    {
#if UNITY_EDITOR

        // Show mobile HUD while testing in Unity
        return true;

#elif UNITY_ANDROID || UNITY_IOS

        return true;

#elif UNITY_WEBGL

        return IsWebGLMobile();

#else

        return false;

#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR

    private bool IsWebGLMobile()
    {
        return IsMobileBrowser() == 1;
    }

#endif
}