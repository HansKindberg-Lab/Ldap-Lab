using System.DirectoryServices.Protocols;

namespace Application.Models.DirectoryServices.Protocols.Configuration
{
	public class LdapConnectionOptions
	{
		#region Properties

		public virtual AuthType? AuthenticationType { get; set; }
		public virtual string CredentialDomain { get; set; }
		public virtual string CredentialPassword { get; set; }
		public virtual string CredentialUserName { get; set; }
		public virtual ushort? Port { get; set; }
		public virtual byte? ProtocolVersion { get; set; }
		public virtual bool? SecureSocketLayer { get; set; }
		public virtual ISet<string> Servers { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		public virtual TimeSpan? Timeout { get; set; }

		#endregion
	}
}