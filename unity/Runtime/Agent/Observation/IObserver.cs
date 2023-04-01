using System.Collections;
using System.Collections.Generic;

namespace PAIA.Gymize
{
    public interface IObserver
    {
        IData GetObservation(int cacheId);
    }
}