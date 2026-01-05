using System.Collections.Generic;
public static class AutoTranslate
{

public static string Your_Timezone (string Time)  { return(Translator.inst.Translate("Your_Timezone", new(){("Time", Time)})); }

public static string Next_Challenge (string Time)  { return(Translator.inst.Translate("Next_Challenge", new(){("Time", Time)})); }

public static string Current_Date (string Month,string Day,string Year)  { return(Translator.inst.Translate("Current_Date", new(){("Month", Month),("Day", Day),("Year", Year)})); }

public static string Apply_Ineffectives_Fail (string This)  { return(Translator.inst.Translate("Apply_Ineffectives_Fail", new(){("This", This)})); }

public static string Super_Effective (string Num)  { return(Translator.inst.Translate("Super_Effective", new(){("Num", Num)})); }

public static string Not_Effective (string Num)  { return(Translator.inst.Translate("Not_Effective", new(){("Num", Num)})); }

public static string Increase_Cooldown (string Num,string Target,string MiscStat)  { return(Translator.inst.Translate("Increase_Cooldown", new(){("Num", Num),("Target", Target),("MiscStat", MiscStat)})); }

public static string Decrease_Cooldown (string Num,string Target,string MiscStat)  { return(Translator.inst.Translate("Decrease_Cooldown", new(){("Num", Num),("Target", Target),("MiscStat", MiscStat)})); }

public static string Apply_Cooldown (string Target,string Ability,string MiscStat)  { return(Translator.inst.Translate("Apply_Cooldown", new(){("Target", Target),("Ability", Ability),("MiscStat", MiscStat)})); }

public static string Out_of_Time (string This)  { return(Translator.inst.Translate("Out_of_Time", new(){("This", This)})); }

public static string Blocked_Stat_Drop (string This,string Num,string Stat)  { return(Translator.inst.Translate("Blocked_Stat_Drop", new(){("This", This),("Num", Num),("Stat", Stat)})); }

public static string Blocked_Emotion (string This)  { return(Translator.inst.Translate("Blocked_Emotion", new(){("This", This)})); }

public static string Blocked_Position (string This)  { return(Translator.inst.Translate("Blocked_Position", new(){("This", This)})); }

public static string Miss_Turn (string This)  { return(Translator.inst.Translate("Miss_Turn", new(){("This", This)})); }

public static string Gain_Extra_Ability (string This,string Num)  { return(Translator.inst.Translate("Gain_Extra_Ability", new(){("This", This),("Num", Num)})); }

public static string Use_Extra_Ability (string This)  { return(Translator.inst.Translate("Use_Extra_Ability", new(){("This", This)})); }

public static string Defeat_Waves (string Num)  { return(Translator.inst.Translate("Defeat_Waves", new(){("Num", Num)})); }

public static string Round (string Num)  { return(Translator.inst.Translate("Round", new(){("Num", Num)})); }

public static string Wave (string Num,string Max)  { return(Translator.inst.Translate("Wave", new(){("Num", Num),("Max", Max)})); }

public static string Enter_Fight (string This)  { return(Translator.inst.Translate("Enter_Fight", new(){("This", This)})); }

public static string Waves_Survived (string Num)  { return(Translator.inst.Translate("Waves_Survived", new(){("Num", Num)})); }

public static string Time_Taken (string Time)  { return(Translator.inst.Translate("Time_Taken", new(){("Time", Time)})); }

public static string Increase_Stat (string This,string Num,string Stat)  { return(Translator.inst.Translate("Increase_Stat", new(){("This", This),("Num", Num),("Stat", Stat)})); }

public static string Decrease_Stat (string This,string Num,string Stat)  { return(Translator.inst.Translate("Decrease_Stat", new(){("This", This),("Num", Num),("Stat", Stat)})); }

public static string Become_New (string This,string Change)  { return(Translator.inst.Translate("Become_New", new(){("This", This),("Change", Change)})); }

public static string Change_Status (string This,string Status,string Num)  { return(Translator.inst.Translate("Change_Status", new(){("This", This),("Status", Status),("Num", Num)})); }

public static string Died (string This)  { return(Translator.inst.Translate("Died", new(){("This", This)})); }

public static string Revived (string This)  { return(Translator.inst.Translate("Revived", new(){("This", This)})); }

public static string Emotion_Effect (string This,string Emotion)  { return(Translator.inst.Translate("Emotion_Effect", new(){("This", This),("Emotion", Emotion)})); }

public static string Choose_Ability (string This)  { return(Translator.inst.Translate("Choose_Ability", new(){("This", This)})); }

public static string Must_Choose_Targeted (string Target)  { return(Translator.inst.Translate("Must_Choose_Targeted", new(){("Target", Target)})); }

public static string Choose_One_Player (string Ability)  { return(Translator.inst.Translate("Choose_One_Player", new(){("Ability", Ability)})); }

public static string Choose_Another_Player (string Ability)  { return(Translator.inst.Translate("Choose_Another_Player", new(){("Ability", Ability)})); }

public static string Choose_An_Enemy (string Ability)  { return(Translator.inst.Translate("Choose_An_Enemy", new(){("Ability", Ability)})); }

public static string Confirm_No_Target (string Ability)  { return(Translator.inst.Translate("Confirm_No_Target", new(){("Ability", Ability)})); }

public static string Confirm_Target (string Ability)  { return(Translator.inst.Translate("Confirm_Target", new(){("Ability", Ability)})); }

public static string Battle_Cry_Text (string DefenseStat)  { return(Translator.inst.Translate("Battle_Cry_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Battle_Cry_Log (string This,string Target)  { return(Translator.inst.Translate("Battle_Cry_Log", new(){("This", This),("Target", Target)})); }

public static string Cheer_Text (string MiscStat)  { return(Translator.inst.Translate("Cheer_Text", new(){("MiscStat", MiscStat)})); }

public static string Cheer_Log (string This,string Target)  { return(Translator.inst.Translate("Cheer_Log", new(){("This", This),("Target", Target)})); }

public static string Daze_Text (string DefenseStat)  { return(Translator.inst.Translate("Daze_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Daze_Log (string This,string Target)  { return(Translator.inst.Translate("Daze_Log", new(){("This", This),("Target", Target)})); }

public static string Desperation_Text (string Num)  { return(Translator.inst.Translate("Desperation_Text", new(){("Num", Num)})); }

public static string Desperation_Log (string This,string Target)  { return(Translator.inst.Translate("Desperation_Log", new(){("This", This),("Target", Target)})); }

public static string Embarass_Text (string Num)  { return(Translator.inst.Translate("Embarass_Text", new(){("Num", Num)})); }

public static string Embarass_Log (string This,string Target)  { return(Translator.inst.Translate("Embarass_Log", new(){("This", This),("Target", Target)})); }

public static string Force_Text (string Num,string MiscStat)  { return(Translator.inst.Translate("Force_Text", new(){("Num", Num),("MiscStat", MiscStat)})); }

public static string Force_Log (string This,string Target)  { return(Translator.inst.Translate("Force_Log", new(){("This", This),("Target", Target)})); }

public static string Impale_Text (string Num,string MiscStat)  { return(Translator.inst.Translate("Impale_Text", new(){("Num", Num),("MiscStat", MiscStat)})); }

public static string Impale_Log (string This,string Target)  { return(Translator.inst.Translate("Impale_Log", new(){("This", This),("Target", Target)})); }

public static string Intimidate_Log (string This,string Target)  { return(Translator.inst.Translate("Intimidate_Log", new(){("This", This),("Target", Target)})); }

public static string Joust_Text (string Num,string PowerStat)  { return(Translator.inst.Translate("Joust_Text", new(){("Num", Num),("PowerStat", PowerStat)})); }

public static string Joust_Log (string This,string Target)  { return(Translator.inst.Translate("Joust_Log", new(){("This", This),("Target", Target)})); }

public static string Knock_Down_Text (string Num,string DefenseStat)  { return(Translator.inst.Translate("Knock_Down_Text", new(){("Num", Num),("DefenseStat", DefenseStat)})); }

public static string Knock_Down_Log (string This,string Target)  { return(Translator.inst.Translate("Knock_Down_Log", new(){("This", This),("Target", Target)})); }

public static string Meditate_Text (string MiscStat)  { return(Translator.inst.Translate("Meditate_Text", new(){("MiscStat", MiscStat)})); }

public static string Meditate_Log (string This,string Target)  { return(Translator.inst.Translate("Meditate_Log", new(){("This", This),("Target", Target)})); }

public static string Mirror_Text (string Num)  { return(Translator.inst.Translate("Mirror_Text", new(){("Num", Num)})); }

public static string Mirror_Log (string This,string Target)  { return(Translator.inst.Translate("Mirror_Log", new(){("This", This),("Target", Target)})); }

public static string Mock_Text (string Num)  { return(Translator.inst.Translate("Mock_Text", new(){("Num", Num)})); }

public static string Mock_Log (string This,string Target)  { return(Translator.inst.Translate("Mock_Log", new(){("This", This),("Target", Target)})); }

public static string Neutralize_Text (string Num)  { return(Translator.inst.Translate("Neutralize_Text", new(){("Num", Num)})); }

public static string Neutralize_Log (string This,string Target)  { return(Translator.inst.Translate("Neutralize_Log", new(){("This", This),("Target", Target)})); }

public static string Redirect_Text (string DefenseStat,string MiscStat)  { return(Translator.inst.Translate("Redirect_Text", new(){("DefenseStat", DefenseStat),("MiscStat", MiscStat)})); }

public static string Redirect_Log (string This,string Target)  { return(Translator.inst.Translate("Redirect_Log", new(){("This", This),("Target", Target)})); }

public static string Strike_Text (string DefenseStat,string Num)  { return(Translator.inst.Translate("Strike_Text", new(){("DefenseStat", DefenseStat),("Num", Num)})); }

public static string Strike_Log (string This,string Target)  { return(Translator.inst.Translate("Strike_Log", new(){("This", This),("Target", Target)})); }

public static string Steal_Text (string DefenseStat)  { return(Translator.inst.Translate("Steal_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Steal_Log (string This,string Target)  { return(Translator.inst.Translate("Steal_Log", new(){("This", This),("Target", Target)})); }

public static string Surprise_Text (string Num)  { return(Translator.inst.Translate("Surprise_Text", new(){("Num", Num)})); }

public static string Surprise_Log (string This,string Target)  { return(Translator.inst.Translate("Surprise_Log", new(){("This", This),("Target", Target)})); }

public static string Twirl_Text (string Num)  { return(Translator.inst.Translate("Twirl_Text", new(){("Num", Num)})); }

public static string Twirl_Log (string This,string Target)  { return(Translator.inst.Translate("Twirl_Log", new(){("This", This),("Target", Target)})); }

public static string Upset_Text (string DefenseStat)  { return(Translator.inst.Translate("Upset_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Upset_Log (string This,string Target)  { return(Translator.inst.Translate("Upset_Log", new(){("This", This),("Target", Target)})); }

public static string Immobilize_Text (string Num,string MiscStat)  { return(Translator.inst.Translate("Immobilize_Text", new(){("Num", Num),("MiscStat", MiscStat)})); }

public static string Immobilize_Log (string This,string Target)  { return(Translator.inst.Translate("Immobilize_Log", new(){("This", This),("Target", Target)})); }

public static string Air_Combat_Text (string PowerStat)  { return(Translator.inst.Translate("Air_Combat_Text", new(){("PowerStat", PowerStat)})); }

public static string Air_Combat_Log (string This)  { return(Translator.inst.Translate("Air_Combat_Log", new(){("This", This)})); }

public static string Assist_Text (string Num,string MiscStat)  { return(Translator.inst.Translate("Assist_Text", new(){("Num", Num),("MiscStat", MiscStat)})); }

public static string Assist_Log (string This,string Target)  { return(Translator.inst.Translate("Assist_Log", new(){("This", This),("Target", Target)})); }

public static string Calm_Down_Text (string Num)  { return(Translator.inst.Translate("Calm_Down_Text", new(){("Num", Num)})); }

public static string Calm_Down_Log (string This)  { return(Translator.inst.Translate("Calm_Down_Log", new(){("This", This)})); }

public static string Crash_Land_Text (string DefenseStat)  { return(Translator.inst.Translate("Crash_Land_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Crash_Land_Log (string This,string Target)  { return(Translator.inst.Translate("Crash_Land_Log", new(){("This", This),("Target", Target)})); }

public static string Enrage_Text (string Num)  { return(Translator.inst.Translate("Enrage_Text", new(){("Num", Num)})); }

public static string Enrage_Log (string This,string Target)  { return(Translator.inst.Translate("Enrage_Log", new(){("This", This),("Target", Target)})); }

public static string Exhaust_Text (string PowerStat)  { return(Translator.inst.Translate("Exhaust_Text", new(){("PowerStat", PowerStat)})); }

public static string Exhaust_Log (string This,string Target)  { return(Translator.inst.Translate("Exhaust_Log", new(){("This", This),("Target", Target)})); }

public static string Exorcism_Text (string DefenseStat,string Num)  { return(Translator.inst.Translate("Exorcism_Text", new(){("DefenseStat", DefenseStat),("Num", Num)})); }

public static string Exorcism_Log (string This,string Target)  { return(Translator.inst.Translate("Exorcism_Log", new(){("This", This),("Target", Target)})); }

public static string Gift_of_Flight_Log (string This,string Target)  { return(Translator.inst.Translate("Gift_of_Flight_Log", new(){("This", This),("Target", Target)})); }

public static string Hide_Text (string PowerStat,string Num)  { return(Translator.inst.Translate("Hide_Text", new(){("PowerStat", PowerStat),("Num", Num)})); }

public static string Hide_Log (string This,string Target)  { return(Translator.inst.Translate("Hide_Log", new(){("This", This),("Target", Target)})); }

public static string Immunity_Text (string Num,string MiscStat)  { return(Translator.inst.Translate("Immunity_Text", new(){("Num", Num),("MiscStat", MiscStat)})); }

public static string Immunity_Log (string This,string Target)  { return(Translator.inst.Translate("Immunity_Log", new(){("This", This),("Target", Target)})); }

public static string Joy_Text (string Num)  { return(Translator.inst.Translate("Joy_Text", new(){("Num", Num)})); }

public static string Joy_Log (string This,string Target)  { return(Translator.inst.Translate("Joy_Log", new(){("This", This),("Target", Target)})); }

public static string Lift_Up_Text (string Num)  { return(Translator.inst.Translate("Lift_Up_Text", new(){("Num", Num)})); }

public static string Lift_Up_Log (string This,string Target)  { return(Translator.inst.Translate("Lift_Up_Log", new(){("This", This),("Target", Target)})); }

public static string Motivate_Text (string Num,string MiscStat)  { return(Translator.inst.Translate("Motivate_Text", new(){("Num", Num),("MiscStat", MiscStat)})); }

public static string Motivate_Log (string This,string Target)  { return(Translator.inst.Translate("Motivate_Log", new(){("This", This),("Target", Target)})); }

public static string Overheal_Text (string MiscStat,string Num)  { return(Translator.inst.Translate("Overheal_Text", new(){("MiscStat", MiscStat),("Num", Num)})); }

public static string Overheal_Log (string This)  { return(Translator.inst.Translate("Overheal_Log", new(){("This", This)})); }

public static string Petrify_Text (string MiscStat)  { return(Translator.inst.Translate("Petrify_Text", new(){("MiscStat", MiscStat)})); }

public static string Petrify_Log (string This,string Target)  { return(Translator.inst.Translate("Petrify_Log", new(){("This", This),("Target", Target)})); }

public static string Plummet_Log (string This,string Target)  { return(Translator.inst.Translate("Plummet_Log", new(){("This", This),("Target", Target)})); }

public static string Retreat_Log (string This,string Target)  { return(Translator.inst.Translate("Retreat_Log", new(){("This", This),("Target", Target)})); }

public static string Security_Text (string DefenseStat)  { return(Translator.inst.Translate("Security_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Security_Log (string This,string Target)  { return(Translator.inst.Translate("Security_Log", new(){("This", This),("Target", Target)})); }

public static string Soft_Landing_Text (string Num)  { return(Translator.inst.Translate("Soft_Landing_Text", new(){("Num", Num)})); }

public static string Soft_Landing_Log (string This,string Target)  { return(Translator.inst.Translate("Soft_Landing_Log", new(){("This", This),("Target", Target)})); }

public static string Tailwinds_Text (string Num,string DefenseStat)  { return(Translator.inst.Translate("Tailwinds_Text", new(){("Num", Num),("DefenseStat", DefenseStat)})); }

public static string Tailwinds_Log (string This)  { return(Translator.inst.Translate("Tailwinds_Log", new(){("This", This)})); }

public static string Team_Up_Text (string Num,string PowerStat)  { return(Translator.inst.Translate("Team_Up_Text", new(){("Num", Num),("PowerStat", PowerStat)})); }

public static string Team_Up_Log (string This)  { return(Translator.inst.Translate("Team_Up_Log", new(){("This", This)})); }

public static string Above_Danger_Text (string MiscStat)  { return(Translator.inst.Translate("Above_Danger_Text", new(){("MiscStat", MiscStat)})); }

public static string Above_Danger_Log (string This,string Target)  { return(Translator.inst.Translate("Above_Danger_Log", new(){("This", This),("Target", Target)})); }

public static string Bad_Omens_Text (string Num,string PowerStat)  { return(Translator.inst.Translate("Bad_Omens_Text", new(){("Num", Num),("PowerStat", PowerStat)})); }

public static string Bad_Omens_Log (string This)  { return(Translator.inst.Translate("Bad_Omens_Log", new(){("This", This)})); }

public static string Bounce_Text (string Num)  { return(Translator.inst.Translate("Bounce_Text", new(){("Num", Num)})); }

public static string Bounce_Log (string This)  { return(Translator.inst.Translate("Bounce_Log", new(){("This", This)})); }

public static string Chill_Text (string Num,string MiscStat)  { return(Translator.inst.Translate("Chill_Text", new(){("Num", Num),("MiscStat", MiscStat)})); }

public static string Chill_Log (string This)  { return(Translator.inst.Translate("Chill_Log", new(){("This", This)})); }

public static string Accelerate_Text (string PowerStat,string MiscStat)  { return(Translator.inst.Translate("Accelerate_Text", new(){("PowerStat", PowerStat),("MiscStat", MiscStat)})); }

public static string Accelerate_Log (string This,string Target)  { return(Translator.inst.Translate("Accelerate_Log", new(){("This", This),("Target", Target)})); }

public static string Flood_Text (string Num)  { return(Translator.inst.Translate("Flood_Text", new(){("Num", Num)})); }

public static string Flood_Log (string This)  { return(Translator.inst.Translate("Flood_Log", new(){("This", This)})); }

public static string Gust_Text (string PowerStat)  { return(Translator.inst.Translate("Gust_Text", new(){("PowerStat", PowerStat)})); }

public static string Gust_Log (string This,string Target)  { return(Translator.inst.Translate("Gust_Log", new(){("This", This),("Target", Target)})); }

public static string Headwinds_Text (string Num,string PowerStat)  { return(Translator.inst.Translate("Headwinds_Text", new(){("Num", Num),("PowerStat", PowerStat)})); }

public static string Headwinds_Log (string This)  { return(Translator.inst.Translate("Headwinds_Log", new(){("This", This)})); }

public static string Crown_Text (string PowerStat,string MiscStat)  { return(Translator.inst.Translate("Crown_Text", new(){("PowerStat", PowerStat),("MiscStat", MiscStat)})); }

public static string Crown_Log (string This,string Target)  { return(Translator.inst.Translate("Crown_Log", new(){("This", This),("Target", Target)})); }

public static string Storm_Text (string Num)  { return(Translator.inst.Translate("Storm_Text", new(){("Num", Num)})); }

public static string Storm_Log (string This)  { return(Translator.inst.Translate("Storm_Log", new(){("This", This)})); }

public static string Manipulate_Time_Text (string Num,string MiscStat)  { return(Translator.inst.Translate("Manipulate_Time_Text", new(){("Num", Num),("MiscStat", MiscStat)})); }

public static string Manipulate_Time_Log (string This)  { return(Translator.inst.Translate("Manipulate_Time_Log", new(){("This", This)})); }

public static string Bind_Text (string MiscStat)  { return(Translator.inst.Translate("Bind_Text", new(){("MiscStat", MiscStat)})); }

public static string Bind_Log (string This,string Target)  { return(Translator.inst.Translate("Bind_Log", new(){("This", This),("Target", Target)})); }

public static string Punch_Up_Text (string Num,string MiscStat)  { return(Translator.inst.Translate("Punch_Up_Text", new(){("Num", Num),("MiscStat", MiscStat)})); }

public static string Punch_Up_Log (string This)  { return(Translator.inst.Translate("Punch_Up_Log", new(){("This", This)})); }

public static string Quick_Attack_Text (string PowerStat,string Num)  { return(Translator.inst.Translate("Quick_Attack_Text", new(){("PowerStat", PowerStat),("Num", Num)})); }

public static string Quick_Attack_Log (string This,string Target)  { return(Translator.inst.Translate("Quick_Attack_Log", new(){("This", This),("Target", Target)})); }

public static string Readjust_Log (string This)  { return(Translator.inst.Translate("Readjust_Log", new(){("This", This)})); }

public static string Restrain_Text (string Num,string MiscStat)  { return(Translator.inst.Translate("Restrain_Text", new(){("Num", Num),("MiscStat", MiscStat)})); }

public static string Restrain_Log (string This)  { return(Translator.inst.Translate("Restrain_Log", new(){("This", This)})); }

public static string Shockwave_Text (string Num,string Sec)  { return(Translator.inst.Translate("Shockwave_Text", new(){("Num", Num),("Sec", Sec)})); }

public static string Shockwave_Log (string This)  { return(Translator.inst.Translate("Shockwave_Log", new(){("This", This)})); }

public static string Stalactites_Text (string Num)  { return(Translator.inst.Translate("Stalactites_Text", new(){("Num", Num)})); }

public static string Stalactites_Log (string This)  { return(Translator.inst.Translate("Stalactites_Log", new(){("This", This)})); }

public static string Team_Attack_Text (string Num)  { return(Translator.inst.Translate("Team_Attack_Text", new(){("Num", Num)})); }

public static string Team_Attack_Log (string This)  { return(Translator.inst.Translate("Team_Attack_Log", new(){("This", This)})); }

public static string Touchdown_Text (string PowerStat)  { return(Translator.inst.Translate("Touchdown_Text", new(){("PowerStat", PowerStat)})); }

public static string Touchdown_Log (string This,string Target)  { return(Translator.inst.Translate("Touchdown_Log", new(){("This", This),("Target", Target)})); }

public static string Warp_Text (string PowerStat)  { return(Translator.inst.Translate("Warp_Text", new(){("PowerStat", PowerStat)})); }

public static string Warp_Log (string This,string Target)  { return(Translator.inst.Translate("Warp_Log", new(){("This", This),("Target", Target)})); }

public static string Revive_Text (string Num)  { return(Translator.inst.Translate("Revive_Text", new(){("Num", Num)})); }

public static string Revive_Log (string This,string Target)  { return(Translator.inst.Translate("Revive_Log", new(){("This", This),("Target", Target)})); }

public static string Skip_Turn_Log (string This)  { return(Translator.inst.Translate("Skip_Turn_Log", new(){("This", This)})); }

public static string Quick_Shot_Text (string Num)  { return(Translator.inst.Translate("Quick_Shot_Text", new(){("Num", Num)})); }

public static string Quick_Shot_Log (string This,string Target)  { return(Translator.inst.Translate("Quick_Shot_Log", new(){("This", This),("Target", Target)})); }

public static string Bullseye_Text (string PowerStat,string Num)  { return(Translator.inst.Translate("Bullseye_Text", new(){("PowerStat", PowerStat),("Num", Num)})); }

public static string Bullseye_Log (string This,string Target)  { return(Translator.inst.Translate("Bullseye_Log", new(){("This", This),("Target", Target)})); }

public static string Volley_Text (string Num)  { return(Translator.inst.Translate("Volley_Text", new(){("Num", Num)})); }

public static string Volley_Log (string This)  { return(Translator.inst.Translate("Volley_Log", new(){("This", This)})); }

public static string Focus_Text (string DefenseStat)  { return(Translator.inst.Translate("Focus_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Focus_Log (string This)  { return(Translator.inst.Translate("Focus_Log", new(){("This", This)})); }

public static string Fury_Text (string Num)  { return(Translator.inst.Translate("Fury_Text", new(){("Num", Num)})); }

public static string Fury_Log (string This,string Target)  { return(Translator.inst.Translate("Fury_Log", new(){("This", This),("Target", Target)})); }

public static string Savagery_Text (string PowerStat)  { return(Translator.inst.Translate("Savagery_Text", new(){("PowerStat", PowerStat)})); }

public static string Savagery_Log (string This)  { return(Translator.inst.Translate("Savagery_Log", new(){("This", This)})); }

public static string Brawl_Text (string MiscStat)  { return(Translator.inst.Translate("Brawl_Text", new(){("MiscStat", MiscStat)})); }

public static string Brawl_Log (string This,string Target)  { return(Translator.inst.Translate("Brawl_Log", new(){("This", This),("Target", Target)})); }

public static string Camp_Text (string MiscStat)  { return(Translator.inst.Translate("Camp_Text", new(){("MiscStat", MiscStat)})); }

public static string Camp_Log (string This)  { return(Translator.inst.Translate("Camp_Log", new(){("This", This)})); }

public static string Drain_Text (string Num)  { return(Translator.inst.Translate("Drain_Text", new(){("Num", Num)})); }

public static string Drain_Log (string This,string Target)  { return(Translator.inst.Translate("Drain_Log", new(){("This", This),("Target", Target)})); }

public static string Screech_Text (string DefenseStat)  { return(Translator.inst.Translate("Screech_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Screech_Log (string This)  { return(Translator.inst.Translate("Screech_Log", new(){("This", This)})); }

public static string Hunting_Season_Text (string PowerStat)  { return(Translator.inst.Translate("Hunting_Season_Text", new(){("PowerStat", PowerStat)})); }

public static string Hunting_Season_Log (string This)  { return(Translator.inst.Translate("Hunting_Season_Log", new(){("This", This)})); }

public static string Bloodthirst_Text (string MiscStat)  { return(Translator.inst.Translate("Bloodthirst_Text", new(){("MiscStat", MiscStat)})); }

public static string Bloodthirst_Log (string This)  { return(Translator.inst.Translate("Bloodthirst_Log", new(){("This", This)})); }

public static string Sting_Text (string MiscStat)  { return(Translator.inst.Translate("Sting_Text", new(){("MiscStat", MiscStat)})); }

public static string Sting_Log (string This,string Target)  { return(Translator.inst.Translate("Sting_Log", new(){("This", This),("Target", Target)})); }

public static string Honey_Text (string Num)  { return(Translator.inst.Translate("Honey_Text", new(){("Num", Num)})); }

public static string Honey_Log (string This,string Target)  { return(Translator.inst.Translate("Honey_Log", new(){("This", This),("Target", Target)})); }

public static string Protect_the_Queen_Text (string MiscStat)  { return(Translator.inst.Translate("Protect_the_Queen_Text", new(){("MiscStat", MiscStat)})); }

public static string Protect_the_Queen_Log (string This,string Target)  { return(Translator.inst.Translate("Protect_the_Queen_Log", new(){("This", This),("Target", Target)})); }

public static string Support_Text (string MiscStat)  { return(Translator.inst.Translate("Support_Text", new(){("MiscStat", MiscStat)})); }

public static string Support_Log (string This,string Target)  { return(Translator.inst.Translate("Support_Log", new(){("This", This),("Target", Target)})); }

public static string Charge_Text (string Num)  { return(Translator.inst.Translate("Charge_Text", new(){("Num", Num)})); }

public static string Charge_Log (string This,string Target)  { return(Translator.inst.Translate("Charge_Log", new(){("This", This),("Target", Target)})); }

public static string Milk_Text (string Num)  { return(Translator.inst.Translate("Milk_Text", new(){("Num", Num)})); }

public static string Milk_Log (string This,string Target)  { return(Translator.inst.Translate("Milk_Log", new(){("This", This),("Target", Target)})); }

public static string Graze_Text (string Num)  { return(Translator.inst.Translate("Graze_Text", new(){("Num", Num)})); }

public static string Graze_Log (string This)  { return(Translator.inst.Translate("Graze_Log", new(){("This", This)})); }

public static string Bellow_Text (string DefenseStat,string MiscStat)  { return(Translator.inst.Translate("Bellow_Text", new(){("DefenseStat", DefenseStat),("MiscStat", MiscStat)})); }

public static string Bellow_Log (string This)  { return(Translator.inst.Translate("Bellow_Log", new(){("This", This)})); }

public static string Peck_Text (string Num)  { return(Translator.inst.Translate("Peck_Text", new(){("Num", Num)})); }

public static string Peck_Log (string This,string Target)  { return(Translator.inst.Translate("Peck_Log", new(){("This", This),("Target", Target)})); }

public static string Dark_Energy_Text (string Num)  { return(Translator.inst.Translate("Dark_Energy_Text", new(){("Num", Num)})); }

public static string Dark_Energy_Log (string This)  { return(Translator.inst.Translate("Dark_Energy_Log", new(){("This", This)})); }

public static string Betrayal_Text (string Num,string MiscStat,string PowerStat)  { return(Translator.inst.Translate("Betrayal_Text", new(){("Num", Num),("MiscStat", MiscStat),("PowerStat", PowerStat)})); }

public static string Betrayal_Log (string This,string Target)  { return(Translator.inst.Translate("Betrayal_Log", new(){("This", This),("Target", Target)})); }

public static string Ritual_Text (string MiscStat)  { return(Translator.inst.Translate("Ritual_Text", new(){("MiscStat", MiscStat)})); }

public static string Ritual_Log (string This,string Target)  { return(Translator.inst.Translate("Ritual_Log", new(){("This", This),("Target", Target)})); }

public static string Demand_Energy_Text (string PowerStat)  { return(Translator.inst.Translate("Demand_Energy_Text", new(){("PowerStat", PowerStat)})); }

public static string Demand_Energy_Log (string This)  { return(Translator.inst.Translate("Demand_Energy_Log", new(){("This", This)})); }

public static string Firebreath_Text (string Num)  { return(Translator.inst.Translate("Firebreath_Text", new(){("Num", Num)})); }

public static string Firebreath_Log (string This)  { return(Translator.inst.Translate("Firebreath_Log", new(){("This", This)})); }

public static string Roost_Text (string Num)  { return(Translator.inst.Translate("Roost_Text", new(){("Num", Num)})); }

public static string Roost_Log (string This)  { return(Translator.inst.Translate("Roost_Log", new(){("This", This)})); }

public static string Take_Flight_Log (string This)  { return(Translator.inst.Translate("Take_Flight_Log", new(){("This", This)})); }

public static string Roar_Text (string PowerStat)  { return(Translator.inst.Translate("Roar_Text", new(){("PowerStat", PowerStat)})); }

public static string Roar_Log (string This)  { return(Translator.inst.Translate("Roar_Log", new(){("This", This)})); }

public static string Sound_Waves_Text (string Num)  { return(Translator.inst.Translate("Sound_Waves_Text", new(){("Num", Num)})); }

public static string Sound_Waves_Log (string This)  { return(Translator.inst.Translate("Sound_Waves_Log", new(){("This", This)})); }

public static string Alarm_Text (string MiscStat)  { return(Translator.inst.Translate("Alarm_Text", new(){("MiscStat", MiscStat)})); }

public static string Alarm_Log (string This)  { return(Translator.inst.Translate("Alarm_Log", new(){("This", This)})); }

public static string Jumpscare_Text (string Num)  { return(Translator.inst.Translate("Jumpscare_Text", new(){("Num", Num)})); }

public static string Jumpscare_Log (string This,string Target)  { return(Translator.inst.Translate("Jumpscare_Log", new(){("This", This),("Target", Target)})); }

public static string Lament_Log (string This,string Target)  { return(Translator.inst.Translate("Lament_Log", new(){("This", This),("Target", Target)})); }

public static string Possession_Text (string PowerStat)  { return(Translator.inst.Translate("Possession_Text", new(){("PowerStat", PowerStat)})); }

public static string Possession_Log (string This,string Target)  { return(Translator.inst.Translate("Possession_Log", new(){("This", This),("Target", Target)})); }

public static string Release_Text (string MiscStat)  { return(Translator.inst.Translate("Release_Text", new(){("MiscStat", MiscStat)})); }

public static string Release_Log (string This,string Target)  { return(Translator.inst.Translate("Release_Log", new(){("This", This),("Target", Target)})); }

public static string Flag_Whack_Text (string Num)  { return(Translator.inst.Translate("Flag_Whack_Text", new(){("Num", Num)})); }

public static string Flag_Whack_Log (string This,string Target)  { return(Translator.inst.Translate("Flag_Whack_Log", new(){("This", This),("Target", Target)})); }

public static string Announce_Text (string MiscStat)  { return(Translator.inst.Translate("Announce_Text", new(){("MiscStat", MiscStat)})); }

public static string Announce_Log (string This)  { return(Translator.inst.Translate("Announce_Log", new(){("This", This)})); }

public static string Scratch_Text (string Num)  { return(Translator.inst.Translate("Scratch_Text", new(){("Num", Num)})); }

public static string Scratch_Log (string This,string Target)  { return(Translator.inst.Translate("Scratch_Log", new(){("This", This),("Target", Target)})); }

public static string Stuck_in_a_Tree_Log (string This)  { return(Translator.inst.Translate("Stuck_in_a_Tree_Log", new(){("This", This)})); }

public static string Land_Feet_First_Text (string Num)  { return(Translator.inst.Translate("Land_Feet_First_Text", new(){("Num", Num)})); }

public static string Land_Feet_First_Log (string This)  { return(Translator.inst.Translate("Land_Feet_First_Log", new(){("This", This)})); }

public static string Lure_Text (string MiscStat)  { return(Translator.inst.Translate("Lure_Text", new(){("MiscStat", MiscStat)})); }

public static string Lure_Log (string This,string Target)  { return(Translator.inst.Translate("Lure_Log", new(){("This", This),("Target", Target)})); }

public static string Prank_Text (string Num)  { return(Translator.inst.Translate("Prank_Text", new(){("Num", Num)})); }

public static string Prank_Log (string This)  { return(Translator.inst.Translate("Prank_Log", new(){("This", This)})); }

public static string Caught_at_a_Good_Time_Text (string PowerStat,string DefenseStat)  { return(Translator.inst.Translate("Caught_at_a_Good_Time_Text", new(){("PowerStat", PowerStat),("DefenseStat", DefenseStat)})); }

public static string Caught_at_a_Good_Time_Log (string This)  { return(Translator.inst.Translate("Caught_at_a_Good_Time_Log", new(){("This", This)})); }

public static string Assassinate_Text (string Num)  { return(Translator.inst.Translate("Assassinate_Text", new(){("Num", Num)})); }

public static string Assassinate_Log (string This,string Target)  { return(Translator.inst.Translate("Assassinate_Log", new(){("This", This),("Target", Target)})); }

public static string Shuriken_Text (string PowerStat,string DefenseStat)  { return(Translator.inst.Translate("Shuriken_Text", new(){("PowerStat", PowerStat),("DefenseStat", DefenseStat)})); }

public static string Shuriken_Log (string This,string Target)  { return(Translator.inst.Translate("Shuriken_Log", new(){("This", This),("Target", Target)})); }

public static string Into_Shadows_Text (string MiscStat)  { return(Translator.inst.Translate("Into_Shadows_Text", new(){("MiscStat", MiscStat)})); }

public static string Into_Shadows_Log (string This)  { return(Translator.inst.Translate("Into_Shadows_Log", new(){("This", This)})); }

public static string Wall_Climb_Text (string DefenseStat)  { return(Translator.inst.Translate("Wall_Climb_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Wall_Climb_Log (string This)  { return(Translator.inst.Translate("Wall_Climb_Log", new(){("This", This)})); }

public static string Swing_Text (string Num)  { return(Translator.inst.Translate("Swing_Text", new(){("Num", Num)})); }

public static string Swing_Log (string This,string Target)  { return(Translator.inst.Translate("Swing_Log", new(){("This", This),("Target", Target)})); }

public static string Fight_Text (string Num)  { return(Translator.inst.Translate("Fight_Text", new(){("Num", Num)})); }

public static string Fight_Log (string This,string Target)  { return(Translator.inst.Translate("Fight_Log", new(){("This", This),("Target", Target)})); }

public static string Share_Drinks_Text (string Num)  { return(Translator.inst.Translate("Share_Drinks_Text", new(){("Num", Num)})); }

public static string Share_Drinks_Log (string This)  { return(Translator.inst.Translate("Share_Drinks_Log", new(){("This", This)})); }

public static string Share_Snacks_Text (string MiscStat)  { return(Translator.inst.Translate("Share_Snacks_Text", new(){("MiscStat", MiscStat)})); }

public static string Share_Snacks_Log (string This)  { return(Translator.inst.Translate("Share_Snacks_Log", new(){("This", This)})); }

public static string Drinking_Game_Log (string This)  { return(Translator.inst.Translate("Drinking_Game_Log", new(){("This", This)})); }

public static string Will_o_Wisp_Text (string Num)  { return(Translator.inst.Translate("Will_o_Wisp_Text", new(){("Num", Num)})); }

public static string Will_o_Wisp_Log (string This,string Target)  { return(Translator.inst.Translate("Will_o_Wisp_Log", new(){("This", This),("Target", Target)})); }

public static string Cast_Happiness_Log (string This)  { return(Translator.inst.Translate("Cast_Happiness_Log", new(){("This", This)})); }

public static string Cast_Sadness_Log (string This)  { return(Translator.inst.Translate("Cast_Sadness_Log", new(){("This", This)})); }

public static string Cast_Anger_Log (string This)  { return(Translator.inst.Translate("Cast_Anger_Log", new(){("This", This)})); }

public static string Collision_Text (string Num)  { return(Translator.inst.Translate("Collision_Text", new(){("Num", Num)})); }

public static string Collision_Log (string This)  { return(Translator.inst.Translate("Collision_Log", new(){("This", This)})); }

public static string Drop_Text (string Num)  { return(Translator.inst.Translate("Drop_Text", new(){("Num", Num)})); }

public static string Drop_Log (string This)  { return(Translator.inst.Translate("Drop_Log", new(){("This", This)})); }

public static string Dangle_Text (string DefenseStat)  { return(Translator.inst.Translate("Dangle_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Dangle_Log (string This)  { return(Translator.inst.Translate("Dangle_Log", new(){("This", This)})); }

public static string Control_Text (string MiscStat)  { return(Translator.inst.Translate("Control_Text", new(){("MiscStat", MiscStat)})); }

public static string Control_Log (string This)  { return(Translator.inst.Translate("Control_Log", new(){("This", This)})); }

public static string Bite_Text (string Num)  { return(Translator.inst.Translate("Bite_Text", new(){("Num", Num)})); }

public static string Bite_Log (string This,string Target)  { return(Translator.inst.Translate("Bite_Log", new(){("This", This),("Target", Target)})); }

public static string Leap_Text (string Num)  { return(Translator.inst.Translate("Leap_Text", new(){("Num", Num)})); }

public static string Leap_Log (string This,string Target)  { return(Translator.inst.Translate("Leap_Log", new(){("This", This),("Target", Target)})); }

public static string Bark_Text (string DefenseStat)  { return(Translator.inst.Translate("Bark_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Bark_Log (string This)  { return(Translator.inst.Translate("Bark_Log", new(){("This", This)})); }

public static string Cuddle_Text (string MiscStat,string DefenseStat)  { return(Translator.inst.Translate("Cuddle_Text", new(){("MiscStat", MiscStat),("DefenseStat", DefenseStat)})); }

public static string Cuddle_Log (string This,string Target)  { return(Translator.inst.Translate("Cuddle_Log", new(){("This", This),("Target", Target)})); }

public static string Swarm_Text (string Num,string DefenseStat)  { return(Translator.inst.Translate("Swarm_Text", new(){("Num", Num),("DefenseStat", DefenseStat)})); }

public static string Swarm_Log (string This,string Target)  { return(Translator.inst.Translate("Swarm_Log", new(){("This", This),("Target", Target)})); }

public static string Strength_in_Numbers_Log (string This)  { return(Translator.inst.Translate("Strength_in_Numbers_Log", new(){("This", This)})); }

public static string Order_Text (string Num,string MiscStat)  { return(Translator.inst.Translate("Order_Text", new(){("Num", Num),("MiscStat", MiscStat)})); }

public static string Order_Log (string This,string Target)  { return(Translator.inst.Translate("Order_Log", new(){("This", This),("Target", Target)})); }

public static string Promote_Text (string MiscStat,string Num)  { return(Translator.inst.Translate("Promote_Text", new(){("MiscStat", MiscStat),("Num", Num)})); }

public static string Promote_Log (string This,string Target)  { return(Translator.inst.Translate("Promote_Log", new(){("This", This),("Target", Target)})); }

public static string Call_Text (string MiscStat)  { return(Translator.inst.Translate("Call_Text", new(){("MiscStat", MiscStat)})); }

public static string Call_Log (string This)  { return(Translator.inst.Translate("Call_Log", new(){("This", This)})); }

public static string Procession_Text (string PowerStat)  { return(Translator.inst.Translate("Procession_Text", new(){("PowerStat", PowerStat)})); }

public static string Procession_Log (string This)  { return(Translator.inst.Translate("Procession_Log", new(){("This", This)})); }

public static string Drown_Text (string Num)  { return(Translator.inst.Translate("Drown_Text", new(){("Num", Num)})); }

public static string Drown_Log (string This,string Target)  { return(Translator.inst.Translate("Drown_Log", new(){("This", This),("Target", Target)})); }

public static string Shipwreck_Text (string DefenseStat,string MiscStat)  { return(Translator.inst.Translate("Shipwreck_Text", new(){("DefenseStat", DefenseStat),("MiscStat", MiscStat)})); }

public static string Shipwreck_Log (string This,string Target)  { return(Translator.inst.Translate("Shipwreck_Log", new(){("This", This),("Target", Target)})); }

public static string Manipulate_Text (string MiscStat)  { return(Translator.inst.Translate("Manipulate_Text", new(){("MiscStat", MiscStat)})); }

public static string Manipulate_Log (string This,string Target)  { return(Translator.inst.Translate("Manipulate_Log", new(){("This", This),("Target", Target)})); }

public static string Sing_Text (string MiscStat)  { return(Translator.inst.Translate("Sing_Text", new(){("MiscStat", MiscStat)})); }

public static string Sing_Log (string This)  { return(Translator.inst.Translate("Sing_Log", new(){("This", This)})); }

public static string Stab_Prey_Text (string Num)  { return(Translator.inst.Translate("Stab_Prey_Text", new(){("Num", Num)})); }

public static string Stab_Prey_Log (string This,string Target)  { return(Translator.inst.Translate("Stab_Prey_Log", new(){("This", This),("Target", Target)})); }

public static string Trapped_Text (string MiscStat)  { return(Translator.inst.Translate("Trapped_Text", new(){("MiscStat", MiscStat)})); }

public static string Trapped_Log (string This,string Target)  { return(Translator.inst.Translate("Trapped_Log", new(){("This", This),("Target", Target)})); }

public static string Silk_Wrap_Text (string PowerStat)  { return(Translator.inst.Translate("Silk_Wrap_Text", new(){("PowerStat", PowerStat)})); }

public static string Silk_Wrap_Log (string This,string Target)  { return(Translator.inst.Translate("Silk_Wrap_Log", new(){("This", This),("Target", Target)})); }

public static string Spin_Web_Text (string DefenseStat)  { return(Translator.inst.Translate("Spin_Web_Text", new(){("DefenseStat", DefenseStat)})); }

public static string Spin_Web_Log (string This,string Target)  { return(Translator.inst.Translate("Spin_Web_Log", new(){("This", This),("Target", Target)})); }

public static string Punishment_Text (string Num)  { return(Translator.inst.Translate("Punishment_Text", new(){("Num", Num)})); }

public static string Punishment_Log (string This,string Target)  { return(Translator.inst.Translate("Punishment_Log", new(){("This", This),("Target", Target)})); }

public static string Demand_Happiness_Log (string This)  { return(Translator.inst.Translate("Demand_Happiness_Log", new(){("This", This)})); }

public static string Demand_Anger_Log (string This)  { return(Translator.inst.Translate("Demand_Anger_Log", new(){("This", This)})); }

public static string Demand_Sadness_Log (string This)  { return(Translator.inst.Translate("Demand_Sadness_Log", new(){("This", This)})); }

public static string Mischief_Text (string Num)  { return(Translator.inst.Translate("Mischief_Text", new(){("Num", Num)})); }

public static string Mischief_Log (string This,string Target)  { return(Translator.inst.Translate("Mischief_Log", new(){("This", This),("Target", Target)})); }

public static string Delude_Text (string MiscStat)  { return(Translator.inst.Translate("Delude_Text", new(){("MiscStat", MiscStat)})); }

public static string Delude_Log (string This,string Target)  { return(Translator.inst.Translate("Delude_Log", new(){("This", This),("Target", Target)})); }

public static string Delay_Text (string MiscStat)  { return(Translator.inst.Translate("Delay_Text", new(){("MiscStat", MiscStat)})); }

public static string Delay_Log (string This,string Target)  { return(Translator.inst.Translate("Delay_Log", new(){("This", This),("Target", Target)})); }

public static string Distract_Text (string MiscStat)  { return(Translator.inst.Translate("Distract_Text", new(){("MiscStat", MiscStat)})); }

public static string Distract_Log (string This,string Target)  { return(Translator.inst.Translate("Distract_Log", new(){("This", This),("Target", Target)})); }

public static string Toughen_Text (string Num,string DefenseStat)  { return(Translator.inst.Translate("Toughen_Text", new(){("Num", Num),("DefenseStat", DefenseStat)})); }

public static string Toughen_Log (string This)  { return(Translator.inst.Translate("Toughen_Log", new(){("This", This)})); }

public static string In_the_Way_Text (string MiscStat)  { return(Translator.inst.Translate("In_the_Way_Text", new(){("MiscStat", MiscStat)})); }

public static string In_the_Way_Log (string This)  { return(Translator.inst.Translate("In_the_Way_Log", new(){("This", This)})); }

public static string Slash_Text (string Num)  { return(Translator.inst.Translate("Slash_Text", new(){("Num", Num)})); }

public static string Slash_Log (string This,string Target)  { return(Translator.inst.Translate("Slash_Log", new(){("This", This),("Target", Target)})); }

public static string Howl_Log (string This)  { return(Translator.inst.Translate("Howl_Log", new(){("This", This)})); }

public static string Nurture_Text (string MiscStat,string PowerStat,string DefenseStat)  { return(Translator.inst.Translate("Nurture_Text", new(){("MiscStat", MiscStat),("PowerStat", PowerStat),("DefenseStat", DefenseStat)})); }

public static string Nurture_Log (string This)  { return(Translator.inst.Translate("Nurture_Log", new(){("This", This)})); }

public static string Guard_Text (string MiscStat)  { return(Translator.inst.Translate("Guard_Text", new(){("MiscStat", MiscStat)})); }

public static string Guard_Log (string This,string Target)  { return(Translator.inst.Translate("Guard_Log", new(){("This", This),("Target", Target)})); }

public static string Counting_Down (string Num)  { return(Translator.inst.Translate("Counting_Down", new(){("Num", Num)})); }

public static string DoEnum(ToTranslate thing) {return(Translator.inst.Translate(thing.ToString()));}
}
public enum ToTranslate {
Title,Author_Credit,Last_Update,Language,Translator_Credit,Loading,Play_Game,Update_History,Story,Story_Text,Daily_Challenge,Month_1,Month_2,Month_3,Month_4,Month_5,Month_6,Month_7,Month_8,Month_9,Month_10,Month_11,Month_12,Cheats_and_Challenges,Cheat,New_Abilities,New_Abilities_Text,Knight_Reach,Knight_Reach_Text,Weaker_Enemies,Weaker_Enemies_Text,Slower_Enemy_Cooldowns,Slower_Enemy_Cooldowns_Text,Number_Cap,Number_Cap_Text,Challenge,No_Revives,No_Revives_Text,Ineffectives_Fail,Ineffectives_Fail_Text,Player_Timer,Player_Timer_Text,Extra_Enemy_Turns,Extra_Enemy_Turns_Text,More_Enemies,More_Enemies_Text,Clear_All,Apply_Number_Cap,Apply_Minimum,Failed,Timer,Gain_New_Abilities,Special_Thanks,Seth_Royston_Thanks,Liam_Cribbs_Thanks,Flan_Falacci_Thanks,Inspiration_Thanks,Playtest_Thursday_Thanks,Settings,Emotions_Guide,Close_Settings,Close,Random,Animation_Time,Confirm_Decisions,Confirm,Rechoose,Display_Tooltip,Cooldown,Cooldown_Text,Dead,Neutral,Neutral_Text,Sad,Sad_Text,Happy,Happy_Text,Angry,Angry_Text,Grounded,Grounded_Text,Elevated,Elevated_Text,Health,Health_Text,Max_Health,Power,Power_Text,Defense,Defense_Text,Protected,Protected_Text,Blocked,Locked,Locked_Text,Stunned,Stunned_Text,Targeted,Targeted_Text,Extra,Extra_Text,Star,Star_Text,Customize_Players,Customize_Description,Choose_Emotion,Next_in_Line,Title_Screen,Quit_Game,Quit_Fight,Game_Won,Game_Lost,Tutorial,Tutorial_10,Tutorial_20,Tutorial_30,Tutorial_40,Tutorial_50,Tutorial_51,Tutorial_52,Tutorial_53,Tutorial_54,Tutorial_60,Tutorial_70,Tutorial_71,Tutorial_72,Tutorial_80,Tutorial_81,Tutorial_82,Tutorial_83,Tutorial_90,Tutorial_91,Tutorial_100,Tutorial_110,Tutorial_120,Tutorial_121,Tutorial_130,Tutorial_140,Tutorial_150,Tutorial_151,Tutorial_Finished,Next,Encyclopedia,Exit,Abilities,Search_Abilities,Type_1,Type_2,Any,Attack,Healing,EmotionPlayer,EmotionEnemy,PositionPlayer,PositionEnemy,StatPlayer,StatEnemy,Player,Enemies,Search_Enemies,Right_Click_Reminder,Default_Position,Knight,Knight_Text,Battle_Cry,Cheer,Daze,Desperation,Embarass,Force,Impale,Intimidate,Intimidate_Text,Joust,Knock_Down,Meditate,Mirror,Mock,Neutralize,Redirect,Strike,Steal,Surprise,Twirl,Upset,Immobilize,Angel,Angel_Text,Air_Combat,Assist,Calm_Down,Crash_Land,Enrage,Exhaust,Exorcism,Gift_of_Flight,Gift_of_Flight_Text,Hide,Immunity,Joy,Lift_Up,Motivate,Overheal,Petrify,Plummet,Plummet_Text,Retreat,Retreat_Text,Security,Soft_Landing,Tailwinds,Team_Up,Wizard,Wizard_Text,Above_Danger,Bad_Omens,Bounce,Chill,Accelerate,Flood,Gust,Headwinds,Crown,Storm,Manipulate_Time,Bind,Punch_Up,Quick_Attack,Readjust,Readjust_Text,Restrain,Shockwave,Stalactites,Team_Attack,Touchdown,Warp,Revive,Skip_Turn,Skip_Turn_Text,Archer,Archer_Text,Quick_Shot,Bullseye,Volley,Focus,Barbarian,Barbarian_Text,Fury,Savagery,Brawl,Camp,Bat,Bat_Text,Drain,Screech,Hunting_Season,Bloodthirst,Bees,Bees_Text,Sting,Honey,Protect_the_Queen,Support,Cow,Cow_Text,Charge,Milk,Graze,Bellow,Crow,Crow_Text,Peck,Demon,Demon_Text,Dark_Energy,Betrayal,Ritual,Demand_Energy,Dragon,Dragon_Text,Firebreath,Roost,Take_Flight,Take_Flight_Text,Roar,Drone,Drone_Text,Sound_Waves,Alarm,Ghost,Ghost_Text,Jumpscare,Lament,Lament_Text,Possession,Release,Herald,Herald_Text,Flag_Whack,Announce,Kitty,Kitty_Text,Scratch,Stuck_in_a_Tree,Stuck_in_a_Tree_Text,Land_Feet_First,Lure,Leprechaun,Leprechaun_Text,Prank,Caught_at_a_Good_Time,Ninja,Ninja_Text,Assassinate,Shuriken,Into_Shadows,Wall_Climb,Page,Page_Text,Swing,Partier,Partier_Text,Fight,Share_Drinks,Share_Snacks,Drinking_Game,Drinking_Game_Text,Pixie,Pixie_Text,Will_o_Wisp,Cast_Happiness,Cast_Happiness_Text,Cast_Sadness,Cast_Sadness_Text,Cast_Anger,Cast_Anger_Text,Puppeteer,Puppeteer_Text,Collision,Drop,Dangle,Control,Puppy,Puppy_Text,Bite,Leap,Bark,Cuddle,Rats,Rats_Text,Swarm,Strength_in_Numbers,Strength_in_Numbers_Text,Royalty,Royalty_Text,Order,Promote,Call,Procession,Siren,Siren_Text,Drown,Shipwreck,Manipulate,Sing,Spider,Spider_Text,Stab_Prey,Trapped,Silk_Wrap,Spin_Web,Taskmaster,Taskmaster_Text,Punishment,Demand_Happiness,Demand_Happiness_Text,Demand_Anger,Demand_Anger_Text,Demand_Sadness,Demand_Sadness_Text,Trickster,Trickster_Text,Mischief,Delude,Delay,Distract,Wall,Wall_Text,Toughen,In_the_Way,Wolf,Wolf_Text,Slash,Howl,Howl_Text,Nurture,Guard,Update_0,Update_0_Text,Update_1,Update_1_Text,Update_2,Update_2_Text,Upload_Translation,Download_English,Blank,Update_3,Update_3_Text,Update_4,Update_4_Text
}
