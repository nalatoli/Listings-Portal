namespace Listings_Portal.Tests.Tools.Managers
{
    internal static class ConfigurationExt
    {
        /// <summary>
        /// Runs configuration over specifed object and returns the object.
        /// </summary>
        /// <typeparam name="T"> Object type. </typeparam>
        /// <param name="obj"> Object to confiure. </param>
        /// <param name="configure"> Optional configuration. </param>
        /// <returns> Configured object. </returns>
        internal static T GetConfigured<T>(this T obj, Action<T>? configure = null) where T : class
        {
            configure?.Invoke(obj);
            return obj;
        }
    }
}