namespace Application.Models.DirectoryServices.Protocols.Configuration
{
	public interface ILdapConnectionOptionsParser
	{
		#region Methods

		LdapConnectionOptions Parse(string value);

		#endregion
	}
}