using System;
using System.Collections.Generic;
public class DiabetesTypeTwoModel : EndocrineAndMetabolicDisorderModel
{
    public DiabetesTypeTwoModel()
    {
        string Name = "Diabetes Type 2";
        string Description = "A chronic condition that affects the way the body processes blood sugar (glucose).";
        List<SymptomModel> Symptoms = new List<SymptomModel>
        {
            new SymptomModel { Name = "Increased thirst" },
            new SymptomModel { Name = "Frequent urination" },
            new SymptomModel { Name = "Extreme hunger" },
            new SymptomModel { Name = "Unexplained weight loss" },
            new SymptomModel { Name = "Fatigue" },
            new SymptomModel { Name = "Blurred vision" },
            new SymptomModel { Name = "Slow-healing sores or frequent infections" }
        };

        List<String> RiskFactors = new List<string>
        {
            "Being overweight",
            "Being inactive",
            "Having a family history of diabetes",
            "Being over the age of 45",
            "Having high blood pressure or abnormal cholesterol levels"
        };
        
        List<String> Complications = new List<string>
        {
            "Heart disease and stroke",
            "Nerve damage (neuropathy)",
            "Kidney damage (nephropathy)",
            "Eye damage (retinopathy)",
            "Foot damage",
            "Skin conditions",
            "Hearing impairment"
        };   
    }

}