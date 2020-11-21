using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Griddlers.Library
{
    public static class ImageLogic
    {

        private static readonly string? Base = AppDomain.CurrentDomain.BaseDirectory;

        private static bool IsBlack(this Rgba32 rgba32)
        {
            //return rgba32.B <= 30 && rgba32.G <= 30 && rgba32.R <= 30;
            return rgba32.B <= 180 && rgba32.G <= 180 && rgba32.R <= 180;
        }

        private static Dictionary<(int, int), T> ToDictionary<T>(this List<List<T>> list)
        {
            Dictionary<(int, int), T> Dictionary = new Dictionary<(int, int), T>();
            int firstCount = 0, secondCount = 0;

            foreach (List<T> first in list)
            {
                foreach (T sec in first)
                {
                    Dictionary.TryAdd((firstCount, secondCount), sec);
                    secondCount++;
                }
                firstCount++;
            }

            return Dictionary;
        }

        private static T[][] ToArray<T>(this List<List<T>> list)
        {
            List<T[]> array = new List<T[]>();

            foreach (List<T> item in list)
            {
                array.Add(item.ToArray());
            }

            return array.ToArray();
        }

        public static (Dictionary<(int, int), int>, Dictionary<(int, int), int>) LoadFile(string fileName)
        {
            List<List<int>> rows = new List<List<int>>();
            List<List<int>> columns = new List<List<int>>();

            using (FileStream s = File.OpenRead(Base + "/Images/" + fileName + ".png"))
            {
                Image<Rgba32> Im = (Image<Rgba32>)Image.Load(s);
                ProcessImageLine(true, Im, rows, Im.Height, Im.Width);
                ProcessImageLine(false, Im, columns, Im.Width, Im.Height);
            }

            //using (FileStream s = File.OpenRead(Base + "/Images/" + fileName + "-row.png"))
            //{
            //    Image<Rgba32> Im = Image.Load(s);
            //    ReadNumbers(true, Im, rows, Im.Height, Im.Width);
            //}

            //using (FileStream s = File.OpenRead(Base + "/Images/" + fileName + "-col.png"))
            //{
            //    Image<Rgba32> Im = Image.Load(s);
            //    ReadNumbers(false, Im, columns, Im.Height, Im.Width);
            //}

            return (rows.ToDictionary(), columns.ToDictionary());
        }

        public static (string, string) UploadFile(IFormFile file)
        {
            List<List<int>> rows = new List<List<int>>();
            List<List<int>> columns = new List<List<int>>();
            (string jsonRows, string jsonCols) = (string.Empty, string.Empty);
            byte[] data = new byte[] { };

            using (MemoryStream s = new MemoryStream())
            {
                file.CopyTo(s);
                data = s.ToArray();
            }

            if (data != null)
            {
                Image<Rgba32> image = Image.Load(data);
                ProcessImageLine(true, image, rows, image.Height, image.Width);
                ProcessImageLine(false, image, columns, image.Width, image.Height);

                int[][] rowsArray = rows.ToArray<int>();
                int[][] colsArray = columns.ToArray<int>();                

                jsonRows = JsonSerializer.Serialize(rowsArray);
                jsonCols = JsonSerializer.Serialize(colsArray);
            }

            return (jsonRows, jsonCols);
        } 

        private static void ProcessImageLine(bool isRow, Image<Rgba32> Im, List<List<int>> lines, int width, int height)
        {
            int x = 7, y = 0, sS = 9, lineCount = 0, itemCount = 0;

            for (int i = x; i < width; i++)
            {
                List<int> line = new List<int>();
                int solidPartCount = 0, solidCount = 0;
                int pixelsWithoutBlack = 0;
                for (int c = y; c < height; c++)
                {
                    Rgba32 Pixel = isRow ? Im[c, i] : Im[i, c];

                    if (Pixel.IsBlack())
                    {
                        pixelsWithoutBlack = 0;
                        solidPartCount++;

                        if (c > height - sS && solidCount > 0)
                        {
                            line.Add(solidCount + 1);
                            break;
                        }
                    }
                    else
                    {
                        pixelsWithoutBlack++;

                        if (solidPartCount > sS)
                            solidCount++;

                        if (solidCount > 0 && pixelsWithoutBlack > sS)
                        {
                            line.Add(solidCount);
                            itemCount++;
                            solidCount = 0;
                        }

                        solidPartCount = 0;
                    }
                }
                lines.Add(line);
                lineCount++;
                itemCount = 0;
                i += sS + 6;
            }

        }

        private static (byte, bool) GetNumber(byte horzLines, byte diagLines, byte vertLines, byte curves)
        {
            byte num = 0;
            bool valid = true;

            //1 & 4
            if (diagLines == 1 && vertLines == 1 && horzLines == 1 && curves == 0)
                valid = false;

            //6, 8 & 9
            if (curves == 1 && horzLines == 0 && vertLines == 0 && diagLines == 0)
                valid = false;

            if (diagLines <= 1 && vertLines == 1 && horzLines <= 1 && curves == 0 && (horzLines == 0 || diagLines == 1))
                num = 1;
            else if (horzLines == 1 && curves == 1 && diagLines == 1 && vertLines == 0)
                num = 2;
            else if (curves == 2 && horzLines == 0 && diagLines == 0 && vertLines == 0)
                num = 3;
            else if (((diagLines == 1 && vertLines == 1) || (diagLines == 0 && vertLines == 2))
                        && horzLines == 1 && curves == 0)
                num = 4;
            else if (horzLines == 1 && vertLines == 1 && curves == 1 && diagLines == 0)
                num = 5;
            else if (curves == 1 && horzLines == 0 && vertLines == 0 && diagLines == 0)
                num = 6;
            else if (horzLines == 1 && diagLines == 1 && vertLines == 0 && curves == 0)
                num = 7;
            else if (curves == 1 && horzLines == 0 && vertLines == 0 && diagLines == 0)
                num = 8;
            else if (curves == 1 && horzLines == 0 && vertLines == 0 && diagLines == 0)
                num = 9;

            return (num, valid);
        }

        private static void ReadNumbers(bool isRow, Image<Rgba32> Im, Dictionary<(int, int), int> lines, int width, int height)
        {
            int cellX = 0, cellY = 0, cellWidth = 0;
            int vertBlackCount = 0, horzBlackCount = 0;
            int lineCount = 0, itemCount = 0;//, x = 7, y = 0, sS = 9;

            for (int i = 0; i < width; i++)
            {
                for (int c = 0; c < height; c++)
                {
                    Rgba32 Pixel = Im[i, c];

                    if (Pixel.B <= 30 && Pixel.G <= 30 && Pixel.R <= 30)
                    {
                        cellX = i;
                        vertBlackCount++;
                    }
                    else
                        break;
                }

                if (vertBlackCount == height - 1)
                    break;
            }

            for (int i = 0; i < height; i++)
            {
                for (int c = 0; c < width; c++)
                {
                    Rgba32 Pixel = Im[c, i];

                    if (Pixel.B <= 30 && Pixel.G <= 30 && Pixel.R <= 30)
                    {
                        cellY = i;
                        horzBlackCount++;
                    }
                    else
                        break;
                }

                if (horzBlackCount == width - 1)
                    break;
            }

            for (int i = cellX; i < width; i++)
            {
                Rgba32 Pixel = Im[i, cellY + 2];

                if (Pixel.B <= 30 && Pixel.G <= 30 && Pixel.R <= 30)
                    break;
                else
                    cellWidth++;
            }

            for (int i = cellY; i < height; i++)
            {
                for (int c = cellX; c < width; c++)
                {
                    byte horizonalLines = FindHorizontalLines(Im, c, i, cellWidth);
                    byte verticalLines = FindVerticalLines(Im, c, i, cellWidth);
                    byte diagonalLines = FindDiagonalLines(Im, c, i, cellWidth);
                    byte curves = FindCurves(Im, c, i, cellWidth);
                    (byte Number, bool valid) = GetNumber(horizonalLines, diagonalLines, verticalLines, curves);

                    if (Number == 0)
                    {
                        //multiple

                    }
                    else if (valid)
                    {
                        lines.TryAdd((lineCount, itemCount), Number);
                    }

                    itemCount++;
                    c += cellWidth;
                }
                lineCount++;
                i += cellWidth;
            }

        }
        private static byte FindHorizontalLines(Image<Rgba32> im, int x, int y, int w)
            => FindPerpendicularLines(true, im, x, y, w);

        private static byte FindVerticalLines(Image<Rgba32> im, int x, int y, int w)
            => FindPerpendicularLines(false, im, x, y, w);

        private static byte FindPerpendicularLines(bool isRow, Image<Rgba32> im, int x, int y, int w)
        {
            const byte linePixels = 5;
            byte lineCount = 0;

            for (int i = isRow ? y : x; i < (isRow ? y + w : x + w); i++)
            {
                int pixelCount = 0;

                for (int c = isRow ? x : y; c < (isRow ? x + w : y + w); c++)
                {
                    Rgba32 pixel = isRow ? im[c, i] : im[i, c];

                    if (pixel.IsBlack())
                        pixelCount++;
                    else
                        pixelCount = 0;

                    if (pixelCount >= linePixels)
                    {
                        lineCount++;
                        break;
                    }
                }
            }

            return lineCount;
        }

        private static byte FindDiagonalLines(Image<Rgba32> im, int x, int y, int w)
        {
            const byte linePixels = 5;
            byte lineCount = 0;
            bool finish = false;

            for (int i = y; i < y + w; i++)
            {
                int pixelCount = 0;

                for (int c = x; c < x + w; c++)
                {
                    int curX = c, curY = i;

                    do
                    {
                        Rgba32 pixel = im[curX, curY];

                        if (pixel.IsBlack())
                            pixelCount++;
                        else
                            pixelCount = 0;

                        if (pixelCount >= linePixels)
                        {
                            lineCount++;
                            finish = true;
                            break;
                        }

                        curX++;
                        curY++;

                    } while (curX < x + w && curY < y + w);

                    if (finish)
                        break;

                    curX = c;
                    curY = i;

                    do
                    {
                        Rgba32 pixel = im[curX, curY];

                        if (pixel.IsBlack())
                            pixelCount++;
                        else
                            pixelCount = 0;

                        if (pixelCount >= linePixels)
                        {
                            lineCount++;
                            finish = true;
                            break;
                        }

                        curX--;
                        curY++;

                    } while (curX > x && curY < y + w);

                    if (finish)
                        break;
                }

                if (finish)
                    break;
            }

            return lineCount;
        }

        private static byte FindCurves(Image<Rgba32> im, int x, int y, int w)
        {
            const byte linePixels = 5;
            byte lineCount = 0;
            bool finish = false;

            for (int i = y; i < y + w; i++)
            {
                int pixelCount = 0;

                for (int c = x; c < x + w; c++)
                {
                    int curX = c, curY = i;
                    Rgba32 pixel = im[c, i];

                    if (pixel.IsBlack())
                    {

                        for (int d = c; d < x + w; d++)
                        {
                            Rgba32 curPixel = im[d, i];

                            if (curPixel.IsBlack())
                                pixelCount++;
                            else
                                break;
                        }

                        if (pixelCount >= linePixels)
                            break;

                        pixelCount = 0;

                        for (int d = i; d < y + w; d++)
                        {
                            Rgba32 curPixel = im[c, d];

                            if (curPixel.IsBlack())
                                pixelCount++;
                            else
                                break;
                        }

                        if (pixelCount >= linePixels)
                            break;

                        pixelCount = 0;

                        do
                        {
                            Rgba32 curPixel = im[curX, curY];

                            if (pixel.IsBlack())
                                pixelCount++;
                            else
                                break;
                                                     
                            curX++;
                            curY++;

                        } while (curX > x && curY < y + w);

                        if (pixelCount >= linePixels)
                            break;

                        pixelCount = 0;
                        curX = c;
                        curY = i;

                        do
                        {
                            Rgba32 curPixel = im[curX, curY];

                            if (pixel.IsBlack())
                                pixelCount++;
                            else
                                break;

                            curX--;
                            curY++;

                        } while (curX > x && curY < y + w);

                        if (pixelCount >= linePixels)
                            break;

                        finish = true;
                        lineCount++;
                        break;
                    }

                }

                if (finish)
                    break;
            }

            return lineCount;
        }
    }
}
