namespace MinimalisticTD
{
#if WINDOWS || XBOX

	/// <summary>
	/// The main entry class.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		private static void Main(string[] args)
		{
			using (Game1 game = new Game1())
			{
				game.Run();
			}
		}
	}

#endif
}