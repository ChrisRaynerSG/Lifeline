using UnityEngine;

public class HumanModel
{

    private string firstName;
    private string lastName;
    private string title;
    private int age;
    private float height;
    private float weight;
    private string jobTitle;
    private int bloodPressure;
    private int heartRate;
    private int respitoryRate;
    private float oxygenSaturation;
    private float respitoryVolume;
    private float bloodOxygenPartialPressure;

    private float bodyTemperature;
    private float bloodGlucose;
    private string[] languagesSpoken;

    private ConditionModel[] conditions; 
    private SymptomModel[] symptoms;


    private void GiveDiabetesType2(){
        // Logic to give the human diabetes type 2
        // This could involve setting a flag, modifying health parameters, etc.
        Debug.Log("Diabetes Type 2 has been given to the human.");
    }
}
