using System.DirectoryServices.Protocols;
using System.Net;
using Application.Models.DirectoryServices.Protocols.Configuration;

namespace Application.Models.DirectoryServices.Protocols
{
	public class LdapConnectionFactory : ILdapConnectionFactory
	{
		#region Properties

		protected internal virtual LdapDirectoryIdentifier DefaultLdapDirectoryIdentifier { get; } = new LdapDirectoryIdentifier("Server");
		protected internal virtual int ProtocolVersion => 3;

		#endregion

		#region Methods

		public virtual LdapConnection Create(LdapConnectionOptions options)
		{
			if(options == null)
				throw new ArgumentNullException(nameof(options));

			var identifier = this.CreateIdentifier(options);

			// ReSharper disable ConvertToUsingDeclaration
			using(var defaultLdapConnection = new LdapConnection("Server"))
			{
				//var ldapConnection = new LdapConnection(identifier, credential, options.AuthenticationType ?? defaultLdapConnection.AuthType);
				var ldapConnection = new LdapConnection(identifier)
				{
					AuthType = options.AuthenticationType ?? defaultLdapConnection.AuthType
				};

				if(ldapConnection.AuthType != AuthType.Anonymous)
				{
					var credential = this.CreateCredential(options);
					ldapConnection.Credential = credential;
				}

				ldapConnection.SessionOptions.ProtocolVersion = options.ProtocolVersion ?? this.ProtocolVersion;

				if(options.SecureSocketLayer != null)
					ldapConnection.SessionOptions.SecureSocketLayer = options.SecureSocketLayer.Value;

				if(options.Timeout != null)
					ldapConnection.Timeout = options.Timeout.Value;

				return ldapConnection;
			}
			// ReSharper restore ConvertToUsingDeclaration
		}

		protected internal virtual NetworkCredential CreateCredential(LdapConnectionOptions options)
		{
			if(options == null)
				throw new ArgumentNullException(nameof(options));

			return new NetworkCredential(options.CredentialUserName, options.CredentialPassword, options.CredentialDomain);
		}

		protected internal virtual LdapDirectoryIdentifier CreateIdentifier(LdapConnectionOptions options)
		{
			if(options == null)
				throw new ArgumentNullException(nameof(options));

			return new LdapDirectoryIdentifier(options.Servers.ToArray(), options.Port ?? this.DefaultLdapDirectoryIdentifier.PortNumber, this.DefaultLdapDirectoryIdentifier.FullyQualifiedDnsHostName, this.DefaultLdapDirectoryIdentifier.Connectionless);
		}

		#endregion
	}
}