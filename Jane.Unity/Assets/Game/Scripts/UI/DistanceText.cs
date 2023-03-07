using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DistanceText : MonoBehaviour
{
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private TMP_Text nameText;
    private float distance;
    private float targetBoxScale = 1000f;
    private Transform player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    private void UpdateDistance(Vector3 objectPos)
    {
        distance = Vector3.Distance(player.position, objectPos);
        float scale = targetBoxScale / distance;

        ScaleAlpha(scale);
        ScaleSize(scale);

        distance = Mathf.RoundToInt(distance);
        distanceText.text = $"{distance}m";
    }
    private void SetName(string name)
    {
        nameText.text = $"{name}";
    }

    private void ScaleAlpha(float scale)
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        Image[] images = GetComponentsInChildren<Image>();
        scale = Mathf.Pow(scale, 2);

        foreach (TMP_Text text in texts)
        {
            text.alpha = scale;
        }
        foreach (Image image in images)
        {
            Color tmp = image.color;
            tmp.a = scale;
            image.color = tmp;
        }
    }

    private void ScaleSize(float scale)
    {
        this.transform.localScale = new Vector3(scale, scale, scale);
    }
}
