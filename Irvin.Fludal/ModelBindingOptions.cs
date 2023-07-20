namespace Irvin.Fludal;

public class ModelBindingOptions
{
    public ModelBindingOptions()
    {
        PopulateFields();
    }
    
    private ModelBindingStrategy Strategy { get; set; }
    public bool AreSourceDriven => Strategy == ModelBindingStrategy.Source || Strategy == ModelBindingStrategy.SourceStrict;
    public bool AreTargetDriven => Strategy == ModelBindingStrategy.Target || Strategy == ModelBindingStrategy.TargetStrict;
    public bool Strict => Strategy == ModelBindingStrategy.SourceStrict || Strategy == ModelBindingStrategy.TargetStrict;

    public void PopulateFields()
    {
        Strategy = ModelBindingStrategy.Target;
    }
    
    public void PopulateAllFields()
    {
        Strategy = ModelBindingStrategy.TargetStrict;
    }

    public void CaptureFromColumns()
    {
        Strategy = ModelBindingStrategy.Source;
    }
    
    public void CaptureAllColumns()
    {
        Strategy = ModelBindingStrategy.SourceStrict;
    }
}