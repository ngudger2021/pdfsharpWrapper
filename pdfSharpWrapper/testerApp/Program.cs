using System;
using System.Drawing;
using System.Windows.Controls;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using pdfSharpWrapper;

namespace testerApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PdfSharpWrapper pdf = new PdfSharpWrapper();
            pdf.AddPage();
            pdf.lineSpacing= 1;

            // Add a string
            XFont font = new XFont("Arial", 12);
            XBrush brush = XBrushes.Black;
            XRect rect = new XRect(10, 10, 200, 20);
            pdf.DrawString("Hello, world!  1", font, brush, rect, XStringFormats.TopLeft);
            pdf.DrawString("Hello, world!  2", font, brush, rect, XStringFormats.TopLeft);
            pdf.DrawString("Hello, world!  3", font, brush, rect, XStringFormats.TopLeft);
            pdf.DrawString("Hello, world!  4", font, brush, rect, XStringFormats.TopLeft);
            pdf.DrawString("Hello, world!  5", font, brush, rect, XStringFormats.TopLeft);
            pdf.DrawString("Hello, world!  6", font, brush, rect, XStringFormats.TopLeft);
            pdf.DrawString("Hello, world!  7", font, brush, rect, XStringFormats.TopLeft);
            pdf.DrawString("Hello, world!  8", font, brush, rect, XStringFormats.TopLeft);
            pdf.DrawString("Hello, world!  9", font, brush, rect, XStringFormats.TopLeft);
            pdf.DrawString("Hello, world! 10", font, brush, rect, XStringFormats.TopLeft);
            pdf.DrawTextVertical(pdf.gfx, "Hello world! 11", font, brush, 170, 50);

            // Add a QR code
            string qrText = "https://www.example.com";
            XRect qrRect = new XRect(100, 50, 50, 50);
            
            pdf.DrawQrCode(pdf.gfx, "test_qr_code", qrRect, 0, XColors.Black, XColors.White, new XSize(40, 40));

            // Add a DataMatrix
            string dataMatrixText = "Hello, world!";
            XUnit dataMatrixX = 100;
            XUnit dataMatrixY = 120;
            XUnit dataMatrixWidth = 50;
            XUnit dataMatrixHeight = 50;
            XColor dataMatrixColor = XColors.Black;
            bool showDataMatrixText = true;
            pdf.DrawDataMatrix(dataMatrixText, dataMatrixX, dataMatrixY, dataMatrixWidth, dataMatrixHeight, dataMatrixColor, showDataMatrixText);

            // Add a barcode
            string barcodeText = "1234567890";
            BarcodeLib.TYPE barcodeType = BarcodeLib.TYPE.CODE128;
            XUnit barcodeX = 100;
            XUnit barcodeY = 200;
            XUnit barcodeWidth = 100;
            XUnit barcodeHeight = 50;
            XColor barcodeColor = XColors.Black;
            bool showBarcodeText = true;
            pdf.DrawBarcode(barcodeText, barcodeType, barcodeX, barcodeY, barcodeWidth, barcodeHeight, barcodeColor, showBarcodeText);

            // Save the document
            string filename = "C:\\Users\\nickg\\Downloads\\output.pdf";
            pdf.document.Save(filename);
        }
    }
}
