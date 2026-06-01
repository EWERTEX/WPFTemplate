namespace WPFTemplate.Helpers
{
	public static class SessionContext
	{
		//public static User CurrentUser { get; set; }
		public static int RoleId { get; set; }

		public static void Clear()
		{
			RoleId = 0;
		}
	}
}