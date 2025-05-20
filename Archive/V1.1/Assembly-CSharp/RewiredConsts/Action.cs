using System;
using Rewired.Dev;

namespace RewiredConsts
{
	// Token: 0x020001FE RID: 510
	public static class Action
	{
		// Token: 0x04000EE1 RID: 3809
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move Forward")]
		public const int MoveForward = 0;

		// Token: 0x04000EE2 RID: 3810
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move Backward")]
		public const int MoveBackward = 1;

		// Token: 0x04000EE3 RID: 3811
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move Left")]
		public const int MoveLeft = 2;

		// Token: 0x04000EE4 RID: 3812
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move Right")]
		public const int MoveRight = 3;

		// Token: 0x04000EE5 RID: 3813
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Jump")]
		public const int Jump = 9;

		// Token: 0x04000EE6 RID: 3814
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Combat Stance")]
		public const int Weapon = 10;

		// Token: 0x04000EE7 RID: 3815
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Rest Stance")]
		public const int Sit = 11;

		// Token: 0x04000EE8 RID: 3816
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Torch Stance")]
		public const int Torch = 12;

		// Token: 0x04000EE9 RID: 3817
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Swap Primary Secondary")]
		public const int SwapMainHand = 13;

		// Token: 0x04000EEA RID: 3818
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Swap Off-Hand")]
		public const int SwapOffHand = 14;

		// Token: 0x04000EEB RID: 3819
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Bag")]
		public const int Bag = 15;

		// Token: 0x04000EEC RID: 3820
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Gathering Bag")]
		public const int Gathering = 96;

		// Token: 0x04000EED RID: 3821
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Inventory Equipment")]
		public const int Inventory = 17;

		// Token: 0x04000EEE RID: 3822
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Skills")]
		public const int Journal = 16;

		// Token: 0x04000EEF RID: 3823
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Log")]
		public const int QuestLog = 97;

		// Token: 0x04000EF0 RID: 3824
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Crafting Recipes")]
		public const int CraftingRecipes = 64;

		// Token: 0x04000EF1 RID: 3825
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Social")]
		public const int Social = 71;

		// Token: 0x04000EF2 RID: 3826
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Map")]
		public const int Map = 67;

		// Token: 0x04000EF3 RID: 3827
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Time Window")]
		public const int TimeWindow = 112;

		// Token: 0x04000EF4 RID: 3828
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Toggle All Selected")]
		public const int Toggle_All = 114;

		// Token: 0x04000EF5 RID: 3829
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Crouch")]
		public const int Crouch = 18;

		// Token: 0x04000EF6 RID: 3830
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Toggle Walk")]
		public const int WalkToggle = 68;

		// Token: 0x04000EF7 RID: 3831
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Consider")]
		public const int Consider = 104;

		// Token: 0x04000EF8 RID: 3832
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Next Offensive Target")]
		public const int NextOffensiveTarget = 19;

		// Token: 0x04000EF9 RID: 3833
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Previous Offensive Target")]
		public const int PrevOffensiveTarget = 58;

		// Token: 0x04000EFA RID: 3834
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Next Defensive Target")]
		public const int NextDefensiveTarget = 65;

		// Token: 0x04000EFB RID: 3835
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Previous Defensive Target")]
		public const int PrevDefensiveTarget = 66;

		// Token: 0x04000EFC RID: 3836
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Auto-Attack")]
		public const int AutoAttack = 20;

		// Token: 0x04000EFD RID: 3837
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Cancel Execution")]
		public const int CancelExecution = 103;

		// Token: 0x04000EFE RID: 3838
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Assist Defensive Target")]
		public const int DefensiveAssist = 69;

		// Token: 0x04000EFF RID: 3839
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Assist Offensive Target")]
		public const int OffensiveAssist = 70;

		// Token: 0x04000F00 RID: 3840
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Drag Bag")]
		public const int DragBag = 113;

		// Token: 0x04000F01 RID: 3841
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Auto-Run")]
		public const int AutoRun = 21;

		// Token: 0x04000F02 RID: 3842
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Action Bar 1")]
		public const int ActionBar1 = 22;

		// Token: 0x04000F03 RID: 3843
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Action Bar 2")]
		public const int ActionBar2 = 23;

		// Token: 0x04000F04 RID: 3844
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Action Bar 3")]
		public const int ActionBar3 = 24;

		// Token: 0x04000F05 RID: 3845
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Action Bar 4")]
		public const int ActionBar4 = 25;

		// Token: 0x04000F06 RID: 3846
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Action Bar 5")]
		public const int ActionBar5 = 26;

		// Token: 0x04000F07 RID: 3847
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Action Bar 6")]
		public const int ActionBar6 = 27;

		// Token: 0x04000F08 RID: 3848
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Action Bar 7")]
		public const int ActionBar7 = 28;

		// Token: 0x04000F09 RID: 3849
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Action Bar 8")]
		public const int ActionBar8 = 29;

		// Token: 0x04000F0A RID: 3850
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Action Bar 9")]
		public const int ActionBar9 = 30;

		// Token: 0x04000F0B RID: 3851
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Action Bar 0")]
		public const int ActionBar0 = 31;

		// Token: 0x04000F0C RID: 3852
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Consumable 1")]
		public const int Consumable1 = 88;

		// Token: 0x04000F0D RID: 3853
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Consumable 2")]
		public const int Consumable2 = 89;

		// Token: 0x04000F0E RID: 3854
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Consumable 3")]
		public const int Consumable3 = 90;

		// Token: 0x04000F0F RID: 3855
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Consumable 4")]
		public const int Consumable4 = 91;

		// Token: 0x04000F10 RID: 3856
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Reagent 1")]
		public const int Reagent1 = 92;

		// Token: 0x04000F11 RID: 3857
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Reagent 2")]
		public const int Reagent2 = 93;

		// Token: 0x04000F12 RID: 3858
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Reagent 3")]
		public const int Reagent3 = 94;

		// Token: 0x04000F13 RID: 3859
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Reagent 4")]
		public const int Reagent4 = 95;

		// Token: 0x04000F14 RID: 3860
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Macro 1")]
		public const int Macro0 = 117;

		// Token: 0x04000F15 RID: 3861
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Macro 2")]
		public const int Macro1 = 118;

		// Token: 0x04000F16 RID: 3862
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Macro 3")]
		public const int Macro2 = 119;

		// Token: 0x04000F17 RID: 3863
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Macro 4")]
		public const int Macro3 = 120;

		// Token: 0x04000F18 RID: 3864
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Macro 5")]
		public const int Macro4 = 121;

		// Token: 0x04000F19 RID: 3865
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Select Self")]
		public const int SelectSelf = 32;

		// Token: 0x04000F1A RID: 3866
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Select Group Member 1")]
		public const int SelectGroup1 = 33;

		// Token: 0x04000F1B RID: 3867
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Select Group Member 2")]
		public const int SelectGroup2 = 34;

		// Token: 0x04000F1C RID: 3868
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Select Group Member 3")]
		public const int SelectGroup3 = 35;

		// Token: 0x04000F1D RID: 3869
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Select Group Member 4")]
		public const int SelectGroup4 = 36;

		// Token: 0x04000F1E RID: 3870
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Select Group Member 5")]
		public const int SelectGroup5 = 37;

		// Token: 0x04000F1F RID: 3871
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Select Group Member 6")]
		public const int SelectGroup6 = 38;

		// Token: 0x04000F20 RID: 3872
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Select Group Member 7")]
		public const int SelectGroup7 = 39;

		// Token: 0x04000F21 RID: 3873
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Interact with the nearest loot")]
		public const int OpenLoot = 109;

		// Token: 0x04000F22 RID: 3874
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Loot Roll Need")]
		public const int Need = 100;

		// Token: 0x04000F23 RID: 3875
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Loot Roll Greed")]
		public const int Greed = 101;

		// Token: 0x04000F24 RID: 3876
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Loot Roll Pass")]
		public const int Pass = 102;

		// Token: 0x04000F25 RID: 3877
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Toggle Nameplates")]
		public const int Nameplates = 40;

		// Token: 0x04000F26 RID: 3878
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Toggle Compass")]
		public const int Compass = 122;

		// Token: 0x04000F27 RID: 3879
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Toggle UI")]
		public const int ToggleUI = 41;

		// Token: 0x04000F28 RID: 3880
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move UI")]
		public const int MoveUI = 115;

		// Token: 0x04000F29 RID: 3881
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Capture Screenshot")]
		public const int Screenshot = 42;

		// Token: 0x04000F2A RID: 3882
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Reply")]
		public const int Tell = 43;

		// Token: 0x04000F2B RID: 3883
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Hail")]
		public const int Hail = 44;

		// Token: 0x04000F2C RID: 3884
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Hail (Invert Priority)")]
		public const int HailInvert = 116;

		// Token: 0x04000F2D RID: 3885
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Turn Left")]
		public const int TurnLeft = 51;

		// Token: 0x04000F2E RID: 3886
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Turn Right")]
		public const int TurnRight = 52;

		// Token: 0x04000F2F RID: 3887
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Look Up")]
		public const int LookUp = 53;

		// Token: 0x04000F30 RID: 3888
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Look Down")]
		public const int LookDown = 54;

		// Token: 0x04000F31 RID: 3889
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Look Left")]
		public const int LookLeft = 55;

		// Token: 0x04000F32 RID: 3890
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Look Right")]
		public const int LookRight = 56;

		// Token: 0x04000F33 RID: 3891
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Zoom In")]
		public const int ZoomIn = 98;

		// Token: 0x04000F34 RID: 3892
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Zoom Out")]
		public const int ZoomOut = 99;

		// Token: 0x04000F35 RID: 3893
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Over the Shoulder Camera")]
		public const int CameraPOV = 61;

		// Token: 0x04000F36 RID: 3894
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "First Person Camera")]
		public const int CameraFirstPerson = 105;

		// Token: 0x04000F37 RID: 3895
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Select Alchemy I")]
		public const int AlchemyI = 106;

		// Token: 0x04000F38 RID: 3896
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Select Alchemy II")]
		public const int AlchemyII = 107;

		// Token: 0x04000F39 RID: 3897
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Cycle Alchemy Selection")]
		public const int AlchemyCycle = 108;

		// Token: 0x04000F3A RID: 3898
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Activate Chat")]
		public const int ActivateChat = 110;

		// Token: 0x04000F3B RID: 3899
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Activate Chat with Slash")]
		public const int ActivateChatWithSlash = 111;

		// Token: 0x04000F3C RID: 3900
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Move Forward Back")]
		public const int MoveNonLateral = 45;

		// Token: 0x04000F3D RID: 3901
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Move Left Right")]
		public const int MoveLateral = 46;

		// Token: 0x04000F3E RID: 3902
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Look Up Down")]
		public const int LookVertical = 47;

		// Token: 0x04000F3F RID: 3903
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Look Left Right")]
		public const int LookHorizontal = 48;

		// Token: 0x04000F40 RID: 3904
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Camera Vertical")]
		public const int CameraVertical = 73;

		// Token: 0x04000F41 RID: 3905
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Camera Zoom")]
		public const int CameraZoom = 49;

		// Token: 0x04000F42 RID: 3906
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Camera Roll")]
		public const int CameraRoll = 74;

		// Token: 0x04000F43 RID: 3907
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Camera FOV")]
		public const int CameraFOV = 84;

		// Token: 0x04000F44 RID: 3908
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Reset Camera FOV")]
		public const int ResetFOV = 87;

		// Token: 0x04000F45 RID: 3909
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Change Camera Mode")]
		public const int ChangeCameraMode = 75;

		// Token: 0x04000F46 RID: 3910
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Attach Detach Orbit")]
		public const int ToggleOrbit = 86;

		// Token: 0x04000F47 RID: 3911
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Fast Camera")]
		public const int FastCamera = 76;

		// Token: 0x04000F48 RID: 3912
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Camera Speed Up")]
		public const int CameraSpeedUp = 78;

		// Token: 0x04000F49 RID: 3913
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Camera Speed Down")]
		public const int CameraSpeedDown = 79;

		// Token: 0x04000F4A RID: 3914
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Swap Between Camera Roll and FOV Control")]
		public const int SwapRollFov = 83;

		// Token: 0x04000F4B RID: 3915
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Swap Trigger and Bumper Functionality")]
		public const int SwapTriggersBumpers = 82;

		// Token: 0x04000F4C RID: 3916
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Pause")]
		public const int Pause = 77;

		// Token: 0x04000F4D RID: 3917
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Escape")]
		public const int Escape = 50;

		// Token: 0x04000F4E RID: 3918
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Look")]
		public const int ActivateLook_Head = 62;

		// Token: 0x04000F4F RID: 3919
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Look + Turn")]
		public const int ActivateLook_Body = 63;

		// Token: 0x04000F50 RID: 3920
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Toggle Controller Turning")]
		public const int ToggleControllerTurning = 72;

		// Token: 0x04000F51 RID: 3921
		[ActionIdFieldInfo(categoryName = "Unbindable", friendlyName = "Toggle Marketing Camera")]
		public const int ToggleSlew = 85;

		// Token: 0x04000F52 RID: 3922
		[ActionIdFieldInfo(categoryName = "GM", friendlyName = "Console")]
		public const int Console = 60;
	}
}
