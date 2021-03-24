using System.Threading.Tasks;
using UnityEngine;

namespace Pngcs.Unity
{
    internal static class PNG
    {
        public static async Task WriteAsync
        (
            Color[] pixels,
            int width,
            int height,
            int bitDepth,
            bool alpha,
            bool greyscale,
            string filePath
        )
        {
            try
            {
                await Task.Run(() =>
                    Write(
                        pixels: pixels,
                        width: width,
                        height: height,
                        bitDepth: bitDepth,
                        alpha: alpha,
                        greyscale: greyscale,
                        filePath: filePath
                    )
                );
            }
            catch (System.Exception ex) { Debug.LogException(ex); await Task.CompletedTask; }//kills debugger execution loop on exception
            finally { await Task.CompletedTask; }
        }

        public static async Task WriteAsync
        (
            Color32[] pixels,
            int width,
            int height,
            int bitDepth,
            bool alpha,
            bool greyscale,
            string filePath
        )
        {
            try
            {
                await Task.Run(() =>
                    Write(
                        pixels: pixels,
                        width: width,
                        height: height,
                        bitDepth: bitDepth,
                        alpha: alpha,
                        greyscale: greyscale,
                        filePath: filePath
                    )
                );
            }
            catch (System.Exception ex) { Debug.LogException(ex); await Task.CompletedTask; }//kills debugger execution loop on exception
            finally { await Task.CompletedTask; }
        }

        public static void Write
        (
            Color[] pixels,
            int width,
            int height,
            int bitDepth,
            bool alpha,
            bool greyscale,
            string filePath
        )
        {
            // convert pixels
            float max = GetBitDepthMaxValue(bitDepth);
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = pixels[i] * max;
            }

            var info = new ImageInfo(
                width,
                height,
                bitDepth,
                alpha,
                greyscale,
                false//not implemented here yet//bitDepth==4
            );

            // open image for writing:
            PngWriter writer = FileHelper.CreatePngWriter(filePath, info, true);
 
            int numRows = info.Rows;
            int numCols = info.Cols;
            ImageLine imageline = new ImageLine(info);
            for (int row = 0; row < numRows; row++)
            {
                //fill line:
                if (greyscale == false)
                {
                    for (int col = 0; col < numCols; col++)
                    {
                        var rgba = pixels[IndexPngToTexture(row, col, numRows, numCols)];
                        ImageLineHelper.SetPixel(imageline, col, (int)rgba.r, (int)rgba.g, (int)rgba.b, (int)rgba.a);
                    }
                }
                else
                {
                    if (alpha == false)
                    {
                        for (int col = 0; col < numCols; col++)
                        {
                            int r = (int)pixels[IndexPngToTexture(row, col, numRows, numCols)].r;
                            ImageLineHelper.SetPixel(imageline, col, r);
                        }
                    }
                    else
                    {
                        for (int col = 0; col < numCols; col++)
                        {
                            int a = (int)pixels[IndexPngToTexture(row, col, numRows, numCols)].a;
                            ImageLineHelper.SetPixel(imageline, col, a);
                        }
                    }
                }

                //write line:
                writer.WriteRow(imageline, row);
            }
            writer.End();
        }

        public static void Write
        (
            Color32[] pixels,
            int width,
            int height,
            int bitDepth,
            bool alpha,
            bool greyscale,
            string filePath
        )
        {
            // convert pixels
            float max = GetBitDepthMaxValue(bitDepth);
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = (Color)pixels[i] * max;
            }

            var info = new ImageInfo(
                width,
                height,
                bitDepth,
                alpha,
                greyscale,
                false//not implemented here yet//bitDepth==4
            );

            // open image for writing:
            PngWriter writer = FileHelper.CreatePngWriter(filePath, info, true);

            int numRows = info.Rows;
            int numCols = info.Cols;

            ImageLine imageline = new ImageLine(info);
            for (int row = 0; row < numRows; row++)
            {
                //fill line:
                if (greyscale == false)
                {
                    for (int col = 0; col < numCols; col++)
                    {
                        var rgba = pixels[IndexPngToTexture(row, col, numRows, numCols)];
                        ImageLineHelper.SetPixel(imageline, col, rgba.r, rgba.g, rgba.b, rgba.a);
                    }
                }
                else
                {
                    if (alpha == false)
                    {
                        for (int col = 0; col < numCols; col++)
                        {
                            int r = pixels[IndexPngToTexture(row, col, numRows, numCols)].r;
                            ImageLineHelper.SetPixel(imageline, col, r);
                        }
                    }
                    else
                    {
                        for (int col = 0; col < numCols; col++)
                        {
                            int a = pixels[IndexPngToTexture(row, col, numRows, numCols)].a;
                            ImageLineHelper.SetPixel(imageline, col, a);
                        }
                    }
                }

                //write line:
                writer.WriteRow(imageline, row);
            }
            writer.End();
        }
        /// <summary> Texture2D's rows start from the bottom while PNG from the top. Hence inverted y/row. </summary>
        public static int IndexPngToTexture(int row, int col, int numRows, int numCols) => numCols * (numRows - 1 - row) + col;

        public static uint GetBitDepthMaxValue(int bitDepth)
        {
            switch (bitDepth)
            {
                case 1: return 1;
                case 2: return 3;
                case 4: return 15;
                case 8: return byte.MaxValue;
                case 16: return ushort.MaxValue;
                case 32: return uint.MaxValue;
                default: throw new System.Exception($"bitDepth '{ bitDepth }' not implemented");
            }
        }
    }
}
