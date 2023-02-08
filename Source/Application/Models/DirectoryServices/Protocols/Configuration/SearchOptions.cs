using System.DirectoryServices.Protocols;

namespace Application.Models.DirectoryServices.Protocols.Configuration
{
	public class SearchOptions
	{
		#region Properties

		public virtual ISet<string> Attributes { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		public virtual string Filter { get; set; }
		public virtual PagingOptions Paging { get; set; } = new();
		public virtual string RootDistinguishedName { get; set; }
		public virtual SearchScope? Scope { get; set; }

		#endregion
	}
}