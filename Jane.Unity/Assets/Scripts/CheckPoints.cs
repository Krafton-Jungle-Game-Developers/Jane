using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

/// <summary>
/// This Script is for Managing The Array of CheckPoints.
/// Index Starts at N and decreases until 0.
/// </summary>
public class CheckPoints : MonoBehaviour
{
    [SerializeField] private GameObject[] checkPointArr;
    private int idx = 0;
    private GameObject currGate;
    private GameObject nextGate;

    // Start is called before the first frame update
    void Start()
    {
        idx = checkPointArr.Length - 1;
        currGate= checkPointArr[idx];
        currGate.SendMessage("Activate");

        for (int k = idx - 1; k >= 0; k--)
        {
            checkPointArr[k].SendMessage("Deactivate");
        }
    }

    public void ControlGates()
    {
        currGate = checkPointArr[idx];
        currGate.SendMessage("Deactivate");

        if (idx != 0)
        {
            nextGate= checkPointArr[idx - 1];
            nextGate.SendMessage("Activate");
        }

        idx--;
    }
}
