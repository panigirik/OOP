using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ConsoleWord.Infrastracture;
using ConsoleWord.Core.Decorators; // Include decorators for text styling
using Document = ConsoleWord.Core.Entities.Document;

namespace ConsoleWord.Application.Services
{
    public class DocumentService
    {
        private readonly StorageService _storageService;

        public DocumentService(StorageService storageService)
        {
            _storageService = storageService;
        }

        public void CreateAndSaveDocument()
        {
            Console.Write("Enter document name: ");
            string documentName = Console.ReadLine()?.Trim();
            while (string.IsNullOrWhiteSpace(documentName))
            {
                Console.WriteLine("Invalid document name. Please enter a valid name.");
                documentName = Console.ReadLine()?.Trim();
            }

            Console.Write("Enter text: ");
            string documentText = Console.ReadLine()?.Trim();

            Console.Write("Choose font (Arial, Times New Roman, Courier New): ");
            string font = Console.ReadLine()?.Trim();

            Console.Write("Choose text size (e.g., 12, 14, 16): ");
            int textSize;
            while (!int.TryParse(Console.ReadLine(), out textSize) || textSize <= 0)
            {
                Console.WriteLine("Invalid text size. Please enter a valid size (e.g., 12, 14, 16): ");
            }

            // Create the Document instance
            Document document = new Document(documentName, documentText, font, textSize);

            // Choose text decorations one by one
            Console.WriteLine("Do you want the text to be bold? (1 - Yes, 2 - No): ");
            string boldChoice = Console.ReadLine()?.Trim();
            if (boldChoice == "1")
            {
                document = new BoldDecorator(document);
            }

            Console.WriteLine("Do you want the text to be italic? (1 - Yes, 2 - No): ");
            string italicChoice = Console.ReadLine()?.Trim();
            if (italicChoice == "1")
            {
                document = new ItalicDecorator(document);
            }

            Console.WriteLine("Do you want the text to be underlined? (1 - Yes, 2 - No): ");
            string underlineChoice = Console.ReadLine()?.Trim();
            if (underlineChoice == "1")
            {
                document = new UnderlineDecorator(document);
            }

            Console.Write("Where do you want to save the document? (local/cloud): ");
            string storageType = Console.ReadLine()?.Trim().ToLower();

            if (storageType == "local")
            {
                Console.Write("Enter save directory: ");
                string directory = Console.ReadLine()?.Trim();
                while (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
                {
                    Console.WriteLine("Invalid directory path. Please enter a valid directory.");
                    directory = Console.ReadLine()?.Trim();
                }
                SaveDocumentLocally(document, directory);
            }
            else if (storageType == "cloud")
            {
                SaveDocumentToCloud(document);
            }
            else
            {
                Console.WriteLine("Invalid storage type.");
            }
        }

        public void SaveDocumentLocally(Document document, string directory)
        {
            try
            {
                string filePath = Path.Combine(directory, document.Name + ".docx");

                // Create a new Word document using Open XML SDK
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    // Add the main part of the document
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(new Body());

                    // Add the content to the document
                    Body body = mainPart.Document.Body;
                    Paragraph paragraph = new Paragraph();
                    Run run = new Run(new Text(document.Content.ToString())); // Use ToString for StringBuilder
                    paragraph.Append(run);
                    body.Append(paragraph);

                    // Font size and settings
                    RunProperties runProperties = new RunProperties();
                    runProperties.Append(new FontSize() { Val = (document.TextSize * 2).ToString() }); // Multiply by 2 for Open XML's half-size units
                    runProperties.Append(new RunFonts() { Ascii = document.Font }); // Set font
                    run.PrependChild(runProperties);

                    // Apply any text decorations (bold, italic, underline)
                    if (document is BoldDecorator)
                    {
                        run.PrependChild(new Bold());
                    }
                    if (document is ItalicDecorator)
                    {
                        run.PrependChild(new Italic());
                    }
                    if (document is UnderlineDecorator)
                    {
                        run.PrependChild(new Underline());
                    }

                    // Save the document
                    wordDocument.Save();
                }

                Console.WriteLine($"Document saved successfully: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving document: {ex.Message}");
            }
        }

        public void SaveDocumentToCloud(Document document)
        {
            _storageService.UploadToCloud(document, "cloud-storage-placeholder");
            Console.WriteLine("Document uploaded to cloud (stub).");
        }
    }
}
