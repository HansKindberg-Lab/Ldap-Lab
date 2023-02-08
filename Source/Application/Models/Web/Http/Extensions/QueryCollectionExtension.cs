namespace Application.Models.Web.Http.Extensions
{
	public static class QueryCollectionExtension
	{
		#region Methods

		public static IDictionary<string, string> ToDictionary(this IQueryCollection queryCollection)
		{
			if(queryCollection == null)
				throw new ArgumentNullException(nameof(queryCollection));

			return queryCollection.ToDictionary(item => item.Key, item => (string)item.Value);
		}

		public static IDictionary<string, string> ToSortedDictionary(this IQueryCollection queryCollection)
		{
			if(queryCollection == null)
				throw new ArgumentNullException(nameof(queryCollection));

			var sortedDictionary = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			foreach(var (key, value) in queryCollection.ToDictionary())
			{
				sortedDictionary.Add(key, value);
			}

			return sortedDictionary;
		}

		#endregion
	}
}