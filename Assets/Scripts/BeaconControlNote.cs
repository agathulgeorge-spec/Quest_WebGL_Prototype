using UnityEngine;
using TMPro;

public class BeaconControlNote : MonoBehaviour
{
    public GameObject noteObject;
    public float displayTime = 4f;

    private bool hasShown = false;
    private float timer = 0f;


    private void Start()
    {
        if (noteObject != null)
        {
            noteObject.SetActive(false);
        }
    }


    public void ShowNote()
    {
        // Don't show again.
        if (hasShown)
            return;

        // Don't show on mobile.
        if (Application.isMobilePlatform)
            return;

        hasShown = true;

        if (noteObject != null)
        {
            noteObject.SetActive(true);
            timer = displayTime;
        }
    }


    private void Update()
    {
        if (!hasShown || noteObject == null)
            return;

        if (!noteObject.activeSelf)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            noteObject.SetActive(false);
        }
    }
}