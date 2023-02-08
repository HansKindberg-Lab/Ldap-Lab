using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.Protocols;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.Models.Views
{
	public class ConnectionOptionsForm
	{
		#region Properties

		[Display(Name = "Attributes")]
		public virtual string Attributes { get; set; }

		public virtual string AttributesHint => "Attributes, separated by comma, to search for.";

		[Display(Name = "Authentication-type")]
		public virtual AuthType? AuthenticationType { get; set; }

		public virtual string AuthenticationTypeHint => "Select an authentication type or leave blank to use ldap-connection default.";
		public virtual IList<SelectListItem> AuthenticationTypes { get; } = new List<SelectListItem>();
		public virtual bool BindOnly { get; set; }

		[Display(Name = "Credential-domain")]
		public virtual string CredentialDomain { get; set; }

		public virtual string CredentialDomainHint => "Enter a credential-domain for the connection.";

		[DataType(DataType.Password)]
		[Display(Name = "Credential-password")]
		public virtual string CredentialPassword { get; set; }

		public virtual string CredentialPasswordHint => "Enter a password for the connection.";

		[Display(Name = "Credential user-name")]
		public virtual string CredentialUserName { get; set; }

		public virtual string CredentialUserNameHint => "Enter a user-name to use instead of the one in the connection-string.";

		[Display(Name = "Filter")]
		public virtual string Filter { get; set; }

		public virtual string FilterHint => "The filter for the search.";
		public virtual bool OverrideCredentialPassword { get; set; }

		[Display(Name = "Page-size")]
		public virtual int PageSize { get; set; } = new PageResultRequestControl().PageSize;

		public virtual string PageSizeHint => "Page-size for paging.";

		[Display(Name = "Paging enabled")]
		public virtual bool PagingEnabled { get; set; } = true;

		public virtual string PagingEnabledHint => "Enable paging.";

		[Display(Name = "Port")]
		public virtual ushort? Port { get; set; }

		public virtual string PortHint => "Enter a port, eg. 389, 636, 1389 or 1636.";

		[Display(Name = "Protocol-version")]
		public virtual byte? ProtocolVersion { get; set; }

		public virtual string ProtocolVersionHint => "Enter a protocol version, eg. 3.";

		[Display(Name = "Root distinguished name")]
		public virtual string RootDistinguishedName { get; set; }

		public virtual string RootDistinguishedNameHint => "Distinguished name to set where to search.";

		[Display(Name = "Search scope")]
		public virtual SearchScope SearchScope { get; set; } = SearchScope.Subtree;

		public virtual string SearchScopeHint => "Scope for the search.";
		public virtual IList<SelectListItem> SearchScopes { get; } = new List<SelectListItem>();

		[Display(Name = "SSL/TLS")]
		public virtual bool SecureSocketsLayer { get; set; }

		public virtual string SecureSocketsLayerHint => "Enable TLS.";

		[Display(Name = "Servers")]
		public virtual string Servers { get; set; }

		public virtual string ServersHint => "Enter one or multiple servers separated by a comma, eg. \"example\" (the domain-name) or \"dc01.example.org, dc02.example.org, dc03.example.org\".";

		[Display(Name = "Timeout")]
		public virtual int? Timeout { get; set; }

		public virtual string TimeoutHint => "Enter the connection-timeout in seconds.";

		#endregion
	}
}