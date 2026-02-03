namespace DvdSubExtractor;

public class TypeEventArgs : EventArgs
{
    public TypeEventArgs(Type type)
    {
        this.Type = type;
    }

    public Type Type { get; private set; }
}

public interface IWizardItem : IDisposable
{
    void Initialize(ExtractData data);
    void Terminate();
}

