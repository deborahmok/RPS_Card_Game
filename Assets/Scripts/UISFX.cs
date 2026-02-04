using UnityEngine;

public class UISFX : MonoBehaviour
{
    public AudioSource source;
    public AudioClip loseClip, clickClip, winClip, tieClip;

    public void PlayLose()
    {
        if (source != null && loseClip != null)
        {
            source.PlayOneShot(loseClip);
        }
    }
    public void PlayClick()
    {
        if (source != null && clickClip != null)
        {
            source.PlayOneShot(clickClip);
        }
    }
    public void PlayWin()
    {
        if (source != null && winClip != null)
        {
            source.PlayOneShot(winClip);
        }
    }
    public void PlayTie()
    {
        if (source != null && tieClip != null)
        {
            source.PlayOneShot(tieClip);
        }
    }
}