using UnityEngine;
using System.Collections;
using Unity.XR.CoreUtils;

public class RecenterPlayer : MonoBehaviour
{
    public XROrigin xrOrigin;
    public Transform pianoSpawnPoint;

    void Start()
    {
        if (xrOrigin != null && pianoSpawnPoint != null)
        {
            StartCoroutine(WaitAndRecenter());
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && xrOrigin != null && pianoSpawnPoint != null)
        {
            DoRecenter();
        }
    }

    IEnumerator WaitAndRecenter()
    {
        yield return new WaitForSeconds(0.2f);
        DoRecenter();
    }

    public void DoRecenter()
    {
        xrOrigin.MoveCameraToWorldLocation(pianoSpawnPoint.position);
        xrOrigin.MatchOriginUpCameraForward(pianoSpawnPoint.up, pianoSpawnPoint.forward);
        
        Debug.Log("<color=green>[XR]</color> Recentrado automático aplicado.");
    }
}