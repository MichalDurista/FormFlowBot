using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;

namespace FormFlowBot.Dialogs
{
	[Serializable]
	public class ProductDialog : IDialog<object>
	{
		private IssueFormModel issueFormModel;
		private Dictionary<string, string[]> productsWithPlatforms;

		public ProductDialog(IssueFormModel issue)
		{
			issueFormModel = new IssueFormModel
			{
				Description = issue.Description
			};

			var platforms = new[] {"platform1, platform2, platform3"};
			productsWithPlatforms = new Dictionary<string, string[]>
			{
				{ "product1", platforms },
				{ "product2", platforms }
			};
		}

		public Task StartAsync(IDialogContext context)
		{
			var productForm = new FormDialog<IssueFormModel>(issueFormModel, BuildProductForm, FormOptions.PromptInStart);
			context.Call(productForm, AfterProductForm);
			return Task.CompletedTask;
		}

		private async Task AfterProductForm(IDialogContext context, IAwaitable<IssueFormModel> result)
		{
			// there is more logic of course
			try
			{
				var issue = await result;
				context.Done(true);
			}
			catch (FormCanceledException ex)
			{
				context.Done(false);
			}
		}

		private IForm<IssueFormModel> BuildProductForm()
		{
			return new FormBuilder<IssueFormModel>()
				.Field(new FieldReflector<IssueFormModel>(nameof(IssueFormModel.ProductName))
					.SetPrompt(new PromptAttribute("Type either product1 or product2"))
					.SetValidate(async (state, value) => await ValidateProductName(value, state)))
				.Field(new FieldReflector<IssueFormModel>(nameof(IssueFormModel.ProductPlatform))
					.SetType(typeof(string)) // when this is used  -> no platform options showed (buttons for user) as I tried to define in SetDefine, but Validation is OK
					//.SetType(null) // when this is used -> platform options are showed (although it is just one button but thats not the point now), but when typed wrong platform (eg. "wrongPlatform") it never goes to ValidateProductPlatform
					.SetDefine(async (issue, field) =>
					{
						if (issue.ProductName != null)
						{
							foreach (var platform in productsWithPlatforms[issue.ProductName])
							{
								field.AddDescription(platform, platform).AddTerms(platform, platform);
							}
						}

						return await Task.FromResult(true);
					})
					.SetPrompt(new PromptAttribute("Which platform of {ProductName}{||}?"))
					.SetAllowsMultiple(false)
					.SetValidate(async (state, value) => await ValidateProductPlatform(value, state)))
				.AddRemainingFields()
				.Confirm(
					prompt: "Is this your issue? {*}{||}")
				.Build();
		}

		private async Task<ValidateResult> ValidateProductPlatform(object value, IssueFormModel state)
		{
			// We never event get here if there is .SetType(typeof(null)) on line 62
			var isValidProduct = true; // some validation here is actually done, lets just assume it is a valid platform
			var validateResult = new ValidateResult
			{
				Value = value as string,
				IsValid = isValidProduct
			};

			// there are some further validations (checking if there were 3+ wrong attempts etc.. )

			return await Task.FromResult(validateResult);
		}

		private async Task<ValidateResult> ValidateProductName(object value, IssueFormModel state)
		{
			var isValidProduct = true; // some validation here is actually done, lets just assume it is a valid product
			var validateResult = new ValidateResult
			{
				Value = value as string,
				IsValid = isValidProduct
			};

			// there are some further validations (checking if there were 3+ wrong attempts etc.. )

			return await Task.FromResult(validateResult);
		}
	}
}