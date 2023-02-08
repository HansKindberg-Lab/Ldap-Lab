using System.DirectoryServices.Protocols;
using Application.Models.DirectoryServices.Protocols;
using Application.Models.DirectoryServices.Protocols.Configuration;
using Application.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Application.Controllers
{
	public class HomeController : Controller
	{
		#region Fields

		private bool? _bindOnly;
		private ConnectionSelectionForm _connectionSelectionForm;
		private Lazy<string> _connectionString;
		private Lazy<string> _connectionStringName;
		private IDictionary<string, string> _connectionStrings;
		private Lazy<string> _maskedConnectionString;
		private bool? _overrideCredentialPassword;

		#endregion

		#region Constructors

		public HomeController(IConfiguration configuration, IOptionsMonitor<DirectoryOptions> directoryOptionsMonitor, ILdapConnectionFactory ldapConnectionFactory, ILdapConnectionOptionsParser ldapConnectionOptionsParser, ILoggerFactory loggerFactory)
		{
			this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this.DirectoryOptionsMonitor = directoryOptionsMonitor ?? throw new ArgumentNullException(nameof(directoryOptionsMonitor));
			this.LdapConnectionFactory = ldapConnectionFactory ?? throw new ArgumentNullException(nameof(ldapConnectionFactory));
			this.LdapConnectionOptionsParser = ldapConnectionOptionsParser ?? throw new ArgumentNullException(nameof(ldapConnectionOptionsParser));
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
		}

		#endregion

		#region Properties

		protected internal virtual bool BindOnly
		{
			get
			{
				// ReSharper disable InvertIf
				if(this._bindOnly == null)
				{
					if(!bool.TryParse(this.Request.Query[nameof(Models.Views.ConnectionSelectionForm.BindOnly)], out var bindOnly))
						bindOnly = false;

					this._bindOnly = bindOnly;
				}
				// ReSharper restore InvertIf

				return this._bindOnly.Value;
			}
		}

		protected internal virtual IConfiguration Configuration { get; }

		protected internal virtual ConnectionSelectionForm ConnectionSelectionForm
		{
			get
			{
				// ReSharper disable InvertIf
				if(this._connectionSelectionForm == null)
				{
					var form = new ConnectionSelectionForm
					{
						BindOnly = this.BindOnly,
						OverrideCredentialPassword = this.OverrideCredentialPassword
					};

					form.ConnectionStringNames.Add(new SelectListItem());

					foreach(var connectionStringName in this.ConnectionStrings.Keys)
					{
						form.ConnectionStringNames.Add(new SelectListItem(connectionStringName, connectionStringName, string.Equals(connectionStringName, this.ConnectionStringName, StringComparison.OrdinalIgnoreCase)));
					}

					this._connectionSelectionForm = form;
				}
				// ReSharper restore InvertIf

				return this._connectionSelectionForm;
			}
		}

		protected internal virtual string ConnectionString
		{
			get
			{
				this._connectionString ??= new Lazy<string>(() =>
				{
					if(this.ConnectionStringName != null && this.ConnectionStrings.TryGetValue(this.ConnectionStringName, out var connectionString))
						return connectionString;

					return null;
				});

				return this._connectionString.Value;
			}
		}

		protected internal virtual string ConnectionStringName
		{
			get
			{
				this._connectionStringName ??= new Lazy<string>(() =>
				{
					var connectionStringName = (string)this.Request.Query[nameof(Models.Views.ConnectionSelectionForm.ConnectionStringName)];

					if(connectionStringName != null && this.ConnectionStrings.ContainsKey(connectionStringName))
						return connectionStringName;

					return null;
				});

				return this._connectionStringName.Value;
			}
		}

		protected internal virtual IDictionary<string, string> ConnectionStrings
		{
			get
			{
				// ReSharper disable InvertIf
				if(this._connectionStrings == null)
				{
					var connectionStrings = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

					this.Configuration.GetSection("ConnectionStrings").Bind(connectionStrings);

					this._connectionStrings = connectionStrings;
				}
				// ReSharper restore InvertIf

				return this._connectionStrings;
			}
		}

		protected internal virtual IOptionsMonitor<DirectoryOptions> DirectoryOptionsMonitor { get; }
		protected internal virtual ILdapConnectionFactory LdapConnectionFactory { get; }
		protected internal virtual ILdapConnectionOptionsParser LdapConnectionOptionsParser { get; }
		protected internal virtual ILogger Logger { get; }

		protected internal virtual string MaskedConnectionString
		{
			get
			{
				this._maskedConnectionString ??= new Lazy<string>(() =>
				{
					if(string.IsNullOrWhiteSpace(this.ConnectionString))
						return this.ConnectionString;

					const char separator = ';';
					const char keyValueSeparator = '=';

					var originalParts = this.ConnectionString.Split(separator).ToList();
					var parts = new List<string>();

					foreach(var part in originalParts)
					{
						var keyValuePair = part.Split(keyValueSeparator, 2);

						if(keyValuePair.Length > 1)
						{
							if(string.Equals(keyValuePair[0].Trim(), nameof(LdapConnectionOptions.CredentialPassword), StringComparison.OrdinalIgnoreCase))
							{
								parts.Add($"{keyValuePair[0]}{keyValueSeparator}**********");

								continue;
							}
						}

						parts.Add(part);
					}

					var maskedConnectionString = string.Join(separator, parts);

					return maskedConnectionString;
				});

				return this._maskedConnectionString.Value;
			}
		}

		protected internal virtual bool OverrideCredentialPassword
		{
			get
			{
				// ReSharper disable InvertIf
				if(this._overrideCredentialPassword == null)
				{
					if(!bool.TryParse(this.Request.Query[nameof(Models.Views.ConnectionSelectionForm.OverrideCredentialPassword)], out var overrideCredentialPassword))
						overrideCredentialPassword = false;

					this._overrideCredentialPassword = overrideCredentialPassword;
				}
				// ReSharper restore InvertIf

				return this._overrideCredentialPassword.Value;
			}
		}

		#endregion

		#region Methods

		protected internal virtual async Task<ConnectionOptionsForm> CreateConnectionOptionsFormAsync(ConnectionOptionsForm currentForm = null)
		{
			var form = currentForm ?? new ConnectionOptionsForm();

			// ReSharper disable InvertIf
			if(this.ConnectionStringName != null)
			{
				form.BindOnly = this.BindOnly;
				form.OverrideCredentialPassword = this.OverrideCredentialPassword;

				if(currentForm == null)
				{
					var ldapConnectionOptions = await this.CreateLdapConnectionOptionsAsync();

					form.AuthenticationType = ldapConnectionOptions.AuthenticationType;
					form.CredentialDomain = ldapConnectionOptions.CredentialDomain;
					form.CredentialUserName = ldapConnectionOptions.CredentialUserName;
					form.Port = ldapConnectionOptions.Port;
					form.ProtocolVersion = ldapConnectionOptions.ProtocolVersion;
					form.SecureSocketsLayer = ldapConnectionOptions.SecureSocketLayer ?? false;

					form.Servers = string.Join(", ", ldapConnectionOptions.Servers);

					if(ldapConnectionOptions.Timeout != null)
						form.Timeout = ldapConnectionOptions.Timeout.Value.Seconds;

					if(!this.BindOnly)
						await this.PopulateSearchOptionsAsync(form);
				}

				await this.PopulateAuthenticationTypesAsync(form);

				if(!this.BindOnly)
					await this.PopulateSearchScopesAsync(form);
			}
			// ReSharper restore InvertIf

			return await Task.FromResult(form);
		}

		protected internal virtual async Task<LdapConnectionOptions> CreateLdapConnectionOptionsAsync(ConnectionOptionsForm form = null)
		{
			var ldapConnectionOptions = this.LdapConnectionOptionsParser.Parse(this.ConnectionString) ?? new LdapConnectionOptions();

			// ReSharper disable InvertIf
			if(form != null)
			{
				ldapConnectionOptions.AuthenticationType = form.AuthenticationType;
				ldapConnectionOptions.CredentialDomain = form.CredentialDomain;

				if(form.OverrideCredentialPassword)
					ldapConnectionOptions.CredentialPassword = form.CredentialPassword;

				ldapConnectionOptions.CredentialUserName = form.CredentialUserName;
				ldapConnectionOptions.Port = form.Port;
				ldapConnectionOptions.ProtocolVersion = form.ProtocolVersion;
				ldapConnectionOptions.SecureSocketLayer = form.SecureSocketsLayer;

				ldapConnectionOptions.Servers.Clear();

				foreach(var server in (form.Servers ?? string.Empty).Split(','))
				{
					ldapConnectionOptions.Servers.Add(server.Trim());
				}

				ldapConnectionOptions.Timeout = form.Timeout == null ? null : TimeSpan.FromSeconds(form.Timeout.Value);
			}
			// ReSharper restore InvertIf

			return await Task.FromResult(ldapConnectionOptions);
		}

		protected internal virtual async Task<HomeViewModel> CreateModelAsync(ConnectionOptionsForm form = null)
		{
			var model = new HomeViewModel
			{
				ConnectionStringName = this.ConnectionStringName,
				ConnectionOptionsForm = await this.CreateConnectionOptionsFormAsync(form),
				ConnectionSelectionForm = this.ConnectionSelectionForm,
				MaskedConnectionString = this.MaskedConnectionString
			};

			return await Task.FromResult(model);
		}

		protected internal virtual async Task<IEnumerable<SearchResultEntry>> GetSearchResultAsync(LdapConnection connection, ConnectionOptionsForm form)
		{
			if(connection == null)
				throw new ArgumentNullException(nameof(connection));

			if(form == null)
				throw new ArgumentNullException(nameof(form));

			var searchResult = new List<SearchResultEntry>();

			var searchRequest = new SearchRequest(form.RootDistinguishedName, form.Filter, form.SearchScope, (form.Attributes ?? string.Empty).Split(',').Select(attribute => attribute.Trim()).ToArray());
			SearchResponse searchResponse = null;

			// ReSharper disable PossibleNullReferenceException
			if(form.PagingEnabled)
			{
				connection.SessionOptions.ReferralChasing = ReferralChasingOptions.None;

				var pageResultRequestControl = new PageResultRequestControl(form.PageSize);

				searchRequest.Controls.Add(pageResultRequestControl);

				while(searchResponse == null || pageResultRequestControl.Cookie.Length > 0)
				{
					searchResponse = (SearchResponse)connection.SendRequest(searchRequest);

					var pageResultResponseControl = searchResponse.Controls.OfType<PageResultResponseControl>().FirstOrDefault();

					if(pageResultResponseControl != null)
						pageResultRequestControl.Cookie = pageResultResponseControl.Cookie;

					searchResult.AddRange(searchResponse.Entries.Cast<SearchResultEntry>());
				}
			}
			else
			{
				searchResponse = (SearchResponse)connection.SendRequest(searchRequest);

				searchResult.AddRange(searchResponse.Entries.Cast<SearchResultEntry>());
			}
			// ReSharper restore PossibleNullReferenceException

			return await Task.FromResult(searchResult);
		}

		public virtual async Task<IActionResult> Index()
		{
			return await Task.FromResult(this.View(await this.CreateModelAsync()));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Index(ConnectionOptionsForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			var model = await this.CreateModelAsync(form);

			var ldapConnectionOptions = await this.CreateLdapConnectionOptionsAsync(form);

			try
			{
				// ReSharper disable ConvertToUsingDeclaration
				using(var ldapConnection = this.LdapConnectionFactory.Create(ldapConnectionOptions))
				{
					if(form.BindOnly)
					{
						ldapConnection.Bind();
						model.Message = "Bind was successful";
					}
					else
					{
						var searchResult = await this.GetSearchResultAsync(ldapConnection, form);
						var count = searchResult.Count();
						model.Message = $"The search gave {count} number of records.";
					}
				}
				// ReSharper restore ConvertToUsingDeclaration
			}
			catch(Exception exception)
			{
				this.Logger.LogError(exception, "Could not bind/search.");
				this.ModelState.AddModelError("Exception", $"Could not bind: {exception}");
			}

			return await Task.FromResult(this.View(model));
		}

		protected internal virtual async Task PopulateAuthenticationTypesAsync(ConnectionOptionsForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			await Task.CompletedTask;

			if(this.ConnectionStringName == null)
				return;

			form.AuthenticationTypes.Add(new SelectListItem());

			foreach(var authenticationType in Enum.GetValues<AuthType>().OrderBy(authenticationType => authenticationType.ToString()))
			{
				var value = authenticationType.ToString();

				form.AuthenticationTypes.Add(new SelectListItem(value, value, form.AuthenticationType != null && form.AuthenticationType.Value == authenticationType));
			}
		}

		protected internal virtual async Task PopulateSearchOptionsAsync(ConnectionOptionsForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			await Task.CompletedTask;

			if(this.ConnectionStringName == null || this.BindOnly)
				return;

			var directoryOptions = this.DirectoryOptionsMonitor.CurrentValue;

			var searchOptions = new List<SearchOptions>();

			if(directoryOptions.DefaultSearch != null)
				searchOptions.Add(directoryOptions.DefaultSearch);

			if(this.ConnectionStringName != null && directoryOptions.SearchMappings.TryGetValue(this.ConnectionStringName, out var mappedSearchOptions))
				searchOptions.Add(mappedSearchOptions);

			foreach(var options in searchOptions)
			{
				if(options.Attributes.Any())
				{
					form.Attributes = string.Join(", ", options.Attributes);
				}

				if(options.Filter != null)
					form.Filter = options.Filter;

				if(options.Paging.PageSize != null)
					form.PageSize = options.Paging.PageSize.Value;

				if(options.Paging.Enabled != null)
					form.PagingEnabled = options.Paging.Enabled.Value;

				if(options.RootDistinguishedName != null)
					form.RootDistinguishedName = options.RootDistinguishedName;

				if(options.Scope != null)
					form.SearchScope = options.Scope.Value;
			}
		}

		protected internal virtual async Task PopulateSearchScopesAsync(ConnectionOptionsForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			await Task.CompletedTask;

			if(this.ConnectionStringName == null || this.BindOnly)
				return;

			foreach(var searchScope in Enum.GetValues<SearchScope>().OrderBy(searchScope => searchScope.ToString()))
			{
				var value = searchScope.ToString();

				form.SearchScopes.Add(new SelectListItem(value, value, form.SearchScope == searchScope));
			}
		}

		#endregion
	}
}