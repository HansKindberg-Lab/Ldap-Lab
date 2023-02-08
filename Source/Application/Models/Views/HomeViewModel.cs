namespace Application.Models.Views
{
	public class HomeViewModel
	{
		#region Fields

		private ConnectionOptionsForm _connectionOptionsForm;
		private ConnectionSelectionForm _connectionSelectionForm;

		#endregion

		#region Properties

		public virtual ConnectionOptionsForm ConnectionOptionsForm
		{
			get => this._connectionOptionsForm ??= new ConnectionOptionsForm();
			set => this._connectionOptionsForm = value;
		}

		public virtual ConnectionSelectionForm ConnectionSelectionForm
		{
			get => this._connectionSelectionForm ??= new ConnectionSelectionForm();
			set => this._connectionSelectionForm = value;
		}

		public virtual string ConnectionStringName { get; set; }
		public virtual string MaskedConnectionString { get; set; }
		public virtual string Message { get; set; }

		#endregion
	}
}