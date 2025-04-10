using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SimulationSpeedController : MonoBehaviour
{

    // This script is used to control the simulation speed of the game
    // The simulation speed can be adjusted using the keyboard
    // The simulation speed can be set to 1x, 2x, or 4x

    float simulationSpeed; // Default simulation speed
    public Button pauseButton;
    public Button speedOneButton; // Button to set simulation speed to 1x
    public Button speedTwoButton; // Button to set simulation speed to 2x
    public Button speedFourButton; // Button to set simulation speed to 4x

    private bool isPaused = true;
    private bool isStarted = false;

    private Button[] buttons = new Button[4];
    
    /*
     * The simulation speed is controlled by a dictionary that maps the simulation speed to a string representation
     * The dictionary is used to map the simulation speed to a string representation
     * The simulation speed can be set to 1x, 2x, or 4x
     */

    Dictionary<int , (float, string)> simulationSpeeds = new Dictionary<int, (float, string)>
    {
        {0,(0f, "Paused")},
       {1, (1f, "1x")},
       {2, (4f, "4x")},
       {3, (10f, "10x")}
    };

    int currentSimulationSpeedIndex = 0; // Index of the current simulation speed
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int simulationSpeedBeforePause = 1;
    void Start()
    {
        // Initialize the buttons array with the buttons in the scene
        // This is done to ensure that the buttons are initialized before they are used
        buttons[0] = pauseButton;
        buttons[1] = speedOneButton;
        buttons[2] = speedTwoButton;
        buttons[3] = speedFourButton;

        // Set the initial simulation speed to 0 (paused)
        // This is done to ensure that the simulation speed is set to 0 when the game starts
        SetSimulationSpeed(0);
        isStarted = true;

    }

    // Update is called once per frame
    void Update()
    {
        HandleSpeedChange(); // Call the method to handle speed change
    }

    private void HandleSpeedChange(){
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetSimulationSpeed(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetSimulationSpeed(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetSimulationSpeed(3);
        }

        if(Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyUp(KeyCode.KeypadPlus))
        {
            if(currentSimulationSpeedIndex < simulationSpeeds.Count - 1)
            {
                currentSimulationSpeedIndex++;
                SetSimulationSpeed(currentSimulationSpeedIndex);
            }
        }

        if(Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus))
        {
            if(currentSimulationSpeedIndex > 0)
            {
                currentSimulationSpeedIndex--;
                SetSimulationSpeed(currentSimulationSpeedIndex);
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SetSimulationSpeed(0); // Pause the simulation
        }

        // Add listeners to the buttons to handle button clicks
        pauseButton.onClick.AddListener(() => OnSpeedButtonClick(0)); // Pause the simulation
        speedOneButton.onClick.AddListener(() => OnSpeedButtonClick(1));
        speedTwoButton.onClick.AddListener(() => OnSpeedButtonClick(2));
        speedFourButton.onClick.AddListener(() => OnSpeedButtonClick(3));
    }

    private void SetSimulationSpeed(int index)
    {
        if (index < 0 || index >= simulationSpeeds.Count)
        {
            Debug.LogError("Invalid simulation speed index");
            return;
        }
        if(index == 0 && isPaused && isStarted){
            isPaused = false;
            index = simulationSpeedBeforePause; // Set the index to the previous speed before pause
        }
        else if(index == 0 && !isPaused){
            isPaused = true;
            simulationSpeedBeforePause = currentSimulationSpeedIndex;
        }

        SetButtonInteractivity(index); // Set the button interactivity based on the current simulation speed index
        currentSimulationSpeedIndex = index;
        var (speed, label) = simulationSpeeds[index];
        simulationSpeed = speed; // Get the simulation speed from the dictiona
        Time.timeScale = simulationSpeed; // Set the time scale to the new simulation speed
        if(index != 0){
            isPaused = false;
        }

    }

    private void OnSpeedButtonClick(int index)
    {

        // Enable all buttons before disabling the clicked one in SetSimulationSpeed
        // This is to ensure that the button clicked is disabled in the SetSimulationSpeed method
        
        SetSimulationSpeed(index);
        
        // play noise based on the button clicked
        switch(index)
        {
            case 0:
                
                // Play noise for pause need to be implemented
                break;
            case 1:
                // Play noise for 1x speed need to be implemented
                buttons[index].interactable = false;
                break;
            case 2:
                // Play noise for 2x speed
                buttons[index].interactable = false;
                break;
            case 3:
                // Play noise for 4x speed
                buttons[index].interactable = false;
                break;
            default:
                Debug.LogError("Invalid button index");
                break;
        }
    }

    private void SetButtonInteractivity(int index){
         foreach (Button button in buttons)
        {
            // enable all buttons
            button.interactable = true;
        }
        //disable the button that corresponds to the current simulation speed
        buttons[index].interactable = false;

    }
}
