using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpeedGate : MonoBehaviour
{
    [SerializeField] private VisualEffect warpVFX;
    public GameObject warpDestination;
    private GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject;
            StartCoroutine(Warp());
        }
    }

    IEnumerator Warp()
    {
/*        player.GetComponent<SpaceshipController>
*/        if(warpVFX != null)
        {
            Instantiate(warpVFX);
            // wait for visual effect to end
        }
        player.transform.position = warpDestination.transform.position;
        yield return null;
    }
}