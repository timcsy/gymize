using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public interface IObserver
    {
        IData GetObservation(int cacheId);
    }
}