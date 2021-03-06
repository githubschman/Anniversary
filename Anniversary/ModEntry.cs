﻿using System;
using StardewModdingAPI;
using StardewValley;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Anniversary
{
    public class Anniversary : Mod
	{
		private ModConfig Config;
		private bool eventTrigger;
		private string DataFilePath => Path.Combine("data", $"{Constants.SaveFolderName}.json");

		public override void Entry(IModHelper helper)
		{
			this.Config = helper.ReadConfig<ModConfig>();
			StardewModdingAPI.Events.TimeEvents.AfterDayStarted += Event_AfterDayStarted;
			StardewModdingAPI.Events.LocationEvents.CurrentLocationChanged += Event_LocationChanged;
		}

		public class ModConfig
		{
			public int AnniDay { get; set; }
			public string AnniSeason { get; set; }
			public string KeyBinding { get; set; } = "O";
		}

		public void Event_AfterDayStarted(object sender, EventArgs e)
		{

			// Get Wedding Anniversary Date (and other necessary info)
			String[] Seasons = {"spring", "summer", "fall", "winter"};
			int daysMarried = Game1.player.daysMarried;
			float years = daysMarried / 112;
			double yearsMarried = Math.Floor(years);

			// Message Information
			string favething = Game1.player.favoriteThing;
			string spouse = Game1.player.spouse;
			int numChildren = Game1.player.getNumberOfChildren();

			// Calculate the player's wedding Season and Day. It will write this to a JSON file (config.json), so the mod will only need to calculate once per player.
			// Players can also modify config.json data if they want to hardcode their anniversary date!
			int index = 0;
			if(this.Config.AnniDay == 0 && daysMarried > 1){
				
				if(Game1.currentSeason == "summer") index=1;
				if(Game1.currentSeason == "fall") index=2;
				if(Game1.currentSeason == "winter") index=3;
				
				int day = daysMarried - 0;

				while(day > 28){ // get years % daysmarried and the month by index
					index--;
					if(index < 0) index = 3;
					day -= 28;
				}
					
				// accounts for the remainder days
				if(index > 0) index = index - 1;
				else index = 3;

				this.Config.AnniDay = day;
				this.Config.AnniSeason = Seasons[index];

				this.Helper.WriteJsonFile<ModConfig>(this.DataFilePath, new ModConfig());
				this.Monitor.Log("anniversary date was saved to: " + this.Config.AnniDay + " of " + this.Config.AnniSeason);

			}	

			// Day Before Anniversary
			int dayBefore = this.Config.AnniDay - 1;
			if(dayBefore < 1) dayBefore = 28;
			int indexBefore = index-1;
			if(indexBefore < 0) index = 3;
			
			string monthBefore = Seasons[index];
			if(dayBefore == 28) monthBefore = Seasons[indexBefore];
			
			if(Game1.dayOfMonth == dayBefore && Game1.currentSeason == monthBefore){

				// Your spouse will tell you to check the mail: 
				string dayBeforeTalk = "Hey, " + Game1.player.name + "... You should check the mail, for no particular reason!";				
				Dialogue checkYourMail = new Dialogue(dayBeforeTalk, Game1.player.getSpouse());
				Game1.player.getSpouse().CurrentDialogue.Push(checkYourMail);

				// Enqueue pre-anniversary letter:
				double mailNum = yearsMarried;
				if(mailNum > 4) mailNum = 4;
				Game1.mailbox.Enqueue("AnniversaryTomorrow" + mailNum);
			}

			
			if(daysMarried > 1 && this.Config.AnniDay >= 1){
				// Check to see if the couple's anniversary is today
				if(this.Config.AnniDay == Game1.dayOfMonth && this.Config.AnniSeason == Game1.currentSeason){
					eventTrigger = true; // triggers wedding music to play when you enter town
					// Custom anniversary message to be delivered by your spouse
					string sweetTalk = Game1.player.name + ", I can't believe we've been married for " + yearsMarried + (yearsMarried > 1 ? " years" : " year") + " or " + daysMarried + " days. Yes, I've been keeping count! Every day with you is so special. And I know you love me more than you love " + favething + "! You are the greatest " + (Game1.player.IsMale ? "husband" : "wife") + (numChildren < 1 ? " in Stardew Valley!" : " and parent!$l");
					Dialogue happyAnniversaryBabe = new Dialogue(sweetTalk, Game1.player.getSpouse());
					Game1.player.getSpouse().CurrentDialogue.Push(happyAnniversaryBabe);
				}
			}
		}

		public void Event_LocationChanged(object sender, EventArgs e)
		{
			
			if(Game1.currentLocation.name == "Town" && eventTrigger && Context.IsWorldReady){
				Game1.showGlobalMessage("Memories of your wedding day come flooding back!");

				Game1.currentSong.Stop(AudioStopOptions.Immediate);
				Game1.currentSong.Stop(AudioStopOptions.AsAuthored);
				Game1.currentSong = null;
				Game1.playSound("wedding");
				
				eventTrigger = false;
			}else{
				Game1.currentSong.Resume();
			}
		}
		


	}
}
