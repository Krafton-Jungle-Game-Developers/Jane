using UnityEngine;

/// <summary>
/// This Script is for Managing The Array of CheckPoints.
/// Index Starts at N and decreases until 0.
/// </summary>
public class CheckPoints : MonoBehaviour
{
    [SerializeField] private GameObject[] checkPointArr;
    private int idx = 0;
    private int gateCount;
    private GameObject currGate;
    private GameObject nextGate;

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
