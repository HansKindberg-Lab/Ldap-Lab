using System.DirectoryServices.Protocols;

namespace Application.Models.DirectoryServices.Protocols.Configuration
{
	public class LdapConnectionOptionsParser : ILdapConnectionOptionsParser
	{
		#region Methods

		protected internal virtual LdapConnectionOptions CreateLdapOptions(IDictionary<string, string> dictionary)
		{
			if(dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));

			var ldapOptions = new LdapConnectionOptions();

			var key = nameof(LdapConnectionOptions.AuthenticationType);
			if(dictionary.TryGetValue(key, out var value))
			{
				ldapOptions.AuthenticationType = Enum.Parse<AuthType>(value);
				dictionary.Remove(key);
			}

			key = nameof(LdapConnectionOptions.CredentialDomain);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapOptions.CredentialDomain = value;
				dictionary.Remove(key);
			}

			key = nameof(LdapConnectionOptions.CredentialPassword);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapOptions.CredentialPassword = value;
				dictionary.Remove(key);
			}

			key = nameof(LdapConnectionOptions.CredentialUserName);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapOptions.CredentialUserName = value;
				dictionary.Remove(key);
			}

			key = nameof(LdapConnectionOptions.Port);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapOptions.Port = ushort.Parse(value, null);
				dictionary.Remove(key);
			}

			key = nameof(LdapConnectionOptions.ProtocolVersion);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapOptions.ProtocolVersion = byte.Parse(value, null);
				dictionary.Remove(key);
			}

			key = nameof(LdapConnectionOptions.SecureSocketLayer);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapOptions.SecureSocketLayer = bool.Parse(value);
				dictionary.Remove(key);
			}

			key = nameof(LdapConnectionOptions.Servers);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapOptions.Servers.Clear();
				foreach(var server in value.Split(','))
				{
					ldapOptions.Servers.Add(server.Trim());
				}

				dictionary.Remove(key);
			}

			key = nameof(LdapConnectionOptions.Timeout);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapOptions.Timeout = TimeSpan.Parse(value, null);
				dictionary.Remove(key);
			}

			return ldapOptions;
		}

		public virtual LdapConnectionOptions Parse(string value)
		{
			if(value == null)
				return null;

			try
			{
				var dictionary = this.ParseToDictionary(value);

				var ldapOptions = this.CreateLdapOptions(dictionary);

				if(dictionary.Any())
					throw new InvalidOperationException($"The following keys/properties are not allowed: {string.Join(", ", dictionary.Keys)}");

				return ldapOptions;
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException("Could not parse to ldap-options.", exception);
			}
		}

		protected internal virtual IDictionary<string, string> ParseToDictionary(string value)
		{
			var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			// ReSharper disable InvertIf
			if(value != null)
			{
				foreach(var keyValuePair in value.Trim().Split(';').Select(keyValuePair => keyValuePair.Trim()).Where(keyValuePair => !string.IsNullOrWhiteSpace(keyValuePair)))
				{
					var parts = keyValuePair.Split(new[] { '=' }, 2).Select(part => part.Trim()).ToArray();
					dictionary.Add(parts[0], parts.Length > 1 ? parts[1] : null);
				}
			}
			// ReSharper restore InvertIf

			return dictionary;
		}

		#endregion
	}
}