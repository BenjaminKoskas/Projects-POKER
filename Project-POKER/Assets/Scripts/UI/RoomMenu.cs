using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomMenu : MonoBehaviour
{
    public TMP_Text buyIn;
    public Slider buyInSlider;

    private void Awake()
    {
        buyInSlider.onValueChanged.AddListener(delegate {UpdateBuyIn();});
    }

    private void UpdateBuyIn()
    {
        buyIn.text = "Buy In : " + buyInSlider.value + "$";
    }
}
