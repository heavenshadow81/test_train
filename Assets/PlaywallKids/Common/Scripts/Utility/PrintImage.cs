using UnityEngine;
using System;
using System.Drawing.Printing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace ML.PlaywallKids.Common
{
    /// <summary>
    /// 비트맵 이미지를 프린터로 출력하는 클래스. <seealso cref="DragonPark.DragonPrinter"/>, <seealso cref="Aquarium.AquariumPrinter"/> 참조.
    /// </summary>
    public class PrintImage
    {
        Image image;
        PrintDocument document;

        /// <summary>
        /// 이미지 파일로 오브젝트 생성
        /// </summary>
        public PrintImage(string imagePath, string printerName = "")
        {
            image = new Bitmap(imagePath);
            _SetupPrinter(printerName);
        }

        /// <summary>
        /// byte[] 이미지 데이터로 오브젝트 생성
        /// </summary>
        public PrintImage(byte[] data, int width, int height, string printerName = "")
        {
            byte[] newBytes = ConvertTexture2DRawDataToBitmapScan0(data, width, height);
            Bitmap bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(newBytes, 0));
            image = bitmap;
            _SetupPrinter(printerName);
        }

        /// <summary>
        /// 텍스처 raw ARGB32 데이터를 Bitmap Scan0에 맞게 변환한다. (endian 문제로 인해 바이트 순서를 바꾸어야 함)
        /// </summary>
        static byte[] ConvertTexture2DRawDataToBitmapScan0(byte[] data, int width, int height)
        {
            byte[] result = new byte[data.Length];
            int stride = width * 4;

            for (int i = 0; i < height; i++)
            {
                int ri = (height - 1) - i;
                for (int j = 0; j < width; j++)
                {
                    result[ri * stride + (j * 4) + 0] = data[i * stride + (j * 4) + 3];
                    result[ri * stride + (j * 4) + 1] = data[i * stride + (j * 4) + 2];
                    result[ri * stride + (j * 4) + 2] = data[i * stride + (j * 4) + 1];
                    result[ri * stride + (j * 4) + 3] = data[i * stride + (j * 4) + 0];
                }
            }
            return result;
        }

        private void _SetupPrinter(string printerName)
        {
            // 프린트 document 생성
            document = new PrintDocument();
            document.DocumentName = "Image";

            // 프린터 설정
            PrinterSettings printerSettings = new PrinterSettings();
            printerSettings.PrinterName = printerName;
            if (!printerSettings.IsValid)
                printerSettings.PrinterName = "";

            // 기본 프린터 선택
            PrinterSettings.StringCollection printers = PrinterSettings.InstalledPrinters;
            foreach (string pName in printers)
            {
                PrinterSettings printerSettings2 = new PrinterSettings();
                printerSettings2.PrinterName = pName;
                if (string.IsNullOrEmpty(printerSettings.PrinterName))
                    printerSettings = printerSettings2;
                if (printerSettings2.IsDefaultPrinter)
                {
                    printerSettings = printerSettings2;
                    break;
                }
            }
            printerSettings.Copies = 1;
            document.PrinterSettings = printerSettings;

            // 기본 페이지 설정 할당
            document.DefaultPageSettings = printerSettings.DefaultPageSettings;
            // Unity에서는 DefaultPageSettings의 PageSize가 무조건 A4인 문제가 발생하는듯.
            bool defaultPageSettingsIsValid = false;
            foreach (PaperSize paperSize in printerSettings.PaperSizes)
            {
                if (paperSize.PaperName.Equals(document.DefaultPageSettings.PaperSize.PaperName))
                {
                    defaultPageSettingsIsValid = true;
                    break;
                }
            }
            if (!defaultPageSettingsIsValid)
                document.DefaultPageSettings.PaperSize = printerSettings.PaperSizes[0];
            document.DefaultPageSettings.Landscape = false; // 세로로 프린트

            // margin을 0으로 설정
            Margins margin = document.DefaultPageSettings.Margins;
            margin.Top = 0;
            margin.Bottom = 0;
            margin.Left = 0;
            margin.Right = 0;
            document.DefaultPageSettings.Margins = margin;

            // 페이지 이벤트 추가
            document.PrintPage += new PrintPageEventHandler(PrintPage);
        }

        /// <summary>
        /// 프린트 실행
        /// </summary>
        public bool Print()
        {
            // 프린터 출력창이 기본적으로 나타나지 않도록 함.
            document.PrintController = new StandardPrintController();
            try
            {
                document.Print();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        void PrintPage(object sender, PrintPageEventArgs e)
        {
            // 그래픽 context 가져오기
            System.Drawing.Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // Aspect Ratio를 유지하며 Scale하기
            GraphicsUnit pageUnit = e.Graphics.PageUnit;
            RectangleF srcArea = image.GetBounds(ref pageUnit);
            RectangleF destArea = e.MarginBounds;
            float srcAspect = srcArea.Width / srcArea.Height;
            float destAspect = destArea.Width / destArea.Height;

            if (srcAspect > destAspect)
            {
                destArea.Width = (int)(destArea.Height * srcAspect);
                destArea.X += (e.MarginBounds.Width - destArea.Width) / 2;
            }
            else
            {
                destArea.Height = (int)(destArea.Width / srcAspect);
                destArea.Y += (e.MarginBounds.Height - destArea.Height) / 2;
            }

            // 그리기
            g.DrawImage(image, destArea, srcArea, pageUnit);
        }
    }
}