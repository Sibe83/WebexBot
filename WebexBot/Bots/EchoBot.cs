// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using WebexBot.Infrastructure;
using WebexBot.Models;

namespace WebexBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        AdoDbAccessor adoDbAccessor = new AdoDbAccessor();
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await SendWelcomeMessageAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string message = "";


            if (turnContext.Activity.Attachments != null)
            {
                var activity = MessageFactory.Text($" I got {turnContext.Activity.Attachments.Count} attachments");
                foreach (var attachment in turnContext.Activity.Attachments)
                {
                    var image = new Attachment(
                        "image/png",
                        "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU");

                    activity.Attachments.Add(image);
                }

                await turnContext.SendActivityAsync(activity, cancellationToken);
            }
            else
            {
                string action = turnContext.Activity.Text;
                string clientID = turnContext.Activity.From.Id;
                adoDbAccessor.SaveRequest(clientID, action);

                if (adoDbAccessor.ClientExists(clientID))
                {
                    int actionID;
                    if (int.TryParse(action, out actionID))
                    {
                        if (adoDbAccessor.ClientExists(clientID, actionID)) {
                            message = "Action " + actionID;
                        }
                        else
                        {
                            message = "You are not enabled for this action. You are enabled for this actions: " + "\n";
                            List<Actions> actionsList = adoDbAccessor.GetActionsByClientID(clientID);
                            actionsList.ForEach(action => message += action.ID + " - " + action.Description + "\n");
                        }
                    }
                    else
                    {
                        message = "Not Valid ID";
                    }
                }

                else
                {
                    message = "Your Client dosn't exist in our database";
                }
                var activity = turnContext.Activity.Text == "cards" ? MessageFactory.Attachment(CreateAdaptiveCardAttachment(Directory.GetCurrentDirectory() + @"/Resources/adaptive_card.json")) : MessageFactory.Text(message);

                await turnContext.SendActivityAsync(activity, cancellationToken);
            }
        }

        protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Value != null)
            {
                var inputs = (Dictionary<string, string>)turnContext.Activity.Value;
                var name = inputs["Name"];

                var activity = MessageFactory.Text($"How are you doing {name}?");
                await turnContext.SendActivityAsync(activity, cancellationToken);
            }
        }

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                }
            }
        }

        private static Attachment CreateAdaptiveCardAttachment(string filePath)
        {
            var adaptiveCardJson = File.ReadAllText(filePath);
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }
    }
}
