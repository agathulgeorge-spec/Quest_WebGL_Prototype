using UnityEngine;

public class DoorUnlockManager : MonoBehaviour
{
    public GameObject lockedBlock;

    private bool key2Collected = false;
    private bool key3Collected = false;

    public void CollectKey2()
    {
        key2Collected = true;
        CheckUnlock();
    }

    public void CollectKey3()
    {
        key3Collected = true;
        CheckUnlock();
    }

    private void CheckUnlock()
    {
        if (key2Collected && key3Collected)
        {
            if (lockedBlock != null)
            {
                lockedBlock.SetActive(false);
            }
        }
    }
}