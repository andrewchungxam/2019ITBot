﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public static class ImageExtensions
    {
        public static byte[] ConvertToByteArrayFromBitmap(this Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }
    }

    public class BarcodeScannerService
    {
        private readonly IBarcodeReader barcodeReader;
        private readonly ZXing.IBarcodeReaderGeneric barcodeReader2;

        public BarcodeScannerService()
        {
            barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new DecodingOptions
                {
                    TryHarder = true
                },
            };

            barcodeReader2 = new BarcodeReaderGeneric
            {
                AutoRotate = true,
                Options = new DecodingOptions
                {
                    TryHarder = true
                },
            };
        }

        public String DecodeBarcode(byte[] byteArrayFile)
        {

            var newStringBuilder = new StringBuilder();

            try
            {
                try
                {

                    Bitmap bmp;
                    byte[] heyNow;
                    using (var ms = new MemoryStream(byteArrayFile))
                    {
                        bmp = new Bitmap(ms);

                        //var image = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format64bppArgb);  //Bitmap.Config.ARGB_8888);
                        //heyNow = image.ConvertToByteArrayFromBitmap(ImageFormat.Bmp);
                        heyNow = bmp.ConvertToByteArrayFromBitmap(ImageFormat.Bmp);

                    }
                    var newRBGLuminanceSource = new RGBLuminanceSource(heyNow, bmp.Width, bmp.Height);
                    //var newRBGLuminanceSource = new RGBLuminanceSource(byteArrayFile, bmp.Width, bmp.Height);

                    var result = barcodeReader.Decode(newRBGLuminanceSource);

                    int hi = 5;

                    bmp.Dispose();

                    //foreach (var metaData in result.ResultMetadata)
                    //{
                    //    newStringBuilder.Append($"Metadata:{metaData.Key}: {metaData.Value}");
                    //    newStringBuilder.Append(Environment.NewLine);
                    //    newStringBuilder.Append($"Barcode Format: {result.BarcodeFormat.ToString()}");
                    //    newStringBuilder.Append(Environment.NewLine);
                    //    newStringBuilder.Append($"Text:{result.ToString()}");
                    //    //resultWriter.WriteLine("METADATA:{0}:{1}", metaData.Key, metaData.Value);
                    //}

                    foreach (var metaData in result.ResultMetadata)
                    {
                        //newStringBuilder.Append($"Metadata:{metaData.Key}: {metaData.Value}");
                        //newStringBuilder.Append(Environment.NewLine);
                        //newStringBuilder.Append($"Barcode Format: {result.BarcodeFormat.ToString()}");
                        //newStringBuilder.Append(Environment.NewLine);
                        newStringBuilder.Append($"{result.ToString()}");
                    }

                    return newStringBuilder.ToString();
                }
                catch (Exception innerExc)
                {
                    Console.WriteLine("Exception: {0}", innerExc.Message);
                    Debug.WriteLine("Exception: {0}", innerExc.Message);
                    return "Inner Execption";
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception: {0}", exc.Message);
                Debug.WriteLine("Exception: {0}", exc.Message);
                return "Exception";
            }
        }
    }
}

