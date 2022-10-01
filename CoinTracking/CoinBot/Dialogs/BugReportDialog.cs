using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using CoinBot.Models;
using CoinBot.Services;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Dialogs
{
    public class BugReportDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;

        #endregion

        public BugReportDialog(string dialogId, StateService stateService) : base(dialogId)
        {
            _stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            // Create waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
                DescriptionStepAsync,
                CallbackTimeStepAsync,
                PhoneNumberStepAsync,
                BugStepAsync,
                SummaryStepAsync
            };

            //Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(BugReportDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(BugReportDialog)}.description"));
            AddDialog(new DateTimePrompt($"{nameof(BugReportDialog)}.callbackTime", CallbackTimeValidatorAsync));
            AddDialog(new TextPrompt($"{nameof(BugReportDialog)}.phoneNumber", PhoneNumberValidatorAsync));
            AddDialog(new ChoicePrompt($"{nameof(BugReportDialog)}.bug"));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(BugReportDialog)}.mainFlow";
        }

        #region Waterfall Steps
        private async Task<DialogTurnResult> DescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = (UserProfile)stepContext.Options;

            if (string.IsNullOrEmpty(userProfile.Description))
            {
                return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.description",
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Please describe the bug.")
                    }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(userProfile.Description, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> CallbackTimeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = (UserProfile)stepContext.Options;

            stepContext.Values["description"] = (string)stepContext.Result;

            if (userProfile.CallbackTime == null)
            {
                return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.callbackTime",
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("When would you like us to call you back?"),
                        RetryPrompt = MessageFactory.Text("The value entered must be between 9:00 AM and 5:00 PM.")
                    }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(userProfile.CallbackTime, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> PhoneNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = (UserProfile)stepContext.Options;

            if (stepContext.Result is DateTime time)
                stepContext.Values["callbackTime"] = time;
            else
            {
                var result = (List<DateTimeResolution>)stepContext.Result;
                stepContext.Values["callbackTime"] = result[0] != null ? Convert.ToDateTime(result[0].Value) : null;
            }

            if (string.IsNullOrEmpty(userProfile.PhoneNumber))
            {
                return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.phoneNumber",
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Please enter your phone number."),
                        RetryPrompt = MessageFactory.Text("The value entered must be a valid phone number.")
                    }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(userProfile.PhoneNumber, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> BugStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = (UserProfile)stepContext.Options;

            stepContext.Values["phoneNumber"] = (string)stepContext.Result;

            if (string.IsNullOrEmpty(userProfile.Bug))
            {
                return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.bug",
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What type of bug is this?"),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "Security", "Crash", "Power", "Performance", "Usability", "Serius Bug" })
                    }, cancellationToken); ;
            }

            return await stepContext.NextAsync(userProfile.Bug, cancellationToken);
        }

        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is string bug)
                stepContext.Values["bug"] = bug;
            else
            {
                var result = (FoundChoice)stepContext.Result;
                stepContext.Values["bug"] = result.Value;
            }

            //Get the current profile object from user state.
            var userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            //Save all of the data inside of the user profile.
            userProfile.Description = (string)stepContext.Values["description"];
            userProfile.CallbackTime = (DateTime)stepContext.Values["callbackTime"];
            userProfile.PhoneNumber = (string)stepContext.Values["phoneNumber"];
            userProfile.Bug = (string)stepContext.Values["bug"];

            //Show the summary to the user.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks for submitting a bug report. Here is a summary of what you submitted: {Environment.NewLine}Description: {userProfile.Description}{Environment.NewLine}Callback Time: {userProfile.CallbackTime}{Environment.NewLine}Phone Number: {userProfile.PhoneNumber}{Environment.NewLine}Bug: {userProfile.Bug}"), cancellationToken);

            //Save data in user state.
            await _stateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);

            //WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is the end.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        #endregion

        #region Validators
        private Task<bool> CallbackTimeValidatorAsync(PromptValidatorContext<IList<DateTimeResolution>> promptContext, CancellationToken cancellationToken)
        {
            var isValid = false;

            if (promptContext.Recognized.Succeeded)
            {
                // This value will be a TIMEX. And we are only interested in a Time so grab the first result and drop the Date part.
                // TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
                var timex = promptContext.Recognized.Value[0].Value;

                // Get the current time, we want to construct a DateTime object from what the user entered
                // but we don't know the year/month/day. So we will use the current year/month/day.
                var now = DateTime.Now;

                // Create a DateTime object from what the user entered.
                var dateTime = DateTime.Parse(timex);

                // If the user entered a time that is between 9:00 AM and 5:00 PM then we will consider it valid.
                if (dateTime.Hour >= 9 && dateTime.Hour <= 17)
                {
                    isValid = true;
                }
            }

            return Task.FromResult(isValid);
        }

        private Task<bool> PhoneNumberValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var isValid = false;

            if (promptContext.Recognized.Succeeded)
            {
                // Check if the user entered a valid phone number.
                if (Regex.Match(promptContext.Recognized.Value, @"^(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$").Success)
                {
                    isValid = true;
                }
            }

            return Task.FromResult(isValid);
        }
        #endregion
    }
}