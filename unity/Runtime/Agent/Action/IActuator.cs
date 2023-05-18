namespace Gymize
{
    public delegate void ActionCallBack(object action);
    
    public interface IActuator
    {
        string GetName(); // name should be a constant
        void OnAction(object action);
    }
}