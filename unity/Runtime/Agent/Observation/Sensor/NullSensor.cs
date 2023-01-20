namespace PAIA.Marenv
{
    public class NullSensor : SensorComponent
    {
        public override IData GetObservation(int cacheId = -1)
        {
            return null;
        }
    }
}
