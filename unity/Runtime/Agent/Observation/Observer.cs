using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine; // For Testing

namespace PAIA.Marenv
{
    public class ObservationInfo
    {
        public ObservationInfo()
        {
            Fields = new List<string>();
            Data = null;
        }
        public ObservationInfo(string field, IData data = null)
        {
            Fields = new List<string>();
            Fields.Add(field);
            Data = data;
        }
        public ObservationInfo(List<string> fields, IData data = null)
        {
            Fields = new List<string>(fields);
            Data = data;
        }

        public List<string> Fields { get; set; } // from child to parent, start from [0]
        public IData Data { get; set; }
    }

    public class Observer : IObserver
    {
        public List<string> GetFields()
        {
            return null;
        }
        public IData GetData()
        {
            return null;
        }

        public static ObservationInfo CollectObservations(object o)
        {
            Debug.Log("GetType: " + o.GetType());
            // Which scope to search
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            // Get fields with DataAttribute
            FieldInfo[] fields = o.GetType().GetFields(bindingFlags);
            foreach (FieldInfo field in fields)
            {
                AttributeBase attr = (AttributeBase)field.GetCustomAttribute(typeof(AttributeBase));
                if (attr != null)
                {
                    string declaringTypeName = field.DeclaringType.Name;
                    string memberName = field.Name;
                    Type memberType = field.FieldType;
                    Debug.Log("Field:");
                    Debug.Log("DeclaringTypeName: " + declaringTypeName);
                    Debug.Log("MemberName: " + memberName); // Default Field name for ObservationInfo.Fields
                    Debug.Log("MemberType: " + memberType.ToString());
                    Debug.Log("AttributeField: " + attr.Fields); // Add to ObservationInfo.Fields
                    Debug.Log("AgentName: " + attr.AgentName); // Agent name
                }
            }
            // Get properties with DataAttribute
            PropertyInfo[] properties = o.GetType().GetProperties(bindingFlags);
            foreach (PropertyInfo prop in properties)
            {
                AttributeBase attr = (AttributeBase)prop.GetCustomAttribute(typeof(AttributeBase));
                if (attr != null)
                {
                    string declaringTypeName = prop.DeclaringType.Name;
                    string memberName = prop.Name;
                    Type memberType = prop.PropertyType;
                    Debug.Log("Property:");
                    Debug.Log("DeclaringTypeName: " + declaringTypeName);
                    Debug.Log("MemberName: " + memberName);
                    Debug.Log("MemberType: " + memberType.ToString());
                    Debug.Log("AttributeField: " + attr.Fields);
                    Debug.Log("AgentName: " + attr.AgentName);
                }
            }
            // TODO: Get Data from child sensor componentss
            return null;
        }
    }
}