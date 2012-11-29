using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace MinimalisticTD
{
	/// <summary>
	/// Static class responsible for sound and music.
	/// </summary>
	public static class SoundManager
	{
		/// <summary>
		/// The ContentManager used for loading sounds.
		/// </summary>
		private static ContentManager Content;

		/// <summary>
		/// Contains the loaded sound effects.
		/// </summary>
		private static Dictionary<string, SoundEffect> sounds;

		/// <summary>
		/// Initializes the SoundManager.
		/// </summary>
		/// <param name="content">The ContentManager used for loading sounds. See <see cref="Content"/>.</param>
		public static void Initialize(ContentManager content)
		{
			Content = content;

			sounds = new Dictionary<string, SoundEffect>();
			//First check the game directory for custom sounds:
			if (!Directory.Exists("Sounds"))
			{
				Directory.CreateDirectory("Sounds");
				StreamWriter sw = File.CreateText(@"Sounds\Readme.txt");
				sw.WriteLine("Hi! You can easily add your own custom sounds to the game.\r\n" +
				"There are just two requirements:\r\n" +
				"1)They need to be .wav files to be recognized\r\n2)They need to have the correct names.\r\n" +
				"These are the supported names, which should be quite self explanatory. Note: names ending with a number theoretically support\r\n" +
				"an infinite amount of numbers, but they have to be consecutive. This means \"hit9\", \"hit10\", \"hit11\" would be okay, \r\n" +
				"but \"hit9\", \"hit11\" would not work. If the missing number is contained in the following list, however, it will work.\r\n" +
				"This means that \"hit4\", \"hit7\" would be okay because \"hit5\" and \"hit6\" are covered by the list below.\r\n\r\nSupported names:\r\n");

				sw.Write(string.Join("\r\n", Directory.GetFiles(@"Content\Sounds").Select(file => file.Replace(@"Content\Sounds\", "").Replace(".xnb", ""))));
				sw.Close();
			}
			foreach (string file in Directory.GetFiles("Sounds"))
			{
				FileInfo fi = new FileInfo(file);
				if (!fi.Name.EndsWith(".wav"))
				{
					continue;
				}
				using (FileStream fs = fi.OpenRead())
				{
					try
					{
						string effectName = fi.Name.Substring(0, fi.Name.LastIndexOf('.'));
						sounds.Add(effectName.ToLower(), SoundEffect.FromStream(fs));
						fs.Close();
					}
					catch (IOException)
					{
						Debug.WriteLine(file + " could not be read.");
						fs.Close();
					}
				}
			}

			//Now get the standard sounds and add them if there is no appropriate effect yet:
			if (!Directory.Exists(@"Content\Sounds"))
			{
				Debug.WriteLine("Sound directory was not found!");
				return;
			}
			foreach (string file in Directory.GetFiles(@"Content\Sounds"))
			{
				try
				{
					string effectName = file.Replace(@"Content\", "").Replace(".xnb", "");
					if (!sounds.ContainsKey(effectName.Replace(@"Sounds\", "").ToLower()))
					{
						sounds.Add(effectName.Replace(@"Sounds\", "").ToLower(), Content.Load<SoundEffect>(effectName));
					}
				}
				catch (IOException)
				{
					Debug.WriteLine(file + " could not be read.");
				}
			}
		}

		/// <summary>
		/// Tries to play the effect with the specified name.
		/// </summary>
		/// <param name="effectName">Name of the effect.</param>
		public static void PlayEffect(string effectName)
		{
			try
			{
				sounds[effectName.ToLower()].Play();
			}
			catch (Exception)
			{
				System.Diagnostics.Debug.WriteLine("Sound could not be played:" + effectName);
			}
		}

		/// <summary>
		/// Plays the menu click sound.
		/// </summary>
		public static void PlayMenuClick()
		{
			PlayEffect("menuClick");
		}

		/// <summary>
		/// Plays the enemy spawn sound.
		/// </summary>
		public static void PlayEnemySpawn()
		{
			PlayEffect("enemySpawn");
		}

		/// <summary>
		/// Plays the wave start sound.
		/// </summary>
		public static void PlayWaveStart()
		{
			PlayEffect("waveStart");
		}

		/// <summary>
		/// Plays the explosion sound.
		/// </summary>
		public static void PlayExplosion()
		{
			PlayEffect("explosion");
		}

		/// <summary>
		/// Plays a random hit sound.
		/// </summary>
		public static void PlayHit()
		{
			PlayEffect("hit" + GameManager.rand.Next(1, sounds.Keys.Count(k => k.StartsWith("hit"))));
		}

		/// <summary>
		/// Plays the win sound.
		/// </summary>
		public static void PlayWin()
		{
			PlayEffect("win");
		}

		/// <summary>
		/// Plays the lose sound.
		/// </summary>
		public static void PlayLose()
		{
			PlayEffect("lose");
		}

		/// <summary>
		/// Plays the buy tower sound.
		/// </summary>
		public static void PlayBuyTower()
		{
			PlayEffect("buyTower");
		}

		/// <summary>
		/// Plays the sell tower sound.
		/// </summary>
		public static void PlaySellTower()
		{
			PlayEffect("sellTower");
		}

		/// <summary>
		/// Plays the upgrade tower sound.
		/// </summary>
		public static void PlayUpgradeTower()
		{
			PlayEffect("levelUp");
		}

		/// <summary>
		/// Plays the enemy breach sound.
		/// </summary>
		public static void PlayEnemyBreach()
		{
			PlayEffect("enemyBreach");
		}
	}
}