using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DiabetesTypeOneModel : EndocrineAndMetabolicDisorderModel
{
    
    public DiabetesTypeOneModel()
    {
        // Constructor for Diabetes Type 1 Model
        // Inherits from EndocrineAndMetabolicDisorderModel
        // Initializes the properties specific to Diabetes Type 1
        string Name = "Diabetes Type 1";
        string Description = "A chronic condition in which the pancreas produces little or no insulin.";
        List<SymptomModel> Symptoms = new List<SymptomModel>
        {
            new SymptomModel { Name = "Increased thirst" },
            new SymptomModel { Name = "Frequent urination" },
            new SymptomModel { Name = "Extreme hunger" },
            new SymptomModel { Name = "Unexplained weight loss" },
            new SymptomModel { Name = "Fatigue" },
            new SymptomModel { Name = "Irritability" }
        };

    }
}