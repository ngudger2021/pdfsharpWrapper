using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using QRCoder;
using DataMatrix.net;
using System.Drawing;
using PdfSharp;
using System.Drawing.Imaging;
using ZXing.Common;
using Microsoft.SqlServer.Server;
using System.Windows;

namespace pdfSharpWrapper
{
    public class PdfSharpWrapper
    {
        public PdfDocument document;
        public XGraphics gfx;
        public XUnit currentPageTop;
        public XUnit lineSpacing;

        public PdfSharpWrapper()
        {
            document = new PdfDocument();
            currentPageTop = XUnit.FromMillimeter(10); // start the first page at 10mm from the top
            lineSpacing = XUnit.FromMillimeter(5); // set the default line spacing to 5mm
        }

        public void AddPage()
        {
            PdfPage page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            currentPageTop = XUnit.FromMillimeter(10); // reset the current page top to 10mm
        }

        public void DrawString(string text, XFont font, XBrush brush, XRect rect, XStringFormat format, XUnit? customLineSpacing = null)
        {
            XSize size = gfx.MeasureString(text, font);
            rect.Y = currentPageTop;
            rect.Height = size.Height;
            gfx.DrawString(text, font, brush, rect, format);
            currentPageTop += size.Height + (customLineSpacing ?? lineSpacing);
        }

        public void DrawRectangle(XPen pen, XBrush brush, XRect rect)
        {
            gfx.DrawRectangle(pen, brush, rect);
        }

        public void DrawQrCode(XGraphics gfx, string code, XRect rect, double quietZone, XColor darkColor, XColor lightColor, XSize size)
        {
            var qrCode = new QRCodeGenerator().CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            var moduleSize = new XSize(size.Width / qrCode.ModuleMatrix.Count, size.Height / qrCode.ModuleMatrix.Count);

            for (int x = 0; x < qrCode.ModuleMatrix.Count; x++)
            {
                for (int y = 0; y < qrCode.ModuleMatrix.Count; y++)
                {
                    var module = qrCode.ModuleMatrix[x][y];
                    if (module)
                    {
                        var moduleRect = new XRect(rect.X + x * moduleSize.Width, rect.Y + y * moduleSize.Height, moduleSize.Width, moduleSize.Height);
                        gfx.DrawRectangle(new XPen(darkColor, 0), moduleRect);
                        gfx.DrawRectangle(new XSolidBrush(darkColor), moduleRect);
                    }
                    else
                    {
                        var moduleRect = new XRect(rect.X + x * moduleSize.Width, rect.Y + y * moduleSize.Height, moduleSize.Width, moduleSize.Height);
                        gfx.DrawRectangle(new XPen(lightColor, 0), moduleRect);
                        gfx.DrawRectangle(new XSolidBrush(lightColor), moduleRect);
                    }
                }
            }
        }

        private void DrawModule(XGraphics gfx, XBrush brush, XRect moduleRect, double xPos, double yPos)
        {
            moduleRect = new XRect(new XPoint(xPos, yPos), moduleRect.Size);
            gfx.DrawRectangle(brush, moduleRect);
        }

        public void DrawDataMatrix(string text, XUnit x, XUnit y, XUnit width, XUnit height, XColor color, bool showText)
        {
            DmtxImageEncoder encoder = new DmtxImageEncoder();
            Bitmap dataMatrixImage = encoder.EncodeImage(text);
            XRect rect = new XRect(x, y, width, height);
            using (var ms = new MemoryStream())
            {
                dataMatrixImage.Save(ms, ImageFormat.Png);
                XImage image = XImage.FromStream(ms);
                gfx.DrawImage(image, rect);
            }
            if (showText)
            {
                XSize textSize = gfx.MeasureString(text, new XFont("Arial", 8));
                XPoint textPos = new XPoint(x + width / 2 - textSize.Width / 2, y + height + 3);
                gfx.DrawString(text, new XFont("Arial", 8), XBrushes.Black, textPos);
            }
        }

        public void DrawBarcode(string text, BarcodeLib.TYPE type, XUnit x, XUnit y, XUnit width, XUnit height, XColor color, bool showText)
        {
            BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();
            barcode.IncludeLabel = showText;
            barcode.LabelFont = new Font("Arial", 8);
            barcode.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
            barcode.Encode(type, text, (int)width, (int)height);
            Bitmap barcodeImage = new Bitmap(barcode.EncodedImage);
            XRect rect = new XRect(x, y, width, height);
            using (var ms = new MemoryStream())
            {
                barcodeImage.Save(ms, ImageFormat.Png);
                XImage image = XImage.FromStream(ms);
                gfx.DrawImage(image, rect);
            }
            if (showText)
            {
                XSize textSize = gfx.MeasureString(text, new XFont("Arial", 8));
                XPoint textPos = new XPoint(x + width / 2 - textSize.Width / 2, y + height + 2);
                //gfx.DrawString(text, new XFont("Arial", 8), XBrushes.Black, textPos);
            }
        }

        public void DrawTextVertical(XGraphics gfx, string text, XFont font, XBrush brush, float x, float y)
        {
            // Rotate the graphics context by 90 degrees
            gfx.RotateTransform(90);
            // Translate the origin to the drawing point
            gfx.TranslateTransform(y, -x);
            // Draw the text
            gfx.DrawString(text, font, brush, 0, 0);
            //graphics.DrawString(text, font, brush, 0, 0);
            // Reset the graphics context to its original state
            gfx.RotateTransform(-90);
        }

        public void Save(string filePath)
        {
            document.Save(filePath);
        }

        public void Dispose()
        {
            gfx.Dispose();
            document.Dispose();
        }

        public static XGraphics GetGraphic(PdfDocument document, PageSize pageSize)
        {
            PdfPage page = document.AddPage();
            page.Size = pageSize;
            return XGraphics.FromPdfPage(page);
        }
    }

    public static class XColorExtensions
    {
        public static Color ToGdiColor(this XColor color)
        {
            int a = (int)(color.A * 255);
            int r = (int)(color.R * 255);
            int g = (int)(color.G * 255);
            int b = (int)(color.B * 255);
            return Color.FromArgb(a, r, g, b);
        }
    }

    public static class ColorExtensions
    {
        public static XColor ToXColor(this Color color)
        {
            return XColor.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
