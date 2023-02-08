namespace Application.Models.DirectoryServices.Protocols.Configuration
{
	public class DirectoryOptions
	{
		#region Properties

		public virtual SearchOptions DefaultSearch { get; set; }
		public virtual IDictionary<string, SearchOptions> SearchMappings { get; } = new Dictionary<string, SearchOptions>(StringComparer.OrdinalIgnoreCase);

		#endregion
	}
}