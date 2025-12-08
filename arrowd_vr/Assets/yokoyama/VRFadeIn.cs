using UnityEngine;
using Valve.VR;

public class VRFadeIn : MonoBehaviour
{
    void Start()
    {
        // ‘¦À‚É‰æ–Ê‚ğ–¾‚é‚­‚·‚é
        SteamVR_Fade.Start(Color.clear, 0f);

        // ”O‰Ÿ‚µ‚Å­‚µ’x‚ê‚Ä‚à‚¤ˆê“x
        Invoke("ForceClear", 0.1f);
        Invoke("ForceClear", 0.3f);
        Invoke("ForceClear", 0.5f);
    }

    void ForceClear()
    {
        SteamVR_Fade.Start(Color.clear, 0f);
    }
}