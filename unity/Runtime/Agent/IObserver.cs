using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PAIA.Marenv
{
    public interface IObserver
    {
        // TODO
    }
    
    public static class ObserverExtensions
    {
        public static IData GetObservations(this IObserver observer)
        {
            // Which scope to search
            bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            // Get fields with IDataAttribute
            var fields = observer.GetType().GetFields(bindingFlags);
            foreach (var field in fields)
            {
                var attr = (IDataAttribute)field.GetCustomAttribute(typeof(IDataAttribute));
                if (attr != null)
                {
                    (field, attr);
                }
            }
            // Get properties with IDataAttribute
            var properties = observer.GetType().GetProperties(bindingFlags);
            foreach (var prop in properties)
            {
                var attr = (IDataAttribute)prop.GetCustomAttribute(typeof(IDataAttribute));
                if (attr != null)
                {
                    (prop, attr);
                }
            }
            // TODO: Get Data from child sensor components
        }
    }
}