using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Utilities;

public static class General
{
    /// <summary>
    /// Null guard
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="message"></param>
    /// <param name="objExpression"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [StackTraceHidden]
    [DebuggerHidden]
    [DebuggerStepThrough]
    [return: System.Diagnostics.CodeAnalysis.NotNull]
    public static T NullGuard<T>(
        [System.Diagnostics.CodeAnalysis.NotNull]this T? obj,
        string message = "", [CallerArgumentExpression("obj")] string objExpression = "")
        where T : class
    {
        if (obj is null)
        {
            throw new Exception($"Message: {message}.\n" +
                                $"Expression: " + objExpression);
        }

        return obj;
    }
    
    /// <summary>
    /// Null guard
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="objExpression"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [StackTraceHidden]
    [DebuggerHidden]
    [DebuggerStepThrough]
    [return: System.Diagnostics.CodeAnalysis.NotNull]
    public static T NullGuard<T>(
        [System.Diagnostics.CodeAnalysis.NotNull]this T? obj,
        [CallerArgumentExpression("obj")] string objExpression = "")
        where T : struct
    {
        if (obj is null)
        {
            throw new ArgumentNullException(objExpression);
        }

        return obj.Value;
    }
    
    public static bool CanBeConvertedTo(this Type givenType, Type type) => 
        type.IsAssignableFrom(givenType);
    public static bool CanBeConvertedTo<T>(this Type givenType) => 
        typeof(T).IsAssignableFrom(givenType);
}