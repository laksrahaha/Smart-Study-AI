using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace SmartStudyAI.Backend.Services
{
    public class PDFExtractionService
    {
        public async Task<string> ExtractTextFromPDF(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
            {
                throw new ArgumentException("No file provided.");
            }

            // Check if file is a PDF
            if (!pdfFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Only PDF files are supported.");
            }

            // Max file size: 25MB
            if (pdfFile.Length > 25 * 1024 * 1024)
            {
                throw new ArgumentException("File size exceeds 25MB limit.");
            }

            var extractedText = new System.Text.StringBuilder();

            using (var stream = pdfFile.OpenReadStream())
            {
                // Open PDF reader
                using (var pdfReader = new PdfReader(stream))
                {
                    using (var pdfDocument = new PdfDocument(pdfReader))
                    {
                        int pageCount = pdfDocument.GetNumberOfPages();

                        // Extract text from each page
                        for (int pageNum = 1; pageNum <= pageCount; pageNum++)
                        {
                            var page = pdfDocument.GetPage(pageNum);
                            var strategy = new SimpleTextExtractionStrategy();
                            var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                            extractedText.Append(text);
                            extractedText.Append("\n---PAGE BREAK---\n");
                        }
                    }
                }
            }

            return await Task.FromResult(extractedText.ToString());
        }

        public async Task<string> ExtractTextFromDocument(IFormFile documentFile)
        {
            if (documentFile == null || documentFile.Length == 0)
            {
                throw new ArgumentException("No file provided.");
            }

            // Max file size: 25MB
            if (documentFile.Length > 25 * 1024 * 1024)
            {
                throw new ArgumentException("File size exceeds 25MB limit.");
            }

            var fileName = documentFile.FileName.ToLower();

            // For now, we'll handle PDF files
            // You can extend this to support .docx, .txt, etc.
            if (fileName.EndsWith(".pdf"))
            {
                return await ExtractTextFromPDF(documentFile);
            }
            else if (fileName.EndsWith(".txt"))
            {
                return await ExtractTextFromTextFile(documentFile);
            }
            else
            {
                throw new ArgumentException("Supported formats: PDF, TXT");
            }
        }

        private async Task<string> ExtractTextFromTextFile(IFormFile txtFile)
        {
            using (var reader = new StreamReader(txtFile.OpenReadStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
