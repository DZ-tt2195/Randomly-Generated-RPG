public static class AutoTranslate 
{ 

public static string Your_Timezone (string Time) => Translator.inst.Translate("Your_Timezone", new() {("Time", Time)});

public static string Next_Challenge (string Time) => Translator.inst.Translate("Next_Challenge", new() {("Time", Time)});

public static string Current_Date (string Month,string Day,string Year) => Translator.inst.Translate("Current_Date", new() {("Month", Month),("Day", Day),("Year", Year)});

public static string Apply_Ineffectives_Fail (string This) => Translator.inst.Translate("Apply_Ineffectives_Fail", new() {("This", This)});

public static string Super_Effective (string Num) => Translator.inst.Translate("Super_Effective", new() {("Num", Num)});

public static string Not_Effective (string Num) => Translator.inst.Translate("Not_Effective", new() {("Num", Num)});

public static string Increase_Cooldown (string Num,string Target,string MiscStat) => Translator.inst.Translate("Increase_Cooldown", new() {("Num", Num),("Target", Target),("MiscStat", MiscStat)});

public static string Decrease_Cooldown (string Num,string Target,string MiscStat) => Translator.inst.Translate("Decrease_Cooldown", new() {("Num", Num),("Target", Target),("MiscStat", MiscStat)});

public static string Apply_Cooldown (string Target,string Ability,string MiscStat) => Translator.inst.Translate("Apply_Cooldown", new() {("Target", Target),("Ability", Ability),("MiscStat", MiscStat)});

public static string Out_of_Time (string This) => Translator.inst.Translate("Out_of_Time", new() {("This", This)});

public static string Blocked_Stat_Drop (string This,string Num,string Stat) => Translator.inst.Translate("Blocked_Stat_Drop", new() {("This", This),("Num", Num),("Stat", Stat)});

public static string Blocked_Emotion (string This) => Translator.inst.Translate("Blocked_Emotion", new() {("This", This)});

public static string Blocked_Position (string This) => Translator.inst.Translate("Blocked_Position", new() {("This", This)});

public static string Miss_Turn (string This) => Translator.inst.Translate("Miss_Turn", new() {("This", This)});

public static string Gain_Extra_Ability (string This,string Num) => Translator.inst.Translate("Gain_Extra_Ability", new() {("This", This),("Num", Num)});

public static string Use_Extra_Ability (string This) => Translator.inst.Translate("Use_Extra_Ability", new() {("This", This)});

public static string Defeat_Waves (string Num) => Translator.inst.Translate("Defeat_Waves", new() {("Num", Num)});

public static string Round (string Num) => Translator.inst.Translate("Round", new() {("Num", Num)});

public static string Wave (string Num,string Max) => Translator.inst.Translate("Wave", new() {("Num", Num),("Max", Max)});

public static string Enter_Fight (string This) => Translator.inst.Translate("Enter_Fight", new() {("This", This)});

public static string Waves_Survived (string Num) => Translator.inst.Translate("Waves_Survived", new() {("Num", Num)});

public static string Time_Taken (string Time) => Translator.inst.Translate("Time_Taken", new() {("Time", Time)});

public static string Increase_Stat (string This,string Num,string Stat) => Translator.inst.Translate("Increase_Stat", new() {("This", This),("Num", Num),("Stat", Stat)});

public static string Decrease_Stat (string This,string Num,string Stat) => Translator.inst.Translate("Decrease_Stat", new() {("This", This),("Num", Num),("Stat", Stat)});

public static string Become_New (string This,string Change) => Translator.inst.Translate("Become_New", new() {("This", This),("Change", Change)});

public static string Change_Status (string This,string Status,string Num) => Translator.inst.Translate("Change_Status", new() {("This", This),("Status", Status),("Num", Num)});

public static string Died (string This) => Translator.inst.Translate("Died", new() {("This", This)});

public static string Revived (string This) => Translator.inst.Translate("Revived", new() {("This", This)});

public static string Emotion_Effect (string This,string Emotion) => Translator.inst.Translate("Emotion_Effect", new() {("This", This),("Emotion", Emotion)});

public static string Choose_Ability (string This) => Translator.inst.Translate("Choose_Ability", new() {("This", This)});

public static string Must_Choose_Targeted (string Target) => Translator.inst.Translate("Must_Choose_Targeted", new() {("Target", Target)});

public static string Choose_One_Player (string Ability) => Translator.inst.Translate("Choose_One_Player", new() {("Ability", Ability)});

public static string Choose_Another_Player (string Ability) => Translator.inst.Translate("Choose_Another_Player", new() {("Ability", Ability)});

public static string Choose_An_Enemy (string Ability) => Translator.inst.Translate("Choose_An_Enemy", new() {("Ability", Ability)});

public static string Confirm_No_Target (string Ability) => Translator.inst.Translate("Confirm_No_Target", new() {("Ability", Ability)});

public static string Confirm_Target (string Ability) => Translator.inst.Translate("Confirm_Target", new() {("Ability", Ability)});

public static string Battle_Cry_Text (string DefenseStat) => Translator.inst.Translate("Battle_Cry_Text", new() {("DefenseStat", DefenseStat)});

public static string Battle_Cry_Log (string This,string Target) => Translator.inst.Translate("Battle_Cry_Log", new() {("This", This),("Target", Target)});

public static string Cheer_Text (string MiscStat) => Translator.inst.Translate("Cheer_Text", new() {("MiscStat", MiscStat)});

public static string Cheer_Log (string This,string Target) => Translator.inst.Translate("Cheer_Log", new() {("This", This),("Target", Target)});

public static string Daze_Text (string DefenseStat) => Translator.inst.Translate("Daze_Text", new() {("DefenseStat", DefenseStat)});

public static string Daze_Log (string This,string Target) => Translator.inst.Translate("Daze_Log", new() {("This", This),("Target", Target)});

public static string Desperation_Text (string Num) => Translator.inst.Translate("Desperation_Text", new() {("Num", Num)});

public static string Desperation_Log (string This,string Target) => Translator.inst.Translate("Desperation_Log", new() {("This", This),("Target", Target)});

public static string Embarass_Text (string Num) => Translator.inst.Translate("Embarass_Text", new() {("Num", Num)});

public static string Embarass_Log (string This,string Target) => Translator.inst.Translate("Embarass_Log", new() {("This", This),("Target", Target)});

public static string Force_Text (string Num,string MiscStat) => Translator.inst.Translate("Force_Text", new() {("Num", Num),("MiscStat", MiscStat)});

public static string Force_Log (string This,string Target) => Translator.inst.Translate("Force_Log", new() {("This", This),("Target", Target)});

public static string Impale_Text (string Num,string MiscStat) => Translator.inst.Translate("Impale_Text", new() {("Num", Num),("MiscStat", MiscStat)});

public static string Impale_Log (string This,string Target) => Translator.inst.Translate("Impale_Log", new() {("This", This),("Target", Target)});

public static string Intimidate_Log (string This,string Target) => Translator.inst.Translate("Intimidate_Log", new() {("This", This),("Target", Target)});

public static string Joust_Text (string Num,string PowerStat) => Translator.inst.Translate("Joust_Text", new() {("Num", Num),("PowerStat", PowerStat)});

public static string Joust_Log (string This,string Target) => Translator.inst.Translate("Joust_Log", new() {("This", This),("Target", Target)});

public static string Knock_Down_Text (string Num,string DefenseStat) => Translator.inst.Translate("Knock_Down_Text", new() {("Num", Num),("DefenseStat", DefenseStat)});

public static string Knock_Down_Log (string This,string Target) => Translator.inst.Translate("Knock_Down_Log", new() {("This", This),("Target", Target)});

public static string Meditate_Text (string MiscStat) => Translator.inst.Translate("Meditate_Text", new() {("MiscStat", MiscStat)});

public static string Meditate_Log (string This,string Target) => Translator.inst.Translate("Meditate_Log", new() {("This", This),("Target", Target)});

public static string Mirror_Text (string Num) => Translator.inst.Translate("Mirror_Text", new() {("Num", Num)});

public static string Mirror_Log (string This,string Target) => Translator.inst.Translate("Mirror_Log", new() {("This", This),("Target", Target)});

public static string Mock_Text (string Num) => Translator.inst.Translate("Mock_Text", new() {("Num", Num)});

public static string Mock_Log (string This,string Target) => Translator.inst.Translate("Mock_Log", new() {("This", This),("Target", Target)});

public static string Neutralize_Text (string Num) => Translator.inst.Translate("Neutralize_Text", new() {("Num", Num)});

public static string Neutralize_Log (string This,string Target) => Translator.inst.Translate("Neutralize_Log", new() {("This", This),("Target", Target)});

public static string Redirect_Text (string DefenseStat,string MiscStat) => Translator.inst.Translate("Redirect_Text", new() {("DefenseStat", DefenseStat),("MiscStat", MiscStat)});

public static string Redirect_Log (string This,string Target) => Translator.inst.Translate("Redirect_Log", new() {("This", This),("Target", Target)});

public static string Strike_Text (string DefenseStat,string Num) => Translator.inst.Translate("Strike_Text", new() {("DefenseStat", DefenseStat),("Num", Num)});

public static string Strike_Log (string This,string Target) => Translator.inst.Translate("Strike_Log", new() {("This", This),("Target", Target)});

public static string Steal_Text (string DefenseStat) => Translator.inst.Translate("Steal_Text", new() {("DefenseStat", DefenseStat)});

public static string Steal_Log (string This,string Target) => Translator.inst.Translate("Steal_Log", new() {("This", This),("Target", Target)});

public static string Surprise_Text (string Num) => Translator.inst.Translate("Surprise_Text", new() {("Num", Num)});

public static string Surprise_Log (string This,string Target) => Translator.inst.Translate("Surprise_Log", new() {("This", This),("Target", Target)});

public static string Twirl_Text (string Num) => Translator.inst.Translate("Twirl_Text", new() {("Num", Num)});

public static string Twirl_Log (string This,string Target) => Translator.inst.Translate("Twirl_Log", new() {("This", This),("Target", Target)});

public static string Upset_Text (string DefenseStat) => Translator.inst.Translate("Upset_Text", new() {("DefenseStat", DefenseStat)});

public static string Upset_Log (string This,string Target) => Translator.inst.Translate("Upset_Log", new() {("This", This),("Target", Target)});

public static string Immobilize_Text (string Num,string MiscStat) => Translator.inst.Translate("Immobilize_Text", new() {("Num", Num),("MiscStat", MiscStat)});

public static string Immobilize_Log (string This,string Target) => Translator.inst.Translate("Immobilize_Log", new() {("This", This),("Target", Target)});

public static string Air_Combat_Text (string PowerStat) => Translator.inst.Translate("Air_Combat_Text", new() {("PowerStat", PowerStat)});

public static string Air_Combat_Log (string This) => Translator.inst.Translate("Air_Combat_Log", new() {("This", This)});

public static string Assist_Text (string Num,string MiscStat) => Translator.inst.Translate("Assist_Text", new() {("Num", Num),("MiscStat", MiscStat)});

public static string Assist_Log (string This,string Target) => Translator.inst.Translate("Assist_Log", new() {("This", This),("Target", Target)});

public static string Calm_Down_Text (string Num) => Translator.inst.Translate("Calm_Down_Text", new() {("Num", Num)});

public static string Calm_Down_Log (string This) => Translator.inst.Translate("Calm_Down_Log", new() {("This", This)});

public static string Crash_Land_Text (string DefenseStat) => Translator.inst.Translate("Crash_Land_Text", new() {("DefenseStat", DefenseStat)});

public static string Crash_Land_Log (string This,string Target) => Translator.inst.Translate("Crash_Land_Log", new() {("This", This),("Target", Target)});

public static string Enrage_Text (string Num) => Translator.inst.Translate("Enrage_Text", new() {("Num", Num)});

public static string Enrage_Log (string This,string Target) => Translator.inst.Translate("Enrage_Log", new() {("This", This),("Target", Target)});

public static string Exhaust_Text (string PowerStat) => Translator.inst.Translate("Exhaust_Text", new() {("PowerStat", PowerStat)});

public static string Exhaust_Log (string This,string Target) => Translator.inst.Translate("Exhaust_Log", new() {("This", This),("Target", Target)});

public static string Exorcism_Text (string DefenseStat,string Num) => Translator.inst.Translate("Exorcism_Text", new() {("DefenseStat", DefenseStat),("Num", Num)});

public static string Exorcism_Log (string This,string Target) => Translator.inst.Translate("Exorcism_Log", new() {("This", This),("Target", Target)});

public static string Gift_of_Flight_Log (string This,string Target) => Translator.inst.Translate("Gift_of_Flight_Log", new() {("This", This),("Target", Target)});

public static string Hide_Text (string PowerStat,string Num) => Translator.inst.Translate("Hide_Text", new() {("PowerStat", PowerStat),("Num", Num)});

public static string Hide_Log (string This,string Target) => Translator.inst.Translate("Hide_Log", new() {("This", This),("Target", Target)});

public static string Immunity_Text (string Num,string MiscStat) => Translator.inst.Translate("Immunity_Text", new() {("Num", Num),("MiscStat", MiscStat)});

public static string Immunity_Log (string This,string Target) => Translator.inst.Translate("Immunity_Log", new() {("This", This),("Target", Target)});

public static string Joy_Text (string Num) => Translator.inst.Translate("Joy_Text", new() {("Num", Num)});

public static string Joy_Log (string This,string Target) => Translator.inst.Translate("Joy_Log", new() {("This", This),("Target", Target)});

public static string Lift_Up_Text (string Num) => Translator.inst.Translate("Lift_Up_Text", new() {("Num", Num)});

public static string Lift_Up_Log (string This,string Target) => Translator.inst.Translate("Lift_Up_Log", new() {("This", This),("Target", Target)});

public static string Motivate_Text (string Num,string MiscStat) => Translator.inst.Translate("Motivate_Text", new() {("Num", Num),("MiscStat", MiscStat)});

public static string Motivate_Log (string This,string Target) => Translator.inst.Translate("Motivate_Log", new() {("This", This),("Target", Target)});

public static string Overheal_Text (string MiscStat,string Num) => Translator.inst.Translate("Overheal_Text", new() {("MiscStat", MiscStat),("Num", Num)});

public static string Overheal_Log (string This) => Translator.inst.Translate("Overheal_Log", new() {("This", This)});

public static string Petrify_Text (string MiscStat) => Translator.inst.Translate("Petrify_Text", new() {("MiscStat", MiscStat)});

public static string Petrify_Log (string This,string Target) => Translator.inst.Translate("Petrify_Log", new() {("This", This),("Target", Target)});

public static string Plummet_Log (string This,string Target) => Translator.inst.Translate("Plummet_Log", new() {("This", This),("Target", Target)});

public static string Retreat_Log (string This,string Target) => Translator.inst.Translate("Retreat_Log", new() {("This", This),("Target", Target)});

public static string Security_Text (string DefenseStat) => Translator.inst.Translate("Security_Text", new() {("DefenseStat", DefenseStat)});

public static string Security_Log (string This,string Target) => Translator.inst.Translate("Security_Log", new() {("This", This),("Target", Target)});

public static string Soft_Landing_Text (string Num) => Translator.inst.Translate("Soft_Landing_Text", new() {("Num", Num)});

public static string Soft_Landing_Log (string This,string Target) => Translator.inst.Translate("Soft_Landing_Log", new() {("This", This),("Target", Target)});

public static string Tailwinds_Text (string Num,string DefenseStat) => Translator.inst.Translate("Tailwinds_Text", new() {("Num", Num),("DefenseStat", DefenseStat)});

public static string Tailwinds_Log (string This) => Translator.inst.Translate("Tailwinds_Log", new() {("This", This)});

public static string Team_Up_Text (string Num,string PowerStat) => Translator.inst.Translate("Team_Up_Text", new() {("Num", Num),("PowerStat", PowerStat)});

public static string Team_Up_Log (string This) => Translator.inst.Translate("Team_Up_Log", new() {("This", This)});

public static string Above_Danger_Text (string MiscStat) => Translator.inst.Translate("Above_Danger_Text", new() {("MiscStat", MiscStat)});

public static string Above_Danger_Log (string This,string Target) => Translator.inst.Translate("Above_Danger_Log", new() {("This", This),("Target", Target)});

public static string Bad_Omens_Text (string Num,string PowerStat) => Translator.inst.Translate("Bad_Omens_Text", new() {("Num", Num),("PowerStat", PowerStat)});

public static string Bad_Omens_Log (string This) => Translator.inst.Translate("Bad_Omens_Log", new() {("This", This)});

public static string Bounce_Text (string Num) => Translator.inst.Translate("Bounce_Text", new() {("Num", Num)});

public static string Bounce_Log (string This) => Translator.inst.Translate("Bounce_Log", new() {("This", This)});

public static string Chill_Text (string Num,string MiscStat) => Translator.inst.Translate("Chill_Text", new() {("Num", Num),("MiscStat", MiscStat)});

public static string Chill_Log (string This) => Translator.inst.Translate("Chill_Log", new() {("This", This)});

public static string Accelerate_Text (string PowerStat,string MiscStat) => Translator.inst.Translate("Accelerate_Text", new() {("PowerStat", PowerStat),("MiscStat", MiscStat)});

public static string Accelerate_Log (string This,string Target) => Translator.inst.Translate("Accelerate_Log", new() {("This", This),("Target", Target)});

public static string Flood_Text (string Num) => Translator.inst.Translate("Flood_Text", new() {("Num", Num)});

public static string Flood_Log (string This) => Translator.inst.Translate("Flood_Log", new() {("This", This)});

public static string Gust_Text (string PowerStat) => Translator.inst.Translate("Gust_Text", new() {("PowerStat", PowerStat)});

public static string Gust_Log (string This,string Target) => Translator.inst.Translate("Gust_Log", new() {("This", This),("Target", Target)});

public static string Headwinds_Text (string Num,string PowerStat) => Translator.inst.Translate("Headwinds_Text", new() {("Num", Num),("PowerStat", PowerStat)});

public static string Headwinds_Log (string This) => Translator.inst.Translate("Headwinds_Log", new() {("This", This)});

public static string Crown_Text (string PowerStat,string MiscStat) => Translator.inst.Translate("Crown_Text", new() {("PowerStat", PowerStat),("MiscStat", MiscStat)});

public static string Crown_Log (string This,string Target) => Translator.inst.Translate("Crown_Log", new() {("This", This),("Target", Target)});

public static string Storm_Text (string Num) => Translator.inst.Translate("Storm_Text", new() {("Num", Num)});

public static string Storm_Log (string This) => Translator.inst.Translate("Storm_Log", new() {("This", This)});

public static string Manipulate_Time_Text (string Num,string MiscStat) => Translator.inst.Translate("Manipulate_Time_Text", new() {("Num", Num),("MiscStat", MiscStat)});

public static string Manipulate_Time_Log (string This) => Translator.inst.Translate("Manipulate_Time_Log", new() {("This", This)});

public static string Bind_Text (string MiscStat) => Translator.inst.Translate("Bind_Text", new() {("MiscStat", MiscStat)});

public static string Bind_Log (string This,string Target) => Translator.inst.Translate("Bind_Log", new() {("This", This),("Target", Target)});

public static string Punch_Up_Text (string Num,string MiscStat) => Translator.inst.Translate("Punch_Up_Text", new() {("Num", Num),("MiscStat", MiscStat)});

public static string Punch_Up_Log (string This) => Translator.inst.Translate("Punch_Up_Log", new() {("This", This)});

public static string Quick_Attack_Text (string PowerStat,string Num) => Translator.inst.Translate("Quick_Attack_Text", new() {("PowerStat", PowerStat),("Num", Num)});

public static string Quick_Attack_Log (string This,string Target) => Translator.inst.Translate("Quick_Attack_Log", new() {("This", This),("Target", Target)});

public static string Readjust_Log (string This) => Translator.inst.Translate("Readjust_Log", new() {("This", This)});

public static string Restrain_Text (string Num,string MiscStat) => Translator.inst.Translate("Restrain_Text", new() {("Num", Num),("MiscStat", MiscStat)});

public static string Restrain_Log (string This) => Translator.inst.Translate("Restrain_Log", new() {("This", This)});

public static string Shockwave_Text (string Num,string Sec) => Translator.inst.Translate("Shockwave_Text", new() {("Num", Num),("Sec", Sec)});

public static string Shockwave_Log (string This) => Translator.inst.Translate("Shockwave_Log", new() {("This", This)});

public static string Stalactites_Text (string Num) => Translator.inst.Translate("Stalactites_Text", new() {("Num", Num)});

public static string Stalactites_Log (string This) => Translator.inst.Translate("Stalactites_Log", new() {("This", This)});

public static string Team_Attack_Text (string Num) => Translator.inst.Translate("Team_Attack_Text", new() {("Num", Num)});

public static string Team_Attack_Log (string This) => Translator.inst.Translate("Team_Attack_Log", new() {("This", This)});

public static string Touchdown_Text (string PowerStat) => Translator.inst.Translate("Touchdown_Text", new() {("PowerStat", PowerStat)});

public static string Touchdown_Log (string This,string Target) => Translator.inst.Translate("Touchdown_Log", new() {("This", This),("Target", Target)});

public static string Warp_Text (string PowerStat) => Translator.inst.Translate("Warp_Text", new() {("PowerStat", PowerStat)});

public static string Warp_Log (string This,string Target) => Translator.inst.Translate("Warp_Log", new() {("This", This),("Target", Target)});

public static string Revive_Text (string Num) => Translator.inst.Translate("Revive_Text", new() {("Num", Num)});

public static string Revive_Log (string This,string Target) => Translator.inst.Translate("Revive_Log", new() {("This", This),("Target", Target)});

public static string Skip_Turn_Log (string This) => Translator.inst.Translate("Skip_Turn_Log", new() {("This", This)});

public static string Quick_Shot_Text (string Num) => Translator.inst.Translate("Quick_Shot_Text", new() {("Num", Num)});

public static string Quick_Shot_Log (string This,string Target) => Translator.inst.Translate("Quick_Shot_Log", new() {("This", This),("Target", Target)});

public static string Bullseye_Text (string PowerStat,string Num) => Translator.inst.Translate("Bullseye_Text", new() {("PowerStat", PowerStat),("Num", Num)});

public static string Bullseye_Log (string This,string Target) => Translator.inst.Translate("Bullseye_Log", new() {("This", This),("Target", Target)});

public static string Volley_Text (string Num) => Translator.inst.Translate("Volley_Text", new() {("Num", Num)});

public static string Volley_Log (string This) => Translator.inst.Translate("Volley_Log", new() {("This", This)});

public static string Focus_Text (string DefenseStat) => Translator.inst.Translate("Focus_Text", new() {("DefenseStat", DefenseStat)});

public static string Focus_Log (string This) => Translator.inst.Translate("Focus_Log", new() {("This", This)});

public static string Fury_Text (string Num) => Translator.inst.Translate("Fury_Text", new() {("Num", Num)});

public static string Fury_Log (string This,string Target) => Translator.inst.Translate("Fury_Log", new() {("This", This),("Target", Target)});

public static string Savagery_Text (string PowerStat) => Translator.inst.Translate("Savagery_Text", new() {("PowerStat", PowerStat)});

public static string Savagery_Log (string This) => Translator.inst.Translate("Savagery_Log", new() {("This", This)});

public static string Brawl_Text (string MiscStat) => Translator.inst.Translate("Brawl_Text", new() {("MiscStat", MiscStat)});

public static string Brawl_Log (string This,string Target) => Translator.inst.Translate("Brawl_Log", new() {("This", This),("Target", Target)});

public static string Camp_Text (string MiscStat) => Translator.inst.Translate("Camp_Text", new() {("MiscStat", MiscStat)});

public static string Camp_Log (string This) => Translator.inst.Translate("Camp_Log", new() {("This", This)});

public static string Drain_Text (string Num) => Translator.inst.Translate("Drain_Text", new() {("Num", Num)});

public static string Drain_Log (string This,string Target) => Translator.inst.Translate("Drain_Log", new() {("This", This),("Target", Target)});

public static string Screech_Text (string DefenseStat) => Translator.inst.Translate("Screech_Text", new() {("DefenseStat", DefenseStat)});

public static string Screech_Log (string This) => Translator.inst.Translate("Screech_Log", new() {("This", This)});

public static string Hunting_Season_Text (string PowerStat) => Translator.inst.Translate("Hunting_Season_Text", new() {("PowerStat", PowerStat)});

public static string Hunting_Season_Log (string This) => Translator.inst.Translate("Hunting_Season_Log", new() {("This", This)});

public static string Bloodthirst_Text (string MiscStat) => Translator.inst.Translate("Bloodthirst_Text", new() {("MiscStat", MiscStat)});

public static string Bloodthirst_Log (string This) => Translator.inst.Translate("Bloodthirst_Log", new() {("This", This)});

public static string Sting_Text (string MiscStat) => Translator.inst.Translate("Sting_Text", new() {("MiscStat", MiscStat)});

public static string Sting_Log (string This,string Target) => Translator.inst.Translate("Sting_Log", new() {("This", This),("Target", Target)});

public static string Honey_Text (string Num) => Translator.inst.Translate("Honey_Text", new() {("Num", Num)});

public static string Honey_Log (string This,string Target) => Translator.inst.Translate("Honey_Log", new() {("This", This),("Target", Target)});

public static string Protect_the_Queen_Text (string MiscStat) => Translator.inst.Translate("Protect_the_Queen_Text", new() {("MiscStat", MiscStat)});

public static string Protect_the_Queen_Log (string This,string Target) => Translator.inst.Translate("Protect_the_Queen_Log", new() {("This", This),("Target", Target)});

public static string Support_Text (string MiscStat) => Translator.inst.Translate("Support_Text", new() {("MiscStat", MiscStat)});

public static string Support_Log (string This,string Target) => Translator.inst.Translate("Support_Log", new() {("This", This),("Target", Target)});

public static string Charge_Text (string Num) => Translator.inst.Translate("Charge_Text", new() {("Num", Num)});

public static string Charge_Log (string This,string Target) => Translator.inst.Translate("Charge_Log", new() {("This", This),("Target", Target)});

public static string Milk_Text (string Num) => Translator.inst.Translate("Milk_Text", new() {("Num", Num)});

public static string Milk_Log (string This,string Target) => Translator.inst.Translate("Milk_Log", new() {("This", This),("Target", Target)});

public static string Graze_Text (string Num) => Translator.inst.Translate("Graze_Text", new() {("Num", Num)});

public static string Graze_Log (string This) => Translator.inst.Translate("Graze_Log", new() {("This", This)});

public static string Bellow_Text (string DefenseStat,string MiscStat) => Translator.inst.Translate("Bellow_Text", new() {("DefenseStat", DefenseStat),("MiscStat", MiscStat)});

public static string Bellow_Log (string This) => Translator.inst.Translate("Bellow_Log", new() {("This", This)});

public static string Peck_Text (string Num) => Translator.inst.Translate("Peck_Text", new() {("Num", Num)});

public static string Peck_Log (string This,string Target) => Translator.inst.Translate("Peck_Log", new() {("This", This),("Target", Target)});

public static string Dark_Energy_Text (string Num) => Translator.inst.Translate("Dark_Energy_Text", new() {("Num", Num)});

public static string Dark_Energy_Log (string This) => Translator.inst.Translate("Dark_Energy_Log", new() {("This", This)});

public static string Betrayal_Text (string Num,string MiscStat,string PowerStat) => Translator.inst.Translate("Betrayal_Text", new() {("Num", Num),("MiscStat", MiscStat),("PowerStat", PowerStat)});

public static string Betrayal_Log (string This,string Target) => Translator.inst.Translate("Betrayal_Log", new() {("This", This),("Target", Target)});

public static string Ritual_Text (string MiscStat) => Translator.inst.Translate("Ritual_Text", new() {("MiscStat", MiscStat)});

public static string Ritual_Log (string This,string Target) => Translator.inst.Translate("Ritual_Log", new() {("This", This),("Target", Target)});

public static string Demand_Energy_Text (string PowerStat) => Translator.inst.Translate("Demand_Energy_Text", new() {("PowerStat", PowerStat)});

public static string Demand_Energy_Log (string This) => Translator.inst.Translate("Demand_Energy_Log", new() {("This", This)});

public static string Firebreath_Text (string Num) => Translator.inst.Translate("Firebreath_Text", new() {("Num", Num)});

public static string Firebreath_Log (string This) => Translator.inst.Translate("Firebreath_Log", new() {("This", This)});

public static string Roost_Text (string Num) => Translator.inst.Translate("Roost_Text", new() {("Num", Num)});

public static string Roost_Log (string This) => Translator.inst.Translate("Roost_Log", new() {("This", This)});

public static string Take_Flight_Log (string This) => Translator.inst.Translate("Take_Flight_Log", new() {("This", This)});

public static string Roar_Text (string PowerStat) => Translator.inst.Translate("Roar_Text", new() {("PowerStat", PowerStat)});

public static string Roar_Log (string This) => Translator.inst.Translate("Roar_Log", new() {("This", This)});

public static string Sound_Waves_Text (string Num) => Translator.inst.Translate("Sound_Waves_Text", new() {("Num", Num)});

public static string Sound_Waves_Log (string This) => Translator.inst.Translate("Sound_Waves_Log", new() {("This", This)});

public static string Alarm_Text (string MiscStat) => Translator.inst.Translate("Alarm_Text", new() {("MiscStat", MiscStat)});

public static string Alarm_Log (string This) => Translator.inst.Translate("Alarm_Log", new() {("This", This)});

public static string Jumpscare_Text (string Num) => Translator.inst.Translate("Jumpscare_Text", new() {("Num", Num)});

public static string Jumpscare_Log (string This,string Target) => Translator.inst.Translate("Jumpscare_Log", new() {("This", This),("Target", Target)});

public static string Lament_Log (string This,string Target) => Translator.inst.Translate("Lament_Log", new() {("This", This),("Target", Target)});

public static string Possession_Text (string PowerStat) => Translator.inst.Translate("Possession_Text", new() {("PowerStat", PowerStat)});

public static string Possession_Log (string This,string Target) => Translator.inst.Translate("Possession_Log", new() {("This", This),("Target", Target)});

public static string Release_Text (string MiscStat) => Translator.inst.Translate("Release_Text", new() {("MiscStat", MiscStat)});

public static string Release_Log (string This,string Target) => Translator.inst.Translate("Release_Log", new() {("This", This),("Target", Target)});

public static string Flag_Whack_Text (string Num) => Translator.inst.Translate("Flag_Whack_Text", new() {("Num", Num)});

public static string Flag_Whack_Log (string This,string Target) => Translator.inst.Translate("Flag_Whack_Log", new() {("This", This),("Target", Target)});

public static string Announce_Text (string MiscStat) => Translator.inst.Translate("Announce_Text", new() {("MiscStat", MiscStat)});

public static string Announce_Log (string This) => Translator.inst.Translate("Announce_Log", new() {("This", This)});

public static string Scratch_Text (string Num) => Translator.inst.Translate("Scratch_Text", new() {("Num", Num)});

public static string Scratch_Log (string This,string Target) => Translator.inst.Translate("Scratch_Log", new() {("This", This),("Target", Target)});

public static string Stuck_in_a_Tree_Log (string This) => Translator.inst.Translate("Stuck_in_a_Tree_Log", new() {("This", This)});

public static string Land_Feet_First_Text (string Num) => Translator.inst.Translate("Land_Feet_First_Text", new() {("Num", Num)});

public static string Land_Feet_First_Log (string This) => Translator.inst.Translate("Land_Feet_First_Log", new() {("This", This)});

public static string Lure_Text (string MiscStat) => Translator.inst.Translate("Lure_Text", new() {("MiscStat", MiscStat)});

public static string Lure_Log (string This,string Target) => Translator.inst.Translate("Lure_Log", new() {("This", This),("Target", Target)});

public static string Prank_Text (string Num) => Translator.inst.Translate("Prank_Text", new() {("Num", Num)});

public static string Prank_Log (string This) => Translator.inst.Translate("Prank_Log", new() {("This", This)});

public static string Caught_at_a_Good_Time_Text (string PowerStat,string DefenseStat) => Translator.inst.Translate("Caught_at_a_Good_Time_Text", new() {("PowerStat", PowerStat),("DefenseStat", DefenseStat)});

public static string Caught_at_a_Good_Time_Log (string This) => Translator.inst.Translate("Caught_at_a_Good_Time_Log", new() {("This", This)});

public static string Assassinate_Text (string Num) => Translator.inst.Translate("Assassinate_Text", new() {("Num", Num)});

public static string Assassinate_Log (string This,string Target) => Translator.inst.Translate("Assassinate_Log", new() {("This", This),("Target", Target)});

public static string Shuriken_Text (string PowerStat,string DefenseStat) => Translator.inst.Translate("Shuriken_Text", new() {("PowerStat", PowerStat),("DefenseStat", DefenseStat)});

public static string Shuriken_Log (string This,string Target) => Translator.inst.Translate("Shuriken_Log", new() {("This", This),("Target", Target)});

public static string Into_Shadows_Text (string MiscStat) => Translator.inst.Translate("Into_Shadows_Text", new() {("MiscStat", MiscStat)});

public static string Into_Shadows_Log (string This) => Translator.inst.Translate("Into_Shadows_Log", new() {("This", This)});

public static string Wall_Climb_Text (string DefenseStat) => Translator.inst.Translate("Wall_Climb_Text", new() {("DefenseStat", DefenseStat)});

public static string Wall_Climb_Log (string This) => Translator.inst.Translate("Wall_Climb_Log", new() {("This", This)});

public static string Swing_Text (string Num) => Translator.inst.Translate("Swing_Text", new() {("Num", Num)});

public static string Swing_Log (string This,string Target) => Translator.inst.Translate("Swing_Log", new() {("This", This),("Target", Target)});

public static string Fight_Text (string Num) => Translator.inst.Translate("Fight_Text", new() {("Num", Num)});

public static string Fight_Log (string This,string Target) => Translator.inst.Translate("Fight_Log", new() {("This", This),("Target", Target)});

public static string Share_Drinks_Text (string Num) => Translator.inst.Translate("Share_Drinks_Text", new() {("Num", Num)});

public static string Share_Drinks_Log (string This) => Translator.inst.Translate("Share_Drinks_Log", new() {("This", This)});

public static string Share_Snacks_Text (string MiscStat) => Translator.inst.Translate("Share_Snacks_Text", new() {("MiscStat", MiscStat)});

public static string Share_Snacks_Log (string This) => Translator.inst.Translate("Share_Snacks_Log", new() {("This", This)});

public static string Drinking_Game_Log (string This) => Translator.inst.Translate("Drinking_Game_Log", new() {("This", This)});

public static string Will_o_Wisp_Text (string Num) => Translator.inst.Translate("Will_o_Wisp_Text", new() {("Num", Num)});

public static string Will_o_Wisp_Log (string This,string Target) => Translator.inst.Translate("Will_o_Wisp_Log", new() {("This", This),("Target", Target)});

public static string Cast_Happiness_Log (string This) => Translator.inst.Translate("Cast_Happiness_Log", new() {("This", This)});

public static string Cast_Sadness_Log (string This) => Translator.inst.Translate("Cast_Sadness_Log", new() {("This", This)});

public static string Cast_Anger_Log (string This) => Translator.inst.Translate("Cast_Anger_Log", new() {("This", This)});

public static string Collision_Text (string Num) => Translator.inst.Translate("Collision_Text", new() {("Num", Num)});

public static string Collision_Log (string This) => Translator.inst.Translate("Collision_Log", new() {("This", This)});

public static string Drop_Text (string Num) => Translator.inst.Translate("Drop_Text", new() {("Num", Num)});

public static string Drop_Log (string This) => Translator.inst.Translate("Drop_Log", new() {("This", This)});

public static string Dangle_Text (string DefenseStat) => Translator.inst.Translate("Dangle_Text", new() {("DefenseStat", DefenseStat)});

public static string Dangle_Log (string This) => Translator.inst.Translate("Dangle_Log", new() {("This", This)});

public static string Control_Text (string MiscStat) => Translator.inst.Translate("Control_Text", new() {("MiscStat", MiscStat)});

public static string Control_Log (string This) => Translator.inst.Translate("Control_Log", new() {("This", This)});

public static string Bite_Text (string Num) => Translator.inst.Translate("Bite_Text", new() {("Num", Num)});

public static string Bite_Log (string This,string Target) => Translator.inst.Translate("Bite_Log", new() {("This", This),("Target", Target)});

public static string Leap_Text (string Num) => Translator.inst.Translate("Leap_Text", new() {("Num", Num)});

public static string Leap_Log (string This,string Target) => Translator.inst.Translate("Leap_Log", new() {("This", This),("Target", Target)});

public static string Bark_Text (string DefenseStat) => Translator.inst.Translate("Bark_Text", new() {("DefenseStat", DefenseStat)});

public static string Bark_Log (string This) => Translator.inst.Translate("Bark_Log", new() {("This", This)});

public static string Cuddle_Text (string MiscStat,string DefenseStat) => Translator.inst.Translate("Cuddle_Text", new() {("MiscStat", MiscStat),("DefenseStat", DefenseStat)});

public static string Cuddle_Log (string This,string Target) => Translator.inst.Translate("Cuddle_Log", new() {("This", This),("Target", Target)});

public static string Swarm_Text (string Num,string DefenseStat) => Translator.inst.Translate("Swarm_Text", new() {("Num", Num),("DefenseStat", DefenseStat)});

public static string Swarm_Log (string This,string Target) => Translator.inst.Translate("Swarm_Log", new() {("This", This),("Target", Target)});

public static string Strength_in_Numbers_Log (string This) => Translator.inst.Translate("Strength_in_Numbers_Log", new() {("This", This)});

public static string Order_Text (string Num,string MiscStat) => Translator.inst.Translate("Order_Text", new() {("Num", Num),("MiscStat", MiscStat)});

public static string Order_Log (string This,string Target) => Translator.inst.Translate("Order_Log", new() {("This", This),("Target", Target)});

public static string Promote_Text (string MiscStat,string Num) => Translator.inst.Translate("Promote_Text", new() {("MiscStat", MiscStat),("Num", Num)});

public static string Promote_Log (string This,string Target) => Translator.inst.Translate("Promote_Log", new() {("This", This),("Target", Target)});

public static string Call_Text (string MiscStat) => Translator.inst.Translate("Call_Text", new() {("MiscStat", MiscStat)});

public static string Call_Log (string This) => Translator.inst.Translate("Call_Log", new() {("This", This)});

public static string Procession_Text (string PowerStat) => Translator.inst.Translate("Procession_Text", new() {("PowerStat", PowerStat)});

public static string Procession_Log (string This) => Translator.inst.Translate("Procession_Log", new() {("This", This)});

public static string Drown_Text (string Num) => Translator.inst.Translate("Drown_Text", new() {("Num", Num)});

public static string Drown_Log (string This,string Target) => Translator.inst.Translate("Drown_Log", new() {("This", This),("Target", Target)});

public static string Shipwreck_Text (string DefenseStat,string MiscStat) => Translator.inst.Translate("Shipwreck_Text", new() {("DefenseStat", DefenseStat),("MiscStat", MiscStat)});

public static string Shipwreck_Log (string This,string Target) => Translator.inst.Translate("Shipwreck_Log", new() {("This", This),("Target", Target)});

public static string Manipulate_Text (string MiscStat) => Translator.inst.Translate("Manipulate_Text", new() {("MiscStat", MiscStat)});

public static string Manipulate_Log (string This,string Target) => Translator.inst.Translate("Manipulate_Log", new() {("This", This),("Target", Target)});

public static string Sing_Text (string MiscStat) => Translator.inst.Translate("Sing_Text", new() {("MiscStat", MiscStat)});

public static string Sing_Log (string This) => Translator.inst.Translate("Sing_Log", new() {("This", This)});

public static string Stab_Prey_Text (string Num) => Translator.inst.Translate("Stab_Prey_Text", new() {("Num", Num)});

public static string Stab_Prey_Log (string This,string Target) => Translator.inst.Translate("Stab_Prey_Log", new() {("This", This),("Target", Target)});

public static string Trapped_Text (string MiscStat) => Translator.inst.Translate("Trapped_Text", new() {("MiscStat", MiscStat)});

public static string Trapped_Log (string This,string Target) => Translator.inst.Translate("Trapped_Log", new() {("This", This),("Target", Target)});

public static string Silk_Wrap_Text (string PowerStat) => Translator.inst.Translate("Silk_Wrap_Text", new() {("PowerStat", PowerStat)});

public static string Silk_Wrap_Log (string This,string Target) => Translator.inst.Translate("Silk_Wrap_Log", new() {("This", This),("Target", Target)});

public static string Spin_Web_Text (string DefenseStat) => Translator.inst.Translate("Spin_Web_Text", new() {("DefenseStat", DefenseStat)});

public static string Spin_Web_Log (string This,string Target) => Translator.inst.Translate("Spin_Web_Log", new() {("This", This),("Target", Target)});

public static string Punishment_Text (string Num) => Translator.inst.Translate("Punishment_Text", new() {("Num", Num)});

public static string Punishment_Log (string This,string Target) => Translator.inst.Translate("Punishment_Log", new() {("This", This),("Target", Target)});

public static string Demand_Happiness_Log (string This) => Translator.inst.Translate("Demand_Happiness_Log", new() {("This", This)});

public static string Demand_Anger_Log (string This) => Translator.inst.Translate("Demand_Anger_Log", new() {("This", This)});

public static string Demand_Sadness_Log (string This) => Translator.inst.Translate("Demand_Sadness_Log", new() {("This", This)});

public static string Mischief_Text (string Num) => Translator.inst.Translate("Mischief_Text", new() {("Num", Num)});

public static string Mischief_Log (string This,string Target) => Translator.inst.Translate("Mischief_Log", new() {("This", This),("Target", Target)});

public static string Delude_Text (string MiscStat) => Translator.inst.Translate("Delude_Text", new() {("MiscStat", MiscStat)});

public static string Delude_Log (string This,string Target) => Translator.inst.Translate("Delude_Log", new() {("This", This),("Target", Target)});

public static string Delay_Text (string MiscStat) => Translator.inst.Translate("Delay_Text", new() {("MiscStat", MiscStat)});

public static string Delay_Log (string This,string Target) => Translator.inst.Translate("Delay_Log", new() {("This", This),("Target", Target)});

public static string Distract_Text (string MiscStat) => Translator.inst.Translate("Distract_Text", new() {("MiscStat", MiscStat)});

public static string Distract_Log (string This,string Target) => Translator.inst.Translate("Distract_Log", new() {("This", This),("Target", Target)});

public static string Toughen_Text (string Num,string DefenseStat) => Translator.inst.Translate("Toughen_Text", new() {("Num", Num),("DefenseStat", DefenseStat)});

public static string Toughen_Log (string This) => Translator.inst.Translate("Toughen_Log", new() {("This", This)});

public static string In_the_Way_Text (string MiscStat) => Translator.inst.Translate("In_the_Way_Text", new() {("MiscStat", MiscStat)});

public static string In_the_Way_Log (string This) => Translator.inst.Translate("In_the_Way_Log", new() {("This", This)});

public static string Slash_Text (string Num) => Translator.inst.Translate("Slash_Text", new() {("Num", Num)});

public static string Slash_Log (string This,string Target) => Translator.inst.Translate("Slash_Log", new() {("This", This),("Target", Target)});

public static string Howl_Log (string This) => Translator.inst.Translate("Howl_Log", new() {("This", This)});

public static string Nurture_Text (string MiscStat,string PowerStat,string DefenseStat) => Translator.inst.Translate("Nurture_Text", new() {("MiscStat", MiscStat),("PowerStat", PowerStat),("DefenseStat", DefenseStat)});

public static string Nurture_Log (string This) => Translator.inst.Translate("Nurture_Log", new() {("This", This)});

public static string Guard_Text (string MiscStat) => Translator.inst.Translate("Guard_Text", new() {("MiscStat", MiscStat)});

public static string Guard_Log (string This,string Target) => Translator.inst.Translate("Guard_Log", new() {("This", This),("Target", Target)});

public static string Counting_Down (string Num) => Translator.inst.Translate("Counting_Down", new() {("Num", Num)});

public static string Title() => Translator.inst.Translate("Title");
public static string Author_Credit() => Translator.inst.Translate("Author_Credit");
public static string Last_Update() => Translator.inst.Translate("Last_Update");
public static string Language() => Translator.inst.Translate("Language");
public static string Translator_Credit() => Translator.inst.Translate("Translator_Credit");
public static string Loading() => Translator.inst.Translate("Loading");
public static string Play_Game() => Translator.inst.Translate("Play_Game");
public static string Update_History() => Translator.inst.Translate("Update_History");
public static string Story() => Translator.inst.Translate("Story");
public static string Story_Text() => Translator.inst.Translate("Story_Text");
public static string Daily_Challenge() => Translator.inst.Translate("Daily_Challenge");
public static string Month_1() => Translator.inst.Translate("Month_1");
public static string Month_2() => Translator.inst.Translate("Month_2");
public static string Month_3() => Translator.inst.Translate("Month_3");
public static string Month_4() => Translator.inst.Translate("Month_4");
public static string Month_5() => Translator.inst.Translate("Month_5");
public static string Month_6() => Translator.inst.Translate("Month_6");
public static string Month_7() => Translator.inst.Translate("Month_7");
public static string Month_8() => Translator.inst.Translate("Month_8");
public static string Month_9() => Translator.inst.Translate("Month_9");
public static string Month_10() => Translator.inst.Translate("Month_10");
public static string Month_11() => Translator.inst.Translate("Month_11");
public static string Month_12() => Translator.inst.Translate("Month_12");
public static string Cheats_and_Challenges() => Translator.inst.Translate("Cheats_and_Challenges");
public static string Cheat() => Translator.inst.Translate("Cheat");
public static string New_Abilities() => Translator.inst.Translate("New_Abilities");
public static string New_Abilities_Text() => Translator.inst.Translate("New_Abilities_Text");
public static string Knight_Reach() => Translator.inst.Translate("Knight_Reach");
public static string Knight_Reach_Text() => Translator.inst.Translate("Knight_Reach_Text");
public static string Weaker_Enemies() => Translator.inst.Translate("Weaker_Enemies");
public static string Weaker_Enemies_Text() => Translator.inst.Translate("Weaker_Enemies_Text");
public static string Slower_Enemy_Cooldowns() => Translator.inst.Translate("Slower_Enemy_Cooldowns");
public static string Slower_Enemy_Cooldowns_Text() => Translator.inst.Translate("Slower_Enemy_Cooldowns_Text");
public static string Number_Cap() => Translator.inst.Translate("Number_Cap");
public static string Number_Cap_Text() => Translator.inst.Translate("Number_Cap_Text");
public static string Challenge() => Translator.inst.Translate("Challenge");
public static string No_Revives() => Translator.inst.Translate("No_Revives");
public static string No_Revives_Text() => Translator.inst.Translate("No_Revives_Text");
public static string Ineffectives_Fail() => Translator.inst.Translate("Ineffectives_Fail");
public static string Ineffectives_Fail_Text() => Translator.inst.Translate("Ineffectives_Fail_Text");
public static string Player_Timer() => Translator.inst.Translate("Player_Timer");
public static string Player_Timer_Text() => Translator.inst.Translate("Player_Timer_Text");
public static string Extra_Enemy_Turns() => Translator.inst.Translate("Extra_Enemy_Turns");
public static string Extra_Enemy_Turns_Text() => Translator.inst.Translate("Extra_Enemy_Turns_Text");
public static string More_Enemies() => Translator.inst.Translate("More_Enemies");
public static string More_Enemies_Text() => Translator.inst.Translate("More_Enemies_Text");
public static string Clear_All() => Translator.inst.Translate("Clear_All");
public static string Apply_Number_Cap() => Translator.inst.Translate("Apply_Number_Cap");
public static string Apply_Minimum() => Translator.inst.Translate("Apply_Minimum");
public static string Failed() => Translator.inst.Translate("Failed");
public static string Timer() => Translator.inst.Translate("Timer");
public static string Gain_New_Abilities() => Translator.inst.Translate("Gain_New_Abilities");
public static string Special_Thanks() => Translator.inst.Translate("Special_Thanks");
public static string Seth_Royston_Thanks() => Translator.inst.Translate("Seth_Royston_Thanks");
public static string Liam_Cribbs_Thanks() => Translator.inst.Translate("Liam_Cribbs_Thanks");
public static string Flan_Falacci_Thanks() => Translator.inst.Translate("Flan_Falacci_Thanks");
public static string Inspiration_Thanks() => Translator.inst.Translate("Inspiration_Thanks");
public static string Playtest_Thursday_Thanks() => Translator.inst.Translate("Playtest_Thursday_Thanks");
public static string Settings() => Translator.inst.Translate("Settings");
public static string Emotions_Guide() => Translator.inst.Translate("Emotions_Guide");
public static string Close_Settings() => Translator.inst.Translate("Close_Settings");
public static string Close() => Translator.inst.Translate("Close");
public static string Random() => Translator.inst.Translate("Random");
public static string Animation_Time() => Translator.inst.Translate("Animation_Time");
public static string Confirm_Decisions() => Translator.inst.Translate("Confirm_Decisions");
public static string Confirm() => Translator.inst.Translate("Confirm");
public static string Rechoose() => Translator.inst.Translate("Rechoose");
public static string Display_Tooltip() => Translator.inst.Translate("Display_Tooltip");
public static string Cooldown() => Translator.inst.Translate("Cooldown");
public static string Cooldown_Text() => Translator.inst.Translate("Cooldown_Text");
public static string Dead() => Translator.inst.Translate("Dead");
public static string Neutral() => Translator.inst.Translate("Neutral");
public static string Neutral_Text() => Translator.inst.Translate("Neutral_Text");
public static string Sad() => Translator.inst.Translate("Sad");
public static string Sad_Text() => Translator.inst.Translate("Sad_Text");
public static string Happy() => Translator.inst.Translate("Happy");
public static string Happy_Text() => Translator.inst.Translate("Happy_Text");
public static string Angry() => Translator.inst.Translate("Angry");
public static string Angry_Text() => Translator.inst.Translate("Angry_Text");
public static string Grounded() => Translator.inst.Translate("Grounded");
public static string Grounded_Text() => Translator.inst.Translate("Grounded_Text");
public static string Elevated() => Translator.inst.Translate("Elevated");
public static string Elevated_Text() => Translator.inst.Translate("Elevated_Text");
public static string Health() => Translator.inst.Translate("Health");
public static string Health_Text() => Translator.inst.Translate("Health_Text");
public static string Max_Health() => Translator.inst.Translate("Max_Health");
public static string Power() => Translator.inst.Translate("Power");
public static string Power_Text() => Translator.inst.Translate("Power_Text");
public static string Defense() => Translator.inst.Translate("Defense");
public static string Defense_Text() => Translator.inst.Translate("Defense_Text");
public static string Protected() => Translator.inst.Translate("Protected");
public static string Protected_Text() => Translator.inst.Translate("Protected_Text");
public static string Blocked() => Translator.inst.Translate("Blocked");
public static string Locked() => Translator.inst.Translate("Locked");
public static string Locked_Text() => Translator.inst.Translate("Locked_Text");
public static string Stunned() => Translator.inst.Translate("Stunned");
public static string Stunned_Text() => Translator.inst.Translate("Stunned_Text");
public static string Targeted() => Translator.inst.Translate("Targeted");
public static string Targeted_Text() => Translator.inst.Translate("Targeted_Text");
public static string Extra() => Translator.inst.Translate("Extra");
public static string Extra_Text() => Translator.inst.Translate("Extra_Text");
public static string Star() => Translator.inst.Translate("Star");
public static string Star_Text() => Translator.inst.Translate("Star_Text");
public static string Customize_Players() => Translator.inst.Translate("Customize_Players");
public static string Customize_Description() => Translator.inst.Translate("Customize_Description");
public static string Choose_Emotion() => Translator.inst.Translate("Choose_Emotion");
public static string Next_in_Line() => Translator.inst.Translate("Next_in_Line");
public static string Title_Screen() => Translator.inst.Translate("Title_Screen");
public static string Quit_Game() => Translator.inst.Translate("Quit_Game");
public static string Quit_Fight() => Translator.inst.Translate("Quit_Fight");
public static string Game_Won() => Translator.inst.Translate("Game_Won");
public static string Game_Lost() => Translator.inst.Translate("Game_Lost");
public static string Tutorial() => Translator.inst.Translate("Tutorial");
public static string Tutorial_10() => Translator.inst.Translate("Tutorial_10");
public static string Tutorial_20() => Translator.inst.Translate("Tutorial_20");
public static string Tutorial_30() => Translator.inst.Translate("Tutorial_30");
public static string Tutorial_40() => Translator.inst.Translate("Tutorial_40");
public static string Tutorial_50() => Translator.inst.Translate("Tutorial_50");
public static string Tutorial_51() => Translator.inst.Translate("Tutorial_51");
public static string Tutorial_52() => Translator.inst.Translate("Tutorial_52");
public static string Tutorial_53() => Translator.inst.Translate("Tutorial_53");
public static string Tutorial_54() => Translator.inst.Translate("Tutorial_54");
public static string Tutorial_60() => Translator.inst.Translate("Tutorial_60");
public static string Tutorial_70() => Translator.inst.Translate("Tutorial_70");
public static string Tutorial_71() => Translator.inst.Translate("Tutorial_71");
public static string Tutorial_72() => Translator.inst.Translate("Tutorial_72");
public static string Tutorial_80() => Translator.inst.Translate("Tutorial_80");
public static string Tutorial_81() => Translator.inst.Translate("Tutorial_81");
public static string Tutorial_82() => Translator.inst.Translate("Tutorial_82");
public static string Tutorial_83() => Translator.inst.Translate("Tutorial_83");
public static string Tutorial_90() => Translator.inst.Translate("Tutorial_90");
public static string Tutorial_91() => Translator.inst.Translate("Tutorial_91");
public static string Tutorial_100() => Translator.inst.Translate("Tutorial_100");
public static string Tutorial_110() => Translator.inst.Translate("Tutorial_110");
public static string Tutorial_120() => Translator.inst.Translate("Tutorial_120");
public static string Tutorial_121() => Translator.inst.Translate("Tutorial_121");
public static string Tutorial_130() => Translator.inst.Translate("Tutorial_130");
public static string Tutorial_140() => Translator.inst.Translate("Tutorial_140");
public static string Tutorial_150() => Translator.inst.Translate("Tutorial_150");
public static string Tutorial_151() => Translator.inst.Translate("Tutorial_151");
public static string Tutorial_Finished() => Translator.inst.Translate("Tutorial_Finished");
public static string Next() => Translator.inst.Translate("Next");
public static string Encyclopedia() => Translator.inst.Translate("Encyclopedia");
public static string Exit() => Translator.inst.Translate("Exit");
public static string Abilities() => Translator.inst.Translate("Abilities");
public static string Search_Abilities() => Translator.inst.Translate("Search_Abilities");
public static string Type_1() => Translator.inst.Translate("Type_1");
public static string Type_2() => Translator.inst.Translate("Type_2");
public static string Any() => Translator.inst.Translate("Any");
public static string Attack() => Translator.inst.Translate("Attack");
public static string Healing() => Translator.inst.Translate("Healing");
public static string EmotionPlayer() => Translator.inst.Translate("EmotionPlayer");
public static string EmotionEnemy() => Translator.inst.Translate("EmotionEnemy");
public static string PositionPlayer() => Translator.inst.Translate("PositionPlayer");
public static string PositionEnemy() => Translator.inst.Translate("PositionEnemy");
public static string StatPlayer() => Translator.inst.Translate("StatPlayer");
public static string StatEnemy() => Translator.inst.Translate("StatEnemy");
public static string Player() => Translator.inst.Translate("Player");
public static string Enemies() => Translator.inst.Translate("Enemies");
public static string Search_Enemies() => Translator.inst.Translate("Search_Enemies");
public static string Right_Click_Reminder() => Translator.inst.Translate("Right_Click_Reminder");
public static string Default_Position() => Translator.inst.Translate("Default_Position");
public static string Knight() => Translator.inst.Translate("Knight");
public static string Knight_Text() => Translator.inst.Translate("Knight_Text");
public static string Battle_Cry() => Translator.inst.Translate("Battle_Cry");
public static string Cheer() => Translator.inst.Translate("Cheer");
public static string Daze() => Translator.inst.Translate("Daze");
public static string Desperation() => Translator.inst.Translate("Desperation");
public static string Embarass() => Translator.inst.Translate("Embarass");
public static string Force() => Translator.inst.Translate("Force");
public static string Impale() => Translator.inst.Translate("Impale");
public static string Intimidate() => Translator.inst.Translate("Intimidate");
public static string Intimidate_Text() => Translator.inst.Translate("Intimidate_Text");
public static string Joust() => Translator.inst.Translate("Joust");
public static string Knock_Down() => Translator.inst.Translate("Knock_Down");
public static string Meditate() => Translator.inst.Translate("Meditate");
public static string Mirror() => Translator.inst.Translate("Mirror");
public static string Mock() => Translator.inst.Translate("Mock");
public static string Neutralize() => Translator.inst.Translate("Neutralize");
public static string Redirect() => Translator.inst.Translate("Redirect");
public static string Strike() => Translator.inst.Translate("Strike");
public static string Steal() => Translator.inst.Translate("Steal");
public static string Surprise() => Translator.inst.Translate("Surprise");
public static string Twirl() => Translator.inst.Translate("Twirl");
public static string Upset() => Translator.inst.Translate("Upset");
public static string Immobilize() => Translator.inst.Translate("Immobilize");
public static string Angel() => Translator.inst.Translate("Angel");
public static string Angel_Text() => Translator.inst.Translate("Angel_Text");
public static string Air_Combat() => Translator.inst.Translate("Air_Combat");
public static string Assist() => Translator.inst.Translate("Assist");
public static string Calm_Down() => Translator.inst.Translate("Calm_Down");
public static string Crash_Land() => Translator.inst.Translate("Crash_Land");
public static string Enrage() => Translator.inst.Translate("Enrage");
public static string Exhaust() => Translator.inst.Translate("Exhaust");
public static string Exorcism() => Translator.inst.Translate("Exorcism");
public static string Gift_of_Flight() => Translator.inst.Translate("Gift_of_Flight");
public static string Gift_of_Flight_Text() => Translator.inst.Translate("Gift_of_Flight_Text");
public static string Hide() => Translator.inst.Translate("Hide");
public static string Immunity() => Translator.inst.Translate("Immunity");
public static string Joy() => Translator.inst.Translate("Joy");
public static string Lift_Up() => Translator.inst.Translate("Lift_Up");
public static string Motivate() => Translator.inst.Translate("Motivate");
public static string Overheal() => Translator.inst.Translate("Overheal");
public static string Petrify() => Translator.inst.Translate("Petrify");
public static string Plummet() => Translator.inst.Translate("Plummet");
public static string Plummet_Text() => Translator.inst.Translate("Plummet_Text");
public static string Retreat() => Translator.inst.Translate("Retreat");
public static string Retreat_Text() => Translator.inst.Translate("Retreat_Text");
public static string Security() => Translator.inst.Translate("Security");
public static string Soft_Landing() => Translator.inst.Translate("Soft_Landing");
public static string Tailwinds() => Translator.inst.Translate("Tailwinds");
public static string Team_Up() => Translator.inst.Translate("Team_Up");
public static string Wizard() => Translator.inst.Translate("Wizard");
public static string Wizard_Text() => Translator.inst.Translate("Wizard_Text");
public static string Above_Danger() => Translator.inst.Translate("Above_Danger");
public static string Bad_Omens() => Translator.inst.Translate("Bad_Omens");
public static string Bounce() => Translator.inst.Translate("Bounce");
public static string Chill() => Translator.inst.Translate("Chill");
public static string Accelerate() => Translator.inst.Translate("Accelerate");
public static string Flood() => Translator.inst.Translate("Flood");
public static string Gust() => Translator.inst.Translate("Gust");
public static string Headwinds() => Translator.inst.Translate("Headwinds");
public static string Crown() => Translator.inst.Translate("Crown");
public static string Storm() => Translator.inst.Translate("Storm");
public static string Manipulate_Time() => Translator.inst.Translate("Manipulate_Time");
public static string Bind() => Translator.inst.Translate("Bind");
public static string Punch_Up() => Translator.inst.Translate("Punch_Up");
public static string Quick_Attack() => Translator.inst.Translate("Quick_Attack");
public static string Readjust() => Translator.inst.Translate("Readjust");
public static string Readjust_Text() => Translator.inst.Translate("Readjust_Text");
public static string Restrain() => Translator.inst.Translate("Restrain");
public static string Shockwave() => Translator.inst.Translate("Shockwave");
public static string Stalactites() => Translator.inst.Translate("Stalactites");
public static string Team_Attack() => Translator.inst.Translate("Team_Attack");
public static string Touchdown() => Translator.inst.Translate("Touchdown");
public static string Warp() => Translator.inst.Translate("Warp");
public static string Revive() => Translator.inst.Translate("Revive");
public static string Skip_Turn() => Translator.inst.Translate("Skip_Turn");
public static string Skip_Turn_Text() => Translator.inst.Translate("Skip_Turn_Text");
public static string Archer() => Translator.inst.Translate("Archer");
public static string Archer_Text() => Translator.inst.Translate("Archer_Text");
public static string Quick_Shot() => Translator.inst.Translate("Quick_Shot");
public static string Bullseye() => Translator.inst.Translate("Bullseye");
public static string Volley() => Translator.inst.Translate("Volley");
public static string Focus() => Translator.inst.Translate("Focus");
public static string Barbarian() => Translator.inst.Translate("Barbarian");
public static string Barbarian_Text() => Translator.inst.Translate("Barbarian_Text");
public static string Fury() => Translator.inst.Translate("Fury");
public static string Savagery() => Translator.inst.Translate("Savagery");
public static string Brawl() => Translator.inst.Translate("Brawl");
public static string Camp() => Translator.inst.Translate("Camp");
public static string Bat() => Translator.inst.Translate("Bat");
public static string Bat_Text() => Translator.inst.Translate("Bat_Text");
public static string Drain() => Translator.inst.Translate("Drain");
public static string Screech() => Translator.inst.Translate("Screech");
public static string Hunting_Season() => Translator.inst.Translate("Hunting_Season");
public static string Bloodthirst() => Translator.inst.Translate("Bloodthirst");
public static string Bees() => Translator.inst.Translate("Bees");
public static string Bees_Text() => Translator.inst.Translate("Bees_Text");
public static string Sting() => Translator.inst.Translate("Sting");
public static string Honey() => Translator.inst.Translate("Honey");
public static string Protect_the_Queen() => Translator.inst.Translate("Protect_the_Queen");
public static string Support() => Translator.inst.Translate("Support");
public static string Cow() => Translator.inst.Translate("Cow");
public static string Cow_Text() => Translator.inst.Translate("Cow_Text");
public static string Charge() => Translator.inst.Translate("Charge");
public static string Milk() => Translator.inst.Translate("Milk");
public static string Graze() => Translator.inst.Translate("Graze");
public static string Bellow() => Translator.inst.Translate("Bellow");
public static string Crow() => Translator.inst.Translate("Crow");
public static string Crow_Text() => Translator.inst.Translate("Crow_Text");
public static string Peck() => Translator.inst.Translate("Peck");
public static string Demon() => Translator.inst.Translate("Demon");
public static string Demon_Text() => Translator.inst.Translate("Demon_Text");
public static string Dark_Energy() => Translator.inst.Translate("Dark_Energy");
public static string Betrayal() => Translator.inst.Translate("Betrayal");
public static string Ritual() => Translator.inst.Translate("Ritual");
public static string Demand_Energy() => Translator.inst.Translate("Demand_Energy");
public static string Dragon() => Translator.inst.Translate("Dragon");
public static string Dragon_Text() => Translator.inst.Translate("Dragon_Text");
public static string Firebreath() => Translator.inst.Translate("Firebreath");
public static string Roost() => Translator.inst.Translate("Roost");
public static string Take_Flight() => Translator.inst.Translate("Take_Flight");
public static string Take_Flight_Text() => Translator.inst.Translate("Take_Flight_Text");
public static string Roar() => Translator.inst.Translate("Roar");
public static string Drone() => Translator.inst.Translate("Drone");
public static string Drone_Text() => Translator.inst.Translate("Drone_Text");
public static string Sound_Waves() => Translator.inst.Translate("Sound_Waves");
public static string Alarm() => Translator.inst.Translate("Alarm");
public static string Ghost() => Translator.inst.Translate("Ghost");
public static string Ghost_Text() => Translator.inst.Translate("Ghost_Text");
public static string Jumpscare() => Translator.inst.Translate("Jumpscare");
public static string Lament() => Translator.inst.Translate("Lament");
public static string Lament_Text() => Translator.inst.Translate("Lament_Text");
public static string Possession() => Translator.inst.Translate("Possession");
public static string Release() => Translator.inst.Translate("Release");
public static string Herald() => Translator.inst.Translate("Herald");
public static string Herald_Text() => Translator.inst.Translate("Herald_Text");
public static string Flag_Whack() => Translator.inst.Translate("Flag_Whack");
public static string Announce() => Translator.inst.Translate("Announce");
public static string Kitty() => Translator.inst.Translate("Kitty");
public static string Kitty_Text() => Translator.inst.Translate("Kitty_Text");
public static string Scratch() => Translator.inst.Translate("Scratch");
public static string Stuck_in_a_Tree() => Translator.inst.Translate("Stuck_in_a_Tree");
public static string Stuck_in_a_Tree_Text() => Translator.inst.Translate("Stuck_in_a_Tree_Text");
public static string Land_Feet_First() => Translator.inst.Translate("Land_Feet_First");
public static string Lure() => Translator.inst.Translate("Lure");
public static string Leprechaun() => Translator.inst.Translate("Leprechaun");
public static string Leprechaun_Text() => Translator.inst.Translate("Leprechaun_Text");
public static string Prank() => Translator.inst.Translate("Prank");
public static string Caught_at_a_Good_Time() => Translator.inst.Translate("Caught_at_a_Good_Time");
public static string Ninja() => Translator.inst.Translate("Ninja");
public static string Ninja_Text() => Translator.inst.Translate("Ninja_Text");
public static string Assassinate() => Translator.inst.Translate("Assassinate");
public static string Shuriken() => Translator.inst.Translate("Shuriken");
public static string Into_Shadows() => Translator.inst.Translate("Into_Shadows");
public static string Wall_Climb() => Translator.inst.Translate("Wall_Climb");
public static string Page() => Translator.inst.Translate("Page");
public static string Page_Text() => Translator.inst.Translate("Page_Text");
public static string Swing() => Translator.inst.Translate("Swing");
public static string Partier() => Translator.inst.Translate("Partier");
public static string Partier_Text() => Translator.inst.Translate("Partier_Text");
public static string Fight() => Translator.inst.Translate("Fight");
public static string Share_Drinks() => Translator.inst.Translate("Share_Drinks");
public static string Share_Snacks() => Translator.inst.Translate("Share_Snacks");
public static string Drinking_Game() => Translator.inst.Translate("Drinking_Game");
public static string Drinking_Game_Text() => Translator.inst.Translate("Drinking_Game_Text");
public static string Pixie() => Translator.inst.Translate("Pixie");
public static string Pixie_Text() => Translator.inst.Translate("Pixie_Text");
public static string Will_o_Wisp() => Translator.inst.Translate("Will_o_Wisp");
public static string Cast_Happiness() => Translator.inst.Translate("Cast_Happiness");
public static string Cast_Happiness_Text() => Translator.inst.Translate("Cast_Happiness_Text");
public static string Cast_Sadness() => Translator.inst.Translate("Cast_Sadness");
public static string Cast_Sadness_Text() => Translator.inst.Translate("Cast_Sadness_Text");
public static string Cast_Anger() => Translator.inst.Translate("Cast_Anger");
public static string Cast_Anger_Text() => Translator.inst.Translate("Cast_Anger_Text");
public static string Puppeteer() => Translator.inst.Translate("Puppeteer");
public static string Puppeteer_Text() => Translator.inst.Translate("Puppeteer_Text");
public static string Collision() => Translator.inst.Translate("Collision");
public static string Drop() => Translator.inst.Translate("Drop");
public static string Dangle() => Translator.inst.Translate("Dangle");
public static string Control() => Translator.inst.Translate("Control");
public static string Puppy() => Translator.inst.Translate("Puppy");
public static string Puppy_Text() => Translator.inst.Translate("Puppy_Text");
public static string Bite() => Translator.inst.Translate("Bite");
public static string Leap() => Translator.inst.Translate("Leap");
public static string Bark() => Translator.inst.Translate("Bark");
public static string Cuddle() => Translator.inst.Translate("Cuddle");
public static string Rats() => Translator.inst.Translate("Rats");
public static string Rats_Text() => Translator.inst.Translate("Rats_Text");
public static string Swarm() => Translator.inst.Translate("Swarm");
public static string Strength_in_Numbers() => Translator.inst.Translate("Strength_in_Numbers");
public static string Strength_in_Numbers_Text() => Translator.inst.Translate("Strength_in_Numbers_Text");
public static string Royalty() => Translator.inst.Translate("Royalty");
public static string Royalty_Text() => Translator.inst.Translate("Royalty_Text");
public static string Order() => Translator.inst.Translate("Order");
public static string Promote() => Translator.inst.Translate("Promote");
public static string Call() => Translator.inst.Translate("Call");
public static string Procession() => Translator.inst.Translate("Procession");
public static string Siren() => Translator.inst.Translate("Siren");
public static string Siren_Text() => Translator.inst.Translate("Siren_Text");
public static string Drown() => Translator.inst.Translate("Drown");
public static string Shipwreck() => Translator.inst.Translate("Shipwreck");
public static string Manipulate() => Translator.inst.Translate("Manipulate");
public static string Sing() => Translator.inst.Translate("Sing");
public static string Spider() => Translator.inst.Translate("Spider");
public static string Spider_Text() => Translator.inst.Translate("Spider_Text");
public static string Stab_Prey() => Translator.inst.Translate("Stab_Prey");
public static string Trapped() => Translator.inst.Translate("Trapped");
public static string Silk_Wrap() => Translator.inst.Translate("Silk_Wrap");
public static string Spin_Web() => Translator.inst.Translate("Spin_Web");
public static string Taskmaster() => Translator.inst.Translate("Taskmaster");
public static string Taskmaster_Text() => Translator.inst.Translate("Taskmaster_Text");
public static string Punishment() => Translator.inst.Translate("Punishment");
public static string Demand_Happiness() => Translator.inst.Translate("Demand_Happiness");
public static string Demand_Happiness_Text() => Translator.inst.Translate("Demand_Happiness_Text");
public static string Demand_Anger() => Translator.inst.Translate("Demand_Anger");
public static string Demand_Anger_Text() => Translator.inst.Translate("Demand_Anger_Text");
public static string Demand_Sadness() => Translator.inst.Translate("Demand_Sadness");
public static string Demand_Sadness_Text() => Translator.inst.Translate("Demand_Sadness_Text");
public static string Trickster() => Translator.inst.Translate("Trickster");
public static string Trickster_Text() => Translator.inst.Translate("Trickster_Text");
public static string Mischief() => Translator.inst.Translate("Mischief");
public static string Delude() => Translator.inst.Translate("Delude");
public static string Delay() => Translator.inst.Translate("Delay");
public static string Distract() => Translator.inst.Translate("Distract");
public static string Wall() => Translator.inst.Translate("Wall");
public static string Wall_Text() => Translator.inst.Translate("Wall_Text");
public static string Toughen() => Translator.inst.Translate("Toughen");
public static string In_the_Way() => Translator.inst.Translate("In_the_Way");
public static string Wolf() => Translator.inst.Translate("Wolf");
public static string Wolf_Text() => Translator.inst.Translate("Wolf_Text");
public static string Slash() => Translator.inst.Translate("Slash");
public static string Howl() => Translator.inst.Translate("Howl");
public static string Howl_Text() => Translator.inst.Translate("Howl_Text");
public static string Nurture() => Translator.inst.Translate("Nurture");
public static string Guard() => Translator.inst.Translate("Guard");
public static string Update_0() => Translator.inst.Translate("Update_0");
public static string Update_0_Text() => Translator.inst.Translate("Update_0_Text");
public static string Update_1() => Translator.inst.Translate("Update_1");
public static string Update_1_Text() => Translator.inst.Translate("Update_1_Text");
public static string Update_2() => Translator.inst.Translate("Update_2");
public static string Update_2_Text() => Translator.inst.Translate("Update_2_Text");
public static string Upload_Translation() => Translator.inst.Translate("Upload_Translation");
public static string Download_English() => Translator.inst.Translate("Download_English");
public static string Blank() => Translator.inst.Translate("Blank");
public static string Update_3() => Translator.inst.Translate("Update_3");
public static string Update_3_Text() => Translator.inst.Translate("Update_3_Text");
public static string Update_4() => Translator.inst.Translate("Update_4");
public static string Update_4_Text() => Translator.inst.Translate("Update_4_Text");
public static string Update_5() => Translator.inst.Translate("Update_5");
public static string Update_5_Text() => Translator.inst.Translate("Update_5_Text");
}
public enum ToTranslate {
Title,Author_Credit,Last_Update,Language,Translator_Credit,Loading,Play_Game,Update_History,Story,Story_Text,Daily_Challenge,Month_1,Month_2,Month_3,Month_4,Month_5,Month_6,Month_7,Month_8,Month_9,Month_10,Month_11,Month_12,Cheats_and_Challenges,Cheat,New_Abilities,New_Abilities_Text,Knight_Reach,Knight_Reach_Text,Weaker_Enemies,Weaker_Enemies_Text,Slower_Enemy_Cooldowns,Slower_Enemy_Cooldowns_Text,Number_Cap,Number_Cap_Text,Challenge,No_Revives,No_Revives_Text,Ineffectives_Fail,Ineffectives_Fail_Text,Player_Timer,Player_Timer_Text,Extra_Enemy_Turns,Extra_Enemy_Turns_Text,More_Enemies,More_Enemies_Text,Clear_All,Apply_Number_Cap,Apply_Minimum,Failed,Timer,Gain_New_Abilities,Special_Thanks,Seth_Royston_Thanks,Liam_Cribbs_Thanks,Flan_Falacci_Thanks,Inspiration_Thanks,Playtest_Thursday_Thanks,Settings,Emotions_Guide,Close_Settings,Close,Random,Animation_Time,Confirm_Decisions,Confirm,Rechoose,Display_Tooltip,Cooldown,Cooldown_Text,Dead,Neutral,Neutral_Text,Sad,Sad_Text,Happy,Happy_Text,Angry,Angry_Text,Grounded,Grounded_Text,Elevated,Elevated_Text,Health,Health_Text,Max_Health,Power,Power_Text,Defense,Defense_Text,Protected,Protected_Text,Blocked,Locked,Locked_Text,Stunned,Stunned_Text,Targeted,Targeted_Text,Extra,Extra_Text,Star,Star_Text,Customize_Players,Customize_Description,Choose_Emotion,Next_in_Line,Title_Screen,Quit_Game,Quit_Fight,Game_Won,Game_Lost,Tutorial,Tutorial_10,Tutorial_20,Tutorial_30,Tutorial_40,Tutorial_50,Tutorial_51,Tutorial_52,Tutorial_53,Tutorial_54,Tutorial_60,Tutorial_70,Tutorial_71,Tutorial_72,Tutorial_80,Tutorial_81,Tutorial_82,Tutorial_83,Tutorial_90,Tutorial_91,Tutorial_100,Tutorial_110,Tutorial_120,Tutorial_121,Tutorial_130,Tutorial_140,Tutorial_150,Tutorial_151,Tutorial_Finished,Next,Encyclopedia,Exit,Abilities,Search_Abilities,Type_1,Type_2,Any,Attack,Healing,EmotionPlayer,EmotionEnemy,PositionPlayer,PositionEnemy,StatPlayer,StatEnemy,Player,Enemies,Search_Enemies,Right_Click_Reminder,Default_Position,Knight,Knight_Text,Battle_Cry,Cheer,Daze,Desperation,Embarass,Force,Impale,Intimidate,Intimidate_Text,Joust,Knock_Down,Meditate,Mirror,Mock,Neutralize,Redirect,Strike,Steal,Surprise,Twirl,Upset,Immobilize,Angel,Angel_Text,Air_Combat,Assist,Calm_Down,Crash_Land,Enrage,Exhaust,Exorcism,Gift_of_Flight,Gift_of_Flight_Text,Hide,Immunity,Joy,Lift_Up,Motivate,Overheal,Petrify,Plummet,Plummet_Text,Retreat,Retreat_Text,Security,Soft_Landing,Tailwinds,Team_Up,Wizard,Wizard_Text,Above_Danger,Bad_Omens,Bounce,Chill,Accelerate,Flood,Gust,Headwinds,Crown,Storm,Manipulate_Time,Bind,Punch_Up,Quick_Attack,Readjust,Readjust_Text,Restrain,Shockwave,Stalactites,Team_Attack,Touchdown,Warp,Revive,Skip_Turn,Skip_Turn_Text,Archer,Archer_Text,Quick_Shot,Bullseye,Volley,Focus,Barbarian,Barbarian_Text,Fury,Savagery,Brawl,Camp,Bat,Bat_Text,Drain,Screech,Hunting_Season,Bloodthirst,Bees,Bees_Text,Sting,Honey,Protect_the_Queen,Support,Cow,Cow_Text,Charge,Milk,Graze,Bellow,Crow,Crow_Text,Peck,Demon,Demon_Text,Dark_Energy,Betrayal,Ritual,Demand_Energy,Dragon,Dragon_Text,Firebreath,Roost,Take_Flight,Take_Flight_Text,Roar,Drone,Drone_Text,Sound_Waves,Alarm,Ghost,Ghost_Text,Jumpscare,Lament,Lament_Text,Possession,Release,Herald,Herald_Text,Flag_Whack,Announce,Kitty,Kitty_Text,Scratch,Stuck_in_a_Tree,Stuck_in_a_Tree_Text,Land_Feet_First,Lure,Leprechaun,Leprechaun_Text,Prank,Caught_at_a_Good_Time,Ninja,Ninja_Text,Assassinate,Shuriken,Into_Shadows,Wall_Climb,Page,Page_Text,Swing,Partier,Partier_Text,Fight,Share_Drinks,Share_Snacks,Drinking_Game,Drinking_Game_Text,Pixie,Pixie_Text,Will_o_Wisp,Cast_Happiness,Cast_Happiness_Text,Cast_Sadness,Cast_Sadness_Text,Cast_Anger,Cast_Anger_Text,Puppeteer,Puppeteer_Text,Collision,Drop,Dangle,Control,Puppy,Puppy_Text,Bite,Leap,Bark,Cuddle,Rats,Rats_Text,Swarm,Strength_in_Numbers,Strength_in_Numbers_Text,Royalty,Royalty_Text,Order,Promote,Call,Procession,Siren,Siren_Text,Drown,Shipwreck,Manipulate,Sing,Spider,Spider_Text,Stab_Prey,Trapped,Silk_Wrap,Spin_Web,Taskmaster,Taskmaster_Text,Punishment,Demand_Happiness,Demand_Happiness_Text,Demand_Anger,Demand_Anger_Text,Demand_Sadness,Demand_Sadness_Text,Trickster,Trickster_Text,Mischief,Delude,Delay,Distract,Wall,Wall_Text,Toughen,In_the_Way,Wolf,Wolf_Text,Slash,Howl,Howl_Text,Nurture,Guard,Update_0,Update_0_Text,Update_1,Update_1_Text,Update_2,Update_2_Text,Upload_Translation,Download_English,Blank,Update_3,Update_3_Text,Update_4,Update_4_Text,Update_5,Update_5_Text
}
