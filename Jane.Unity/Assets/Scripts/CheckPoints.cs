using UnityEngine;

/// <summary>
/// This Script is for Managing The Array of CheckPoints.
/// Index Starts at N and decreases until 0.
/// </summary>
public class CheckPoints : MonoBehaviour
{
    [SerializeField] public GameObject[] checkPointArr;
    [SerializeField] private AudioSource audioSource;
    //[SerializeField] private AudioClip activateSFX;
    [SerializeField] private AudioClip gateClearSFX;    // Used for user Feedback

    private int idx = 0;
    private int gateCount;
    public GameObject currGate;
    public GameObject nextGate;

    // Start is called before the first frame update
    void Start()
    {
        idx = 0;
        gateCount = checkPointArr.Length;
        currGate= checkPointArr[idx];
        currGate.SendMessage("Activate");

        for (int k = 1; k  < gateCount ; k++)
        {
            checkPointArr[k].SendMessage("Deactivate");
        }
    }

    public void ControlGates()
    {
        audioSource.clip = gateClearSFX;
        audioSource.Play();
        
        currGate = checkPointArr[idx];
        currGate.SendMessage("Deactivate");

        if (idx != gateCount - 1)
        {
            nextGate= checkPointArr[idx + 1];
            nextGate.SendMessage("Activate");
        }

        idx++;
    }
}
