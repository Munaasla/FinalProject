 
public class Baby  
{  
  public int Id { get; set; }  
  public string Name { get; set; }  
  public DateTime DateOfBirth { get; set; }  
  public double Weight { get; set; }  
  public double Height { get; set; }  
  public DateTime? VaccinationDate { get; set; }  
  public string? PhotoPath { get; set; }  
  public int ParentId { get; set; }  
  public Parent Parent { get; set; }  
  public DateTime? NextVaccinationDate { get; }  
  public DateTime? NextMeasurementDate { get; set; } // Added this property  
  public ICollection<Photo> Photos { get; set; }  
  public ICollection<Measurement> Measurements { get; set; }  
}  
