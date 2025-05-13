using System;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;

namespace SoL.Game.UI.Skills
{
	// Token: 0x0200092D RID: 2349
	internal static class ForgetHelper
	{
		// Token: 0x06004515 RID: 17685 RVA: 0x0019E798 File Offset: 0x0019C998
		internal static bool CanForgetSomething()
		{
			return LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.Vitals != null && LocalPlayer.GameEntity.SkillsController != null && LocalPlayer.GameEntity.Vitals.Stance.CanForgetMastery() && !LocalPlayer.GameEntity.SkillsController.PendingIsActive;
		}

		// Token: 0x06004516 RID: 17686 RVA: 0x0019E800 File Offset: 0x0019CA00
		internal static bool CanForgetSpecialization(ArchetypeInstance baseRoleInstance)
		{
			return baseRoleInstance != null && baseRoleInstance.Mastery != null && baseRoleInstance.MasteryData != null && baseRoleInstance.MasteryData.Specialization != null;
		}

		// Token: 0x06004517 RID: 17687 RVA: 0x0019E83C File Offset: 0x0019CA3C
		internal static bool CanForgetWithError()
		{
			if (!LocalPlayer.GameEntity.Vitals.Stance.CanForgetMastery())
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Invalid stance to forget specialization!");
				return false;
			}
			if (LocalPlayer.GameEntity.SkillsController.PendingIsActive)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Cannot forget while executing!");
				return false;
			}
			if (!ForgetHelper.CanForgetSomething())
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Cannot forget with unknown error!");
				return false;
			}
			return true;
		}

		// Token: 0x06004518 RID: 17688 RVA: 0x0019E8B4 File Offset: 0x0019CAB4
		internal static void ForgetSpecializationConfirmation(SpecializedRole specializedRole, Action<bool, object> responseCallback)
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Forget Specialization",
				Text = "Are you sure you want to forget " + specializedRole.DisplayName + "? You will lose <b><i>all specialization progress</i></b> you have made and <b><i>all specialization abilities</i></b>!",
				Callback = responseCallback,
				ConfirmationText = "Yes",
				CancelText = "NO"
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}
	}
}
