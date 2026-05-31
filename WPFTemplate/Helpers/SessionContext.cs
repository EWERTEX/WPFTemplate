namespace WPFTemplate.Helpers
{
	public static class SessionContext
	{
		public static string RoleName { get; set; } = "Гость";

		public static void Clear()
		{
			RoleName = "Гость";
		}
	}
}