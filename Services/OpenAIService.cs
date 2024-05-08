using Azure.AI.OpenAI;
using Azure;

namespace AIContentComposer.Services
{
    public class OpenAIService
    {
        OpenAIClient? client;
        public void Initialize(string openAIKey, string? openAIEndpoint = null)
        {
            client = !string.IsNullOrWhiteSpace(openAIEndpoint)
                ? new OpenAIClient(
                    new Uri(openAIEndpoint),
                    new AzureKeyCredential(openAIKey))
                : new OpenAIClient(openAIKey);
        }

        internal async Task<string> CallOpenAIChat(string systemMessage, string userInput)
        {
            try
            {
                var options = new ChatCompletionsOptions()
                {
                    DeploymentName = "gpt-3.5-turbo", // Specify the gpt model here for non-Azure clients
                    Messages =
                    {
                        new ChatRequestSystemMessage("You are a helpful assistant." + systemMessage),
                        new ChatRequestUserMessage(userInput),
                    }
                };
                Response<ChatCompletions> response = await client.GetChatCompletionsAsync(options);
                return response.Value.Choices[0].Message.Content;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
