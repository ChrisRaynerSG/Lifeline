public class MedicationModel : TreatmentModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Dosage { get; set; }
    public string SideEffects { get; set; }
    public string Contraindications { get; set; }
    public string Interactions { get; set; }
    public string AdministrationRoute { get; set; }
    public string Frequency { get; set; }
}