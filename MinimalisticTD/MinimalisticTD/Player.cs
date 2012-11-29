namespace MinimalisticTD
{
	/// <summary>
	/// The player class. Contains information about the current player, like <see cref="Money"/>, <see cref="Lives"/> or <see cref="Score"/>.
	/// </summary>
	public class Player
	{
		/// <summary>
		/// The points this <see cref="Player"/> has accumulated by killing enemies.
		/// </summary>
		public long Score;

		/// <summary>
		/// The money this <see cref="Player"/> has accumulated by killing enemies.
		/// </summary>
		public int Money;

		/// <summary>
		/// The number of lives this <see cref="Player"/> has left before the <see cref="Map"/> is lost.
		/// </summary>
		public int Lives;

		/// <summary>
		/// The name of this <see cref="Player"/>.
		/// </summary>
		private string name;

		/// <summary>
		/// How many enemies this <see cref="Player"/> has killed.
		/// </summary>
		private int killCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="Player"/> class.
		/// </summary>
		/// <param name="name">The <see cref="name"/>.</param>
		public Player(string name = "Anonymous")
		{
			this.name = name;
			this.Score = 0;
			this.killCount = 0;
		}

		/// <summary>
		/// Adds a kill, i.e. rewards the <see cref="Player"/>.
		/// </summary>
		/// <param name="Points">The points awarded for the killed <see cref="Enemy"/>.</param>
		/// <param name="MoneyWorth">The money awarded for the killed <see cref="Enemy"/>.</param>
		public void AddKill(int Points, int MoneyWorth)
		{
			killCount++;
			Score += Points;
			Money += MoneyWorth;
		}

		/// <summary>
		/// Is called when an <see cref="Enemy"/> reaches the exit and adjusts the <see cref="Player"/>'s <see cref="Lives"/> accordingly and sets <see cref="Map.MapWon"/> to <see cref="Map.MapState.Lost"/>.
		/// </summary>
		public void EnemyBreach()
		{
			Lives--;
			SoundManager.PlayEnemyBreach();
			if (Lives < 0)
			{
				//TODO: Lost the damn map. You noob!!
				GameManager.CurrentMap.MapWon = Map.MapState.Lost;
			}
		}
	}
}