using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System; // Importing TextMeshPro namespace for text rendering

public class MoneyView : MonoBehaviour{

    public TextMeshProUGUI moneyText; // Reference to the TextMeshProUGUI component for displaying money
    public TextMeshProUGUI moneyChangePerSecondText;
    public GameObject moneyTextPanel;
    public GameObject moneyChangePerSecondPanel; // Reference to the TextMeshProUGUI component for displaying money per second
    public Image moneyChangeTrendImage;// Reference to the TextMeshProUGUI component for displaying expense per second

    private void Awake()
    { // Add a listener to the button click event to toggle the money change per second text
        MoneyController.OnMoneyChanged += UpdateMoneyText; 
        MoneyController.OnChangePerSecondChanged += UpdateChangePerSecondText; // Subscribe to the OnMoneyChanged event
    }


    private void OnDestroy()
    {
        MoneyController.OnMoneyChanged -= UpdateMoneyText; // Unsubscribe from the OnMoneyChanged event
        MoneyController.OnChangePerSecondChanged -= UpdateChangePerSecondText; // Unsubscribe from the OnMoneyChanged event
    }

    public void UpdateMoneyText(float currentMoney)
    {
        moneyText.text = "$" + currentMoney.ToString("#,##0.00"); // Update the money text with the current money value
    }

    public void UpdateChangePerSecondText(float moneyPerSecond)
    {

        if(moneyPerSecond < 0)
        {
            moneyChangePerSecondText.color = Color.red;
            moneyChangePerSecondText.text = moneyPerSecond.ToString("F2");
            // moneyChangeTrendImage.image = //down arrow image and also set the color to red
        }
        else if(moneyPerSecond > 0)
        {
            moneyChangePerSecondText.color = Color.green;
            moneyChangePerSecondText.text = "+" + moneyPerSecond.ToString("F2"); // Set the color to green if money per second is positive
            // moneyChangeTrendImage.image = //up arrow image and also set the color to green
        }
        else
        {
            moneyChangePerSecondText.color = Color.white; // Set the color to white if money per second is zero
            moneyChangePerSecondText.text = moneyPerSecond.ToString("F2"); // Set the color to white if money per second is zero
            // moneyChangeTrendImage.image = //neutral arrow image and also set the color to white
        }
    }

    public void ToggleChangePerSecondText(){
        if(moneyChangePerSecondPanel.activeSelf)
        {
            moneyChangePerSecondPanel.SetActive(false); // Hide the money change per second text panel
        }
        else
        {
            moneyChangePerSecondPanel.SetActive(true); // Show the money change per second text panel
        }
    }
}