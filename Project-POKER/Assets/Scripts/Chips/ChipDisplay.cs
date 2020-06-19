using UnityEngine;
using UnityEngine.UI;

public class ChipDisplay : MonoBehaviour
{
    public Chip chip;
    [HideInInspector]
    public float value;

    private void Start()
    {
        if (chip != null)
        {
            value = chip.value;

            GetComponent<Image>().sprite = chip.image;
        }
    }

    public void SetChip(Chip _chip)
    {
        chip = _chip;

        value = chip.value;

        GetComponent<Image>().sprite = chip.image;
    }
}
