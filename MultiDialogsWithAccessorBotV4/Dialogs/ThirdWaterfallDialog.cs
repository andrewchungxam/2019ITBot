using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using SimplifiedWaterfallDialogBotV4.BotAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class ThirdWaterfallDialog : WaterfallDialog
    {
        public static string DialogId { get; } = "thirdWaterfallDialog";
        public static WaterfallDialog BotInstance { get; } = new ThirdWaterfallDialog(DialogId, null);

        public ThirdWaterfallDialog(string dialogId, IEnumerable<WaterfallStep> steps)
            : base(dialogId, steps)
        {
            AddStep(FirstStepAsync);
            AddStep(NameStepAsync);
            AddStep(NameConfirmStepAsync);
        }

        /// <summary>
        /// Creates a <see cref="VideoCard"/>.
        /// </summary>
        /// <returns>A <see cref="VideoCard"/> the user can view and/or interact with.</returns>
        /// <remarks> Related types <see cref="CardAction"/>,
        /// <see cref="ActionTypes"/>, <see cref="MediaUrl"/>, and <see cref="ThumbnailUrl"/>.</remarks>
        private static VideoCard GetVideoCard()
        {
            var videoCard = new VideoCard
            {
                Title = "Big Buck Bunny",
                Subtitle = "by the Blender Institute",
                Text = "Big Buck Bunny (code-named Peach) is a short computer-animated comedy film by the Blender Institute," +
                       " part of the Blender Foundation. Like the foundation's previous film Elephants Dream," +
                       " the film was made using Blender, a free software application for animation made by the same foundation." +
                       " It was released as an open-source film under Creative Commons License Attribution 3.0.",
                Image = new ThumbnailUrl
                {
                    Url = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Big_buck_bunny_poster_big.jpg/220px-Big_buck_bunny_poster_big.jpg",
                },
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4",
                    },
                },
                Buttons = new List<CardAction>
                {
                    new CardAction()
                    {
                        Title = "Learn More",
                        Type = ActionTypes.OpenUrl,
                        Value = "https://peach.blender.org/",
                    },
                },
            };

            return videoCard;
        }

        private static async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var welcomeUserState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).WelcomeUserState.GetAsync(stepContext.Context);
            if (welcomeUserState.DidSeeVideo == false)
            {
                welcomeUserState.DidSeeVideo = true;

                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
                // Running a prompt here means the next WaterfallStep will be run when the users response is received.
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"THIRD WATERFALL STEP 1: This is the first step.  You can put your code in each of these steps."), cancellationToken);

                var reply = stepContext.Context.Activity.CreateReply();
                reply.Attachments = new List<Attachment>();
                reply.Attachments.Add(GetVideoCard().ToAttachment());
                // Send the card(s) to the user as an attachment to the activity
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                await Task.Delay(3000);
            }

            return await stepContext.NextAsync("Data from First Step", cancellationToken);

        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //string stringFromFirstStep = (string)stepContext.Result;
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"THIRD WATERFALL STEP 2: You can pass objects/strings step-to-step like this: {stringFromFirstStep}"), cancellationToken);
            //return await stepContext.PromptAsync("thirdWaterName", new PromptOptions { Prompt = MessageFactory.Text("What is your favorite color?") }, cancellationToken);

            //! DO NOT CHANGE THE NAME OF THIS DIALOG -- thirdWaterName -- iBOT CHECKS AND ALLOWS QNA ANSWERS THROUGH VIA THIS DIALOG
            return await stepContext.PromptAsync("thirdWaterName", new PromptOptions { Prompt = MessageFactory.Text("What questions can I answer about the royals?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"COLOR WATERFALL STEP 3: I like the color {stepContext.Result} too!"), cancellationToken);
            //END-WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            //QNA - COMMENTING OUT
            //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            //var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
            //botState.Color = stepContext.Result.ToString();
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"THIRD WATERFALL STEP 3: I like {botState.Color} {botState.Food} as well! "), cancellationToken);
            //END-WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            //return await stepContext.PromptAsync("thirdWaterName", new PromptOptions { Prompt = MessageFactory.Text("What questions can I answer about the royals?") }, cancellationToken);

            //return await stepContext.ReplaceDialogAsync(new TextPrompt(, cancellationToken);

            return await stepContext.ReplaceDialogAsync(ThirdWaterfallDialog.DialogId, false, cancellationToken);

            //QNA - COMMENTING OUT

            //QNA - METHOD 1
            //var qnaSavedAnswer =  (SAVE IN TURN STATE IN BOT)

            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"QnA value: {qnaSavedAnswer}."), cancellationToken);

            //return await stepContext.EndDialogAsync(null, cancellationToken);
            //QNA - METHOD 2

            //QNA


            //return await stepContext.EndDialogAsync(null, cancellationToken);
            //QNA





        }

    }
}
