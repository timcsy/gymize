namespace Gymize
{
    public interface IObserver
    {
        string GetLocator(); // locator should be a constant
        IInstance GetObservation();
    }
}