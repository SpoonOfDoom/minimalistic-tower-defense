using System;
using Microsoft.Xna.Framework;

namespace MinimalisticTD
{
	/// <summary>
	/// A static class with extension methods.
	/// </summary>
	public static class Extensions
	{
		#region General Extensions
		/// <summary>
		/// Returns the absolute value of the specified float.
		/// </summary>
		/// <param name="f">The float.</param>
		/// <returns>The absolute value.</returns>
		public static float Absolute(this float f)
		{
			return Math.Abs(f);
		}

		/// <summary>
		/// Parses the specified string to float.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The float in this string.</returns>
		public static float ToFloat(this string text)
		{
			return float.Parse(text);
		}

		/// <summary>
		/// Parses the specified string to int.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The int in this string.</returns>
		public static int ToInt(this string text)
		{
			return int.Parse(text);
		}

		/// <summary>
		/// Casts the specified float to int.
		/// </summary>
		/// <param name="f">The float.</param>
		/// <returns>The float cast to an int.</returns>
		public static int ToInt(this float f)
		{
			return (int)f;
		}

		/// <summary>
		/// Converts the specified Point to a Vector2.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <returns>A new Vector2 with the Point's coordinates.</returns>
		public static Vector2 ToVector2(this Point point)
		{
			return new Vector2(point.X, point.Y);
		}

		/// <summary>
		/// Converts the specified Vector2 to a Point.
		/// </summary>
		/// <param name="vector2">The vector2.</param>
		/// <returns>A new Point with the Vector2's coordinates.</returns>
		public static Point ToPoint(this Vector2 vector2)
		{
			return new Point((int)vector2.X, (int)vector2.Y);
		}

		/// <summary>
		/// Parses the string to Color.
		/// </summary>
		/// <param name="colorName">Name of the color.</param>
		/// <returns>The Color in the string.</returns>
		public static Color ToColor(this string colorName)
		{
			System.Reflection.PropertyInfo info = typeof(Color).GetProperty(colorName);
			Color color = (Color)info.GetValue(null, null);
			return color;
		}
		#endregion

		#region MTD-specific Extensions
		/// <summary>
		/// Turns left from the specified direction.
		/// </summary>
		/// <param name="dir">The direction.</param>
		/// <returns>The next direction to the left.</returns>
		public static Map.Direction TurnLeft(this Map.Direction dir)
		{
			int newDir = (int)dir;
			newDir--;
			if (newDir < 0)
			{
				newDir = 3;
			}
			return (Map.Direction)newDir;
		}

		/// <summary>
		/// Turns right from the specified direction.
		/// </summary>
		/// <param name="dir">The direction.</param>
		/// <returns>The next direction to the right.</returns>
		public static Map.Direction TurnRight(this Map.Direction dir)
		{
			int newDir = (int)dir;
			newDir++;
			newDir = newDir % 4;
			return (Map.Direction)newDir;
		}

		/// <summary>
		/// Turns away from the specified direction.
		/// </summary>
		/// <param name="dir">The direction.</param>
		/// <returns>The opposite of the specified direction.</returns>
		public static Map.Direction Opposite(this Map.Direction dir)
		{
			switch (dir)
			{
				case Map.Direction.Up:
					return Map.Direction.Down;
				case Map.Direction.Right:
					return Map.Direction.Left;
				case Map.Direction.Down:
					return Map.Direction.Up;
				case Map.Direction.Left:
					return Map.Direction.Right;
			}
			return Map.Direction.Right;
		}

		/// <summary>
		/// Determines whether the specified TileType is a path. This contains normal path tiles as well as start and end tiles.
		/// </summary>
		/// <param name="type">The TileType.</param>
		/// <returns>
		///   <c>true</c> if the specified type is <see cref="Map.TileType.Path"/>, <see cref="Map.TileType.PathStart"/> or <see cref="Map.TileType.PathEnd"/>; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPath(this Map.TileType type)
		{
			return (int)type <= 2;
		}

		/// <summary>
		/// Parses the string to <see cref="Tower.DamageType"/>.
		/// </summary>
		/// <param name="e">The string.</param>
		/// <returns>The <see cref="Tower.DamageType"/> in this string.</returns>
		public static Tower.DamageType ToDamageType(this string e)
		{
			return (Tower.DamageType)Enum.Parse(typeof(Tower.DamageType), e);
		}

		/// <summary>
		/// Parses the string to <see cref="Enemy.EnemyType"/>.
		/// </summary>
		/// <param name="e">The string.</param>
		/// <returns>The <see cref="Enemy.EnemyType"/> in this string.</returns>
		public static Enemy.EnemyType ToEnemyType(this string e)
		{
			return (Enemy.EnemyType)Enum.Parse(typeof(Enemy.EnemyType), e);
		}
		#endregion MTD-specific Extensions
	}
}