using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public interface IObserver
    {
        List<string> GetFields();
        IData GetData();
    }
}