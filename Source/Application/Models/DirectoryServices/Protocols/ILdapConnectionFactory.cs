using System.DirectoryServices.Protocols;
using Application.Models.DirectoryServices.Protocols.Configuration;

namespace Application.Models.DirectoryServices.Protocols
{
	public interface ILdapConnectionFactory
	{
		#region Methods

		LdapConnection Create(LdapConnectionOptions options);

		#endregion
	}
}