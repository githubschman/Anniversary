using System;
using StardewModdingAPI;
using StardewValley;
using System.IO;

namespace Anniversary
{
    public class Anniversary : Mod
	{
		private ModConfig Config;
		private bool eventTrigger;
		
		private string DataFilePath => Path.Combine("data", $"{Constants.SaveFolderName}.json");
		private NPC NPC;

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
		
			String[] Seasons = {"Spring", "Summer", "Fall", "Winter"};
			int daysMarried = Game1.player.daysMarried;
			float years = daysMarried / 112;
			double yearsMarried = Math.Floor(years);
			int index = 0;

			this.Monitor.Log("Days Married:" + daysMarried);

			this.Monitor.Log("Wow! You've been married for " + yearsMarried + " years!");
			
			string spouse = Game1.player.spouse;
			int numChildren = Game1.player.getNumberOfChildren();

			// Calculate the player's wedding Season and Day. It will write this to a JSON file, so the mod will only need to calculate once per player.
			if(this.Config.AnniDay == 0 && daysMarried > 1){
				
				if(Game1.currentSeason == "Summer") index=1;
				if(Game1.currentSeason == "Fall") index=2;
				if(Game1.currentSeason == "Winter") index=3;
				
				int day = daysMarried - 0;

				while(day > 28){
					index--;
					if(index < 0){
						index = 3;
					}
					day -= 28;
				}
				
				index = index > 0 ? index -= 1 : index = 3;

				this.Config.AnniDay = day;
				this.Config.AnniSeason = Seasons[index];

				this.Helper.WriteJsonFile<ModConfig>(this.DataFilePath, new ModConfig());
				this.Monitor.Log("anniversary date was saved to: " + this.Config.AnniDay + " of " + this.Config.AnniSeason);
			}

			// Enqueue the anniversary letter the day before your anniversary:
			int dayBefore = this.Config.AnniDay - 1 == 0 ? this.Config.AnniDay - 1 : 28;
			int indexBefore = index-1 < 0 ? 3 : index-1;
			string monthBefore = dayBefore == 28 ? Seasons[indexBefore] : Game1.currentSeason;
			string favething = Game1.player.favoriteThing;

			if(Game1.dayOfMonth == dayBefore && Game1.currentSeason == monthBefore){
				
				string talk = Game1.player.name + ", I can't believe we've been married for " + yearsMarried + (yearsMarried > 1 ? "years" : "year") + " or " + daysMarried + ". Yes, I've been keeping count! Every day with you is so special. And I know you love me more than you love " + favething + "! You are the greatest " + (Game1.player.IsMale ? "husband" : "wife") + (numChildren < 1 ? " in Stardew Valley!" : " and parent!");

				// Game1.mailbox.Enqueue("anniversaryTomorrow");
				Dialogue happyAnniversaryBabe = new Dialogue(talk, Game1.player.getSpouse());
				Game1.player.getSpouse().CurrentDialogue.Push(happyAnniversaryBabe);
				this.Monitor.Log("It is the day before your Anniversary!");
			}
			
			// If they are married, and already have an anniversary set, check to see if anniversary is today:
			if(daysMarried > 1 == true && this.Config.AnniDay >= 1){
				if(this.Config.AnniDay == Game1.dayOfMonth && this.Config.AnniSeason == Game1.currentSeason){
					eventTrigger = true;
					if(Context.IsWorldReady){
						Game1.showGlobalMessage("Happy " + yearsMarried + " Anniversary, " + Game1.player.name + " & " + spouse + "!");
						this.Monitor.Log("Happy Anniversary!");	
					}
				}
			}
		}

		public void Event_LocationChanged(object sender, EventArgs e)
		{
			if(Game1.currentLocation.ToString() == "Beach" && eventTrigger){
				this.Monitor.Log("Anniversary event should be triggered, y'all!");
			}
		}
		


	}
}
