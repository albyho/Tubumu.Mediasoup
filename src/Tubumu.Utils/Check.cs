using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tubumu.Utils
{
    [DebuggerStepThrough]
    public static class Check
    {
        public static T NotNull<T>(T value, string parameterName)
        {
            return value != null
                ? value
                : throw new ArgumentNullException(parameterName);
        }

        public static T NotNull<T>(T value, string parameterName, string message)
        {
            return value != null
                ? value
                : throw new ArgumentNullException(parameterName, message);
        }

        public static string NotNullOrWhiteSpace(string value, string parameterName)
        {
            return !value.IsNullOrWhiteSpace()
                ? value
                : throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
        }

        public static string NotNullOrEmpty(string value, string parameterName)
        {
            return !value.IsNullOrEmpty()
                ? value
                : throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
        }

        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> value, string parameterName)
        {
            return value.IsNullOrEmpty()
                ? value
                : throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
        }
    }
}
