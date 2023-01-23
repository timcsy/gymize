using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public interface ISensor : IObserver
    {
        List<string> GetLocations();
    }
}