namespace CK.Crs.SignalR
{
    /// <summary>
    /// Options for CRS SignalR hub
    /// </summary>
    public class CrsSignalROptions
    {
        public CrsSignalROptions()
        {
            CrsHubPath = "/crs";
        }

        /// <summary>
        /// Gets or sets the crs hub path. Defaults to crs.
        /// </summary>
        public string CrsHubPath { get; set; }
    }
}
