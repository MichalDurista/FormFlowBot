using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace FormFlowBot.Dialogs
{
	[Serializable]
	public class RootDialog : IDialog<object>
	{
		public Task StartAsync(IDialogContext context)
		{
			context.Call(new ProductDialog(new IssueFormModel
			{
				Description = "This is always predefined by the user."
			}), AfterProduct);
			//context.Wait(MessageReceivedAsync);

			return Task.CompletedTask;
		}

		private Task AfterProduct(IDialogContext context, IAwaitable<object> result)
		{
			context.Wait(MessageReceivedAsync);
			return Task.CompletedTask;
		}

		private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
		{
			var activity = await result as Activity;

			// calculate something for us to return
			int length = (activity.Text ?? string.Empty).Length;

			// return our reply to the user
			await context.PostAsync($"You sent {activity.Text} which was {length} characters");

			context.Wait(MessageReceivedAsync);
		}
	}
}