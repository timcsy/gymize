using System.Collections;
using System.Collections.Generic;

namespace PAIA.Gymize
{
    public interface ISensor : IObserver
    {
        List<string> GetLocations();
    }
}