using System;
using System.Reflection;

namespace Blood_Pact_Ritual.BloodPactRitual
{
    public class Utils
    {
        public static void CopyFields<T>(T source, T destination)
        {
            var type = source.GetType();
            while (type != null)
            {
                CopyBaseFieldsForType(type, source, destination);
                type = type.BaseType;
            }
        }


        private static void CopyBaseFieldsForType<T>(Type type, T source, T destination)
        {
            var myObjectFields = type.GetFields(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var fi in myObjectFields)
            {
                fi.SetValue(destination, fi.GetValue(source));
            }
        }
    }
}