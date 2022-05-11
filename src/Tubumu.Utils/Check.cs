using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tubumu.Utils
{
    [DebuggerStepThrough]
    public static class Check
    {
        #region NotNull

        public static T NotNull<T>(T? value, string parameterName, string? message = null)
        {
            return value != null
                ? value
                : throw new ArgumentNullException(parameterName, message);
        }

        #endregion

        #region NotNullOrWhiteSpace

        public static string NotNullOrWhiteSpace(string? value, string parameterName)
        {
            return !value.IsNullOrWhiteSpace()
                ? value!
                : throw new ArgumentException($"{parameterName} can not be null or white space!", parameterName);
        }

        #endregion

        #region NotNullOrEmpty

        public static string NotNullOrEmpty(string? value, string parameterName)
        {
            return !value.IsNullOrEmpty()
                ? value!
                : throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
        }

        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T>? value, string parameterName)
        {
            return !value.IsNullOrEmpty()
                ? value!
                : throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
        }

        public static IEnumerable<T> NotNullOrEmpty<T>(IEnumerable<T>? value, string parameterName)
        {
            return !value.IsNullOrEmpty()
                ? value!
                : throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
        }

        public static IEnumerable<T> NotNullOrEmpty<T>(T[]? value, string parameterName)
        {
            return !value.IsNullOrEmpty()
                ? value!
                : throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
        }

        #endregion
    }
}
