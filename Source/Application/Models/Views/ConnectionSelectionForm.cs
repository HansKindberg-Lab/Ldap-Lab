using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.Models.Views
{
	public class ConnectionSelectionForm
	{
		#region Properties

		[Display(Name = "Bind only")]
		public virtual bool BindOnly { get; set; }

		public virtual string BindOnlyHint => "If the connection only will perform a bind instead of a search.";

		[Display(Name = "Connection-string name")]
		public virtual string ConnectionStringName { get; set; }

		public virtual string ConnectionStringNameHint => "Select a connection-string.";
		public virtual IList<SelectListItem> ConnectionStringNames { get; } = new List<SelectListItem>();

		[Display(Name = "Override credential-password")]
		public virtual bool OverrideCredentialPassword { get; set; }

		public virtual string OverrideCredentialPasswordHint => "If you check this option you must specify a password that is used instead of the one in the connection-string.";

		#endregion
	}
}