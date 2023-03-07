using UnityEngine;

/// <summary>
/// This Script is for Managing The Array of CheckPoints.
/// Index Starts at N and decreases until 0.
/// </summary>
public class CheckPoints : MonoBehaviour
{
    [SerializeField] public GameObject[] checkPointArr;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gateClearSFX;    // Used for user Feedback
    public TargetBoxGenerator targetBoxGenerator;

    public int idx = 0;
    public GameObject nextGate;
    public bool goalActive = false;
    private int gateCount;

    // Start is called before the first frame update
    void Start()
    {
        gateCount = checkPointArr.Length;
        nextGate = checkPointArr[idx];
        nextGate.SendMessage("Activate");

        for (int k = 1; k  < gateCount ; k++)
        {
            checkPointArr[k].SendMessage("Deactivate");
        }
    }

    public void ControlGates(int gateNo)
    {
        audioSource.clip = gateClearSFX;
        audioSource.Play();

        nextGate.SendMessage("Deactivate");
        idx += 1;

        if (gateNo != gateCount)
        {
            nextGate= checkPointArr[gateNo];
            nextGate.SendMessage("Activate");
            targetBoxGenerator.SetNextTargetBox(gateNo);
        }
        else
        {
            targetBoxGenerator.ResetTargetBox();
            goalActive = true;
        }
    }
}
