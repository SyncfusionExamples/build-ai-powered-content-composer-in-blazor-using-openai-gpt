using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.DocumentEditor;
using Syncfusion.Blazor.DropDowns;

namespace AIContentComposer.Pages
{
        public partial class Home
        {
            SfDocumentEditorContainer container;

            private string inputValue = "";
            private bool isVisible = false;
            private string previewContent = "";
            private string docSelectedText = "";

            private bool isContentLoaded = false;
            private bool isRephraseContentLoaded = false;

            private string[] toneSelected = ["Professional"];
            private string[] formatSelected = ["Paragraph"];
            private string[] lengthSelected = ["Medium"];

            private string rephraseToneValue = "";
            private string rephraseFormatValue = "";
            private string rephraseLengthValue = "";

            private string currentTone = "";
            private string currentFormat = "";
            private string currentLength = "";

            private string generatedText = "";
            private bool isInlineDialogVisible = false;
            private string rephrasedText = "";

            public class ComposeContent
            {
                public required string ID { get; set; }
                public required string Text { get; set; }
            }

            private readonly List<ComposeContent> toneData = [
                new() { ID= "Tone1", Text= "Professional" },
                new() { ID= "Tone2", Text= "Casual" },
                new() { ID= "Tone3", Text= "Enthusiastic" },
                new() { ID= "Tone4", Text= "Informational" },
                new() { ID= "Tone5", Text= "Funny" },
                new() { ID= "Tone6", Text= "Succint" }
            ];

            private readonly List<ComposeContent> formatData =
            [
                new() { ID= "Format1", Text= "Paragraph" },
                new() { ID= "Format2", Text= "Email" },
                new() { ID= "Format3", Text= "Bullet Points" },
                new() { ID= "Format4", Text= "LinkedIn Post" },
                new() { ID= "Format5", Text= "Report" }
            ];

            private readonly List<ComposeContent> lengthData =
            [
                new() { ID= "Length1", Text= "Short" },
                new() { ID= "Length2", Text= "Medium" },
                new() { ID= "Length3", Text= "Long" }
            ];
            public void OnCreated(object args)
            {
                SfDocumentEditor documentEditor = container.DocumentEditor;
                List<MenuItemModel> contextMenuItem =
                [
                    new() { Text="Rephrase with AI", Id= "rephrase", IconCss="e-icons e-annotation-edit" },
                    new() { Text="Summarize with AI", Id= "summary", IconCss="e-icons e-signature" },
                ];
                documentEditor.ContextMenu.AddCustomMenu(contextMenuItem, true, false);
            }

            public void OnToneSelect(ChipEventArgs args)
            {
                currentTone = args.Text;
                toneSelected = [args.Text];
            }

            public void OnFormatSelect(ChipEventArgs args)
            {
                currentFormat = args.Text;
                formatSelected = [args.Text];
            }

            public void OnLengthSelect(ChipEventArgs args)
            {
                currentLength = args.Text;
                lengthSelected = [args.Text];
            }

            public void OnToneChange(ChangeEventArgs<string, ComposeContent> args)
            {
                rephraseToneValue = args.ItemData.Text;
            }
            public void OnFormatChange(ChangeEventArgs<string, ComposeContent> args)
            {
                rephraseFormatValue = args.ItemData.Text;
            }
            public void OnLengthChange(ChangeEventArgs<string, ComposeContent> args)
            {
                rephraseLengthValue = args.ItemData.Text;
            }

            public async Task OnReplaceClick()
            {
                isInlineDialogVisible = false;
                await container.DocumentEditor.Editor.InsertTextAsync(rephrasedText);
            }

            public async Task OnRewriteClick()
            {
                isRephraseContentLoaded = true;
                if (!String.IsNullOrEmpty(rephraseToneValue) && !String.IsNullOrEmpty(rephraseFormatValue) && !String.IsNullOrEmpty(rephraseLengthValue))
                {
                    string sysMsg = $"Rephrase the given content with the following specifications: - Tone: {rephraseToneValue}, Format: {rephraseFormatValue}, Sentence Length: {rephraseLengthValue}. Please ensure the content is engaging and adheres to the specified tone, format, and sentence length.";
                    rephrasedText = await openAIService.CallOpenAIChat(sysMsg, docSelectedText);
                }
                isRephraseContentLoaded = false;
            }

            public async Task OnGenerateDraftClick()
            {
                isContentLoaded = true;
                var sysMsg = $"Create content for the provided topic with the following specifications: - Tone: {currentTone}, Format: {currentFormat}, Sentence Length: {currentLength}. Please ensure the content is engaging and adheres to the specified tone, format, and sentence length.";
                previewContent = await openAIService.CallOpenAIChat(sysMsg, inputValue);
                isContentLoaded = false;
            }

            public async Task OnAddDocumentClick()
            {
                await container.DocumentEditor.Editor.InsertTextAsync(previewContent);
            }

            private void CloseDialog()
            {
                isVisible = false;
            }

            private void OnDialogClose()
            {
                isInlineDialogVisible = false;
            }

            public async Task OnContextMenuSelect(CustomContentMenuEventArgs args)
            {
                string message;
                docSelectedText = await container.DocumentEditor.Selection.GetTextAsync();
                string prompt;
                if (args.Id.EndsWith("rephrase"))
                {
                    isInlineDialogVisible = true;
                    prompt = "Rephrase the provided content.";
                    rephrasedText = await openAIService.CallOpenAIChat(prompt, docSelectedText);
                }
                else if (args.Id.EndsWith("summary"))
                {
                    isVisible = true;
                    await container.DocumentEditor.Selection.SelectAllAsync();
                    message = await container.DocumentEditor.Selection.GetTextAsync();
                    prompt = "Summarize the content you are provided with in a single line.";
                    generatedText = await openAIService.CallOpenAIChat(prompt, message);
                }
            }

        }
}
