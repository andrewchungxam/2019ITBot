// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using MultiDialogsWithAccessorBotV4.BotAccessor;
using SimplifiedWaterfallDialogBotV4;
using SimplifiedWaterfallDialogBotV4.BotAccessor;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class MultiDialogWithAccessorBot : IBot
    {
        private readonly DialogSet _dialogSet;
        private readonly DialogBotConversationStateAndUserStateAccessor _dialogBotConversationStateAndUserStateAccessor;

        public DialogBotConversationStateAndUserStateAccessor DialogBotConversationStateAndUserStateAccessor { get; set; }
        
   
        public static readonly string QnAMakerKey = "RoyaltyInfo2018";
        private readonly BotServices _services;

        public MultiDialogWithAccessorBot(DialogBotConversationStateAndUserStateAccessor accessor, BotServices services)
        {
            _dialogBotConversationStateAndUserStateAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _dialogSet = new DialogSet(_dialogBotConversationStateAndUserStateAccessor.ConversationDialogState);
            _dialogSet.Add(RootWaterfallDialog.BotInstance);

            _dialogSet.Add(new TextPrompt("name"));
            _dialogSet.Add(new TextPrompt("colorName"));
            _dialogSet.Add(new TextPrompt("foodName"));
            _dialogSet.Add(new TextPrompt("thirdWaterName"));

            _dialogSet.Add(FoodWaterfallDialog.BotInstance);
            _dialogSet.Add(ColorWaterfallDialog.BotInstance);
            _dialogSet.Add(NameWaterfallDialog.BotInstance);

            _dialogSet.Add(new ChoicePrompt("dialogChoice"));
            _dialogSet.Add(ThirdWaterfallDialog.BotInstance);

            _dialogSet.Add(new ChoicePrompt("confirmHero1"));
            //_dialogSet.Add(new ConfirmPrompt("confirmHero1"));
            
            DialogBotConversationStateAndUserStateAccessor = accessor;

            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            if (!_services.QnAServices.ContainsKey(QnAMakerKey))
            {
                throw new System.ArgumentException($"Invalid configuration. Please check your '.bot' file for a QnA service named '{QnAMakerKey}'.");
            }

        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botState = await DialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(turnContext, () => new UserProfile(), cancellationToken);

            //qna
            var myWelcomeUserState = await DialogBotConversationStateAndUserStateAccessor.WelcomeUserState.GetAsync(turnContext, () => new WelcomeUserState(), cancellationToken);
            //qna

            turnContext.TurnState.Add("DialogBotConversationStateAndUserStateAccessor", DialogBotConversationStateAndUserStateAccessor);

            //var myWelcomeUserState = await DialogBotConversationStateAndUserStateAccessor.WelcomeUserState.GetAsync(turnContext, () => new WelcomeUserState(), cancellationToken);
            //turnContext.TurnState.Add("DialogBotConversationStateAndUserStateAccessorMyWelcomeUserState", DialogBotConversationStateAndUserStateAccessor);
            //turnContext.TurnState.Add("DialogBotConversationStateAndUserStateAccessor", DialogBotConversationStateAndUserStateAccessor);


            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {


                ////QNA
                //// Check QnA Maker model
                //var response = await _services.QnAServices[QnAMakerKey].GetAnswersAsync(turnContext);
                //if (response != null && response.Length > 0)
                //{
                //    await turnContext.SendActivityAsync(response[0].Answer, cancellationToken: cancellationToken);
                //}
                ////QNA


                // Run the DialogSet - let the framework identify the current state of the dialog from
                // the dialog stack and figure out what (if any) is the active dialog.
                var dialogContext = await _dialogSet.CreateContextAsync(turnContext, cancellationToken);

                if (dialogContext != null)
                { 
                    if (dialogContext.ActiveDialog != null)
                    {
                        if (dialogContext.ActiveDialog.Id == "thirdWaterName")
                        {
                            var response = await _services.QnAServices[QnAMakerKey].GetAnswersAsync(turnContext);
                            if (response != null && response.Length > 0)
                            {
                                await turnContext.SendActivityAsync(response[0].Answer, cancellationToken: cancellationToken);
                            }
                        }
                    }
                }



                //POP OFF ANY DIALOG IF THE "FLAG IS SWITCHED" 
                string didTypeNamestring = "";
                if (turnContext.TurnState.ContainsKey("didTypeName"))
                {
                    didTypeNamestring = turnContext.TurnState["didTypeName"] as string;
                }

                if (didTypeNamestring == "name")
                {

                    //OPTION 1:
                    await dialogContext.CancelAllDialogsAsync();

                    //OPTION 2: //TRY BELOW OPTIONS - WHY DOES IT MISBEHAVE?
                    //NOTE-CALLING THIS HITS THE CONTINUE IN THE BELOW IF STATEMENT
                    //await dialogContext.ReplaceDialogAsync(NameWaterfallDialog.DialogId, null, cancellationToken);

                    //OPTION 3:
                    //DOES NOT WORK WELL HERE - WHEN HAVE YOU SEEN IT WORK CORRECTLY IN PREVIOUS PROJECTS?
                    //await dialogContext.EndDialogAsync();
                }
                
                if (dialogContext.ActiveDialog == null)
                {
                    if (turnContext.TurnState.ContainsKey("didTypeName"))
                    {
                        didTypeNamestring = turnContext.TurnState["didTypeName"] as string;
                        if (didTypeNamestring == "name")
                        {
                            await dialogContext.BeginDialogAsync(NameWaterfallDialog.DialogId, null, cancellationToken);
                        }
                    }
                    else
                    {
                        await dialogContext.BeginDialogAsync(RootWaterfallDialog.DialogId, null, cancellationToken);
                    }
                }
                else
                {
                    await dialogContext.ContinueDialogAsync(cancellationToken);
                }

                await _dialogBotConversationStateAndUserStateAccessor.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
                await _dialogBotConversationStateAndUserStateAccessor.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            }
        }
    }
}