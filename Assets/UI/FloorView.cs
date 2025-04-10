using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloorView : MonoBehaviour {

    public TextMeshProUGUI floorText; // Text component to display the current floor number
    public Button upButton;
    public Button downButton;

    public void Awake()
    {
        MapController.OnFloorChanged += UpdateFloorText; // Subscribe to the OnFloorChanged event
    }
    public void UpdateFloorText(int floorNumber) {
        if(floorNumber == 0){
            floorText.text = "G";
        }
        else{
            floorText.text = floorNumber.ToString();

        }
    }
}