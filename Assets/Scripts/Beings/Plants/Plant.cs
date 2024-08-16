
/// <summary>
/// Abstraktni trida reprezentuje herni objekt rostliny
/// </summary>
public abstract class Plant : Being, IResetable
{
    /// <summary>
    /// Metoda nastavi rostlinu do puvodniho stavu
    /// </summary>
    public abstract void ResetObject();
}
