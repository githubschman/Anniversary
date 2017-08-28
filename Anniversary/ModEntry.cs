using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Characters = StardewValley.Characters;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;
using System.Text;
using System.IO;
using Objects = StardewValley.Object;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

using StardewValley.Characters;
using StardewValley.Monsters;
using SObject = StardewValley.Object;

namespace Anniversary
{
	public class Anniversary : Mod
	{
	private AnniversaryInfo AnniversaryInfo;

		private ModConfig Config;
		// private ModConfig WriteInfo;
		
		private string DataFilePath => Path.Combine("data", $"{Constants.SaveFolderName}.json");
		
		public override void Entry(IModHelper helper)
		{
			this.Config = helper.ReadConfig<ModConfig>();
			// this.WriteInfo = helper.WriteJsonFile<ModConfig>(this.DataFilePath, new ModConfig());

			StardewModdingAPI.Events.TimeEvents.AfterDayStarted += Event_AfterDayStarted;
		}

		public class ModConfig
		{
			public int AnniDay { get; set; }
			public string AnniSeason { get; set; }
			public string KeyBinding { get; set; } = "O";
			Random rnd = new Random();
			String[] Seasons = {"Spring", "Summer", "Fall", "Winter"};
			
			// public ModConfig(){
			// // Player is married, but downloaded the mod after they got married. They are assigned a random anniversary.
			// 	if(AnniDay == 0 && Game1.player.isMarried()){
			// 		AnniDay = rnd.Next(1,28);
			// 	}
				
			// 	if(AnniSeason == null && Game1.player.isMarried()){
			// 		int num = rnd.Next(1,4);
			// 		AnniSeason = Seasons[num];
			// 	}
			// }	
   
		}

		// public void WriteAnniversary()
		// {
		// 	// breaking here, obviously lol
		// 	this.Helper.WriteJsonFile(this.DataFilePath, AnniversaryInfo);
		// }
		


		public void Event_AfterDayStarted(object sender, EventArgs e)
		{
				

            // AnniversaryInfo = this.Helper.ReadJsonFile<AnniversaryInfo>(this.DataFilePath) ?? new AnniversaryInfo();	

	
			Random rnd = new Random();
			String[] Seasons = {"Spring", "Summer", "Fall", "Winter"};

			// Player is married, but downloaded the mod after they got married. They are assigned a random anniversary.
			if(this.Config.AnniDay == 0){

				// && Game1.player.isMarried()
				
				this.Config.AnniDay = rnd.Next(1,28);
				int num = rnd.Next(1,4);
				this.Config.AnniSeason = Seasons[num];
				
				this.Helper.WriteJsonFile<ModConfig>(this.DataFilePath, new ModConfig());
				this.Monitor.Log("anniversary date was saved to: " + this.Config.AnniDay + this.Config.AnniSeason);
			}

			
			this.Monitor.Log("anniversary date is: " + this.Config.AnniDay + this.Config.AnniSeason);
			this.Monitor.Log("new day started");
			string spouse = Game1.player.spouse;
			
			

			// If they are married, and already have an anniversary set, check to see if anniversary is today:
			if(Game1.player.isMarried() == true && this.Config.AnniDay >= 1){
				this.Monitor.Log(Game1.player.name + " and " + spouse + "'s anniversary is on... " + this.Config.AnniDay);
				if(this.Config.AnniDay == Game1.dayOfMonth && this.Config.AnniSeason == Game1.currentSeason){
					// Game1.mailbox.Enqueue("Happy Birthday!" + Game1.);		
					this.Monitor.Log("Happy Anniversary!");	
				}
			}
		}
	}
}
