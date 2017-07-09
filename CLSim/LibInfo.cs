namespace Simocracy.CLSim
{
    /// <summary>
    /// Info class for external library
    /// </summary>
    public class LibInfo
    {
        /// <summary>
        /// Library Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Library URL
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Library License
        /// </summary>
        public string License { get; }

        /// <summary>
        /// Library License URL to full text
        /// </summary>
        public string LicenseUrl { get; }

        /// <summary>
        /// Creates a new LibInfo instance
        /// </summary>
        /// <param name="name">Library Name</param>
        /// <param name="url">Library URL</param>
        /// <param name="license">Library License</param>
        /// <param name="licenseUrl">Library License URL to full text</param>
        public LibInfo(string name, string url, string license, string licenseUrl)
        {
            Name = name;
            Url = url;
            License = license;
            LicenseUrl = licenseUrl;
        }
    }
}
