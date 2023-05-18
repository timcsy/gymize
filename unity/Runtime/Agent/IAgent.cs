namespace Gymize
{
    public interface IAgent : IActuator
    {
        void OnReset();
        void AddReward(double reward);
        void SetReward(double reward);
        void Terminate();
        void Truncate();
        void SendInfo(object info);
        void OnInfo(object info);
        void AddToEnv();
        void RemoveFromEnv();
    }
}