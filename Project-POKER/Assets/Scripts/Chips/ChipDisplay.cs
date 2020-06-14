using UnityEngine;
using UnityEngine.UI;

public class ChipDisplay : MonoBehaviour
{
    public Chip chip;
    [HideInInspector]
    public float percentageValue;

    private void Start()
    {
        percentageValue = chip.percentageValue;

        GetComponent<Image>().sprite = chip.image;
    } 
}
