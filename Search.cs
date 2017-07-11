using System;
using System.Collections.Generic;

using BotUtilities.ImageProcessing.Types;

namespace BotUtilities.ImageProcessing
{
    public static class Search
    {
        public static unsafe Spot Pixel(ImageData image, PixelData pixel, byte variation = 0)
        {
            fixed (byte* b = image.RawData)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        PixelData curr = *(PixelData*)(b + (y * image.Stride + x * 3));
                        if ((pixel.R >= curr.R - variation && pixel.R <= curr.R + variation) && (pixel.G >= curr.G - variation && pixel.G <= curr.G + variation) && (pixel.B >= curr.B - variation && pixel.B <= curr.B + variation))
                                return new Spot(x, y);
                    }
                }
            }
            return Spot.Empty;
        }

        public static unsafe Spot PixelArea(ImageData image, PixelData pixel, Boundary bounds, byte variation = 0)
        {
            fixed (byte* b = image.RawData)
            {
                for (int y = bounds.Top; y < bounds.Bottom; y++)
                {
                    for (int x = bounds.Left; x < bounds.Right; x++)
                    {
                        PixelData curr = *(PixelData*)(b + (y * image.Stride + x * 3));
                        if ((pixel.R >= curr.R - variation && pixel.R <= curr.R + variation) && (pixel.G >= curr.G - variation && pixel.G <= curr.G + variation) && (pixel.B >= curr.B - variation && pixel.B <= curr.B + variation))
                            return new Spot(x, y);
                    }
                }
            }
            return Spot.Empty;
        }

        public static unsafe List<Spot> PixelList(ImageData image, PixelData pixel, byte variation = 0)
        {
            List<Spot> list = new List<Spot>();

            fixed (byte* b = image.RawData)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        PixelData curr = *(PixelData*)(b + (y * image.Stride + x * 3));
                        if ((pixel.R >= curr.R - variation && pixel.R <= curr.R + variation) && (pixel.G >= curr.G - variation && pixel.G <= curr.G + variation) && (pixel.B >= curr.B - variation && pixel.B <= curr.B + variation))
                            list.Add(new Spot(x, y));
                    }
                }
            }

            return list;
        }

        public static unsafe Spot PixelNextTo(ImageData image, PixelData pixel, Spot pos, byte variation = 0)
        {
            fixed (byte* b = image.RawData)
            {
                for (int radius = 0; ; radius++)
                {
                    if (pos.Y + radius >= image.Height && pos.Y - radius < 0 && pos.X + radius >= image.Width &&
                        pos.X - radius < 0)
                        break;

                    for (int nextX = pos.X - radius; nextX <= pos.X + radius; nextX++)
                    {
                        if (pos.Y - radius >= 0)
                        {
                            PixelData curr = *(PixelData*)(b + ((pos.Y - radius) * image.Stride + nextX * 3));
                            if (nextX >= 0 && nextX < image.Width &&
                                ((pixel.R >= curr.R - variation && pixel.R <= curr.R + variation) && (pixel.G >= curr.G - variation && pixel.G <= curr.G + variation) && (pixel.B >= curr.B - variation && pixel.B <= curr.B + variation)))
                                return new Spot(nextX, pos.Y - radius);
                        }
                        if (pos.Y + radius < image.Height)
                        {
                            PixelData curr = *(PixelData*)(b + ((pos.Y + radius) * image.Stride + nextX * 3));
                            if (nextX >= 0 && nextX < image.Width &&
                                ((pixel.R >= curr.R - variation && pixel.R <= curr.R + variation) && (pixel.G >= curr.G - variation && pixel.G <= curr.G + variation) && (pixel.B >= curr.B - variation && pixel.B <= curr.B + variation)))
                                return new Spot(nextX, pos.Y + radius);
                        }
                    }

                    for (int nextY = pos.Y - radius; nextY <= pos.Y + radius; nextY++)
                    {
                        if (pos.X - radius >= 0)
                        {
                            PixelData curr = *(PixelData*)(b + ((nextY) * image.Stride + (pos.X - radius) * 3));
                            if (nextY >= 0 && nextY < image.Height &&
                                ((pixel.R >= curr.R - variation && pixel.R <= curr.R + variation) && (pixel.G >= curr.G - variation && pixel.G <= curr.G + variation) && (pixel.B >= curr.B - variation && pixel.B <= curr.B + variation)))
                                return new Spot(pos.X - radius, nextY);
                        }
                        if (pos.X + radius < image.Width)
                        {
                            PixelData curr = *(PixelData*)(b + ((nextY) * image.Stride + (pos.X + radius) * 3));
                            if (nextY >= 0 && nextY < image.Height &&
                                ((pixel.R >= curr.R - variation && pixel.R <= curr.R + variation) && (pixel.G >= curr.G - variation && pixel.G <= curr.G + variation) && (pixel.B >= curr.B - variation && pixel.B <= curr.B + variation)))
                                return new Spot(pos.X + radius, nextY);
                        }
                    }
                }
            }
            return Spot.Empty;
        }

        public static unsafe Spot Image(ImageData source, ImageData search, byte variation = 0, MatchMode mode = MatchMode.TopLeft)
        {
            fixed (byte* src = source.RawData)
            {
                fixed (byte* sec = search.RawData)
                {
                    for (int y = 0; y < source.Height - search.Height; y++)
                    {
                        for (int x = 0; x < source.Width - search.Width; x++)
                        {
                            for (int smally = 0; smally < search.Height; smally++)
                            {
                                for (int smallx = 0; smallx < search.Width; smallx++)
                                {
                                    PixelData sourcePixel = *(PixelData*)(src + ((y + smally) * source.Stride + (x + smallx) * 3));
                                    PixelData searchPixel = *(PixelData*)(sec + smally * search.Stride + smallx * 3);

                                    if ((searchPixel.R < sourcePixel.R - variation || searchPixel.R > sourcePixel.R + variation) || (searchPixel.G < sourcePixel.G - variation || searchPixel.G > sourcePixel.G + variation) || (searchPixel.B < sourcePixel.B - variation || searchPixel.B > sourcePixel.B + variation))
                                        goto SecondLoop;
                                }
                            }

                            switch (mode)
                            {
                                case MatchMode.BottomLeft: return new Spot(x, y + search.Height);
                                case MatchMode.BottomRight: return new Spot(x + search.Width, y + search.Height);
                                case MatchMode.Mid: return new Spot(x + search.Mid.X, y + search.Mid.Y);
                                case MatchMode.TopLeft: return new Spot(x, y);
                                case MatchMode.TopRight: return new Spot(x + search.Width, y);
                                case MatchMode.None: return new Spot(x, y);
                                default: return new Spot(x, y);
                            }

                            SecondLoop:;
                        }
                    }

                    return Spot.Empty;
                }
            }
        }

        public static unsafe Spot ImageArea(ImageData source, ImageData search, Boundary bounds, byte variation = 0, MatchMode mode = MatchMode.TopLeft)
        {
            fixed (byte* src = source.RawData)
            {
                fixed (byte* sec = search.RawData)
                {
                    for (int y = bounds.Top; y < bounds.Bottom - search.Height; y++)
                    {
                        for (int x = bounds.Left; x < bounds.Right - search.Width; x++)
                        {
                            for (int smally = 0; smally < search.Height; smally++)
                            {
                                for (int smallx = 0; smallx < search.Width; smallx++)
                                {
                                    PixelData sourcePixel = *(PixelData*)(src + ((y + smally) * source.Stride + (x + smallx) * 3));
                                    PixelData searchPixel = *(PixelData*)(sec + smally * search.Stride + smallx * 3);

                                    if ((searchPixel.R < sourcePixel.R - variation || searchPixel.R > sourcePixel.R + variation) || (searchPixel.G < sourcePixel.G - variation || searchPixel.G > sourcePixel.G + variation) || (searchPixel.B < sourcePixel.B - variation || searchPixel.B > sourcePixel.B + variation))
                                        goto SecondLoop;
                                }
                            }

                            switch (mode)
                            {
                                case MatchMode.BottomLeft: return new Spot(x, y + search.Height);
                                case MatchMode.BottomRight: return new Spot(x + search.Width, y + search.Height);
                                case MatchMode.Mid: return new Spot(x + search.Mid.X, y + search.Mid.Y);
                                case MatchMode.TopLeft: return new Spot(x, y);
                                case MatchMode.TopRight: return new Spot(x + search.Width, y);
                                case MatchMode.None: return new Spot(x, y);
                                default: return new Spot(x, y);
                            }

                        SecondLoop:;
                        }
                    }

                    return Spot.Empty;
                }
            }
        }

        public static unsafe List<Spot> ImageList(ImageData source, ImageData search, byte variation = 0, MatchMode mode = MatchMode.TopLeft)
        {
            List<Spot> list = new List<Spot>();

            fixed (byte* src = source.RawData)
            {
                fixed (byte* sec = search.RawData)
                {
                    for (int y = 0; y < source.Height - search.Height; y++)
                    {
                        for (int x = 0; x < source.Width - search.Width; x++)
                        {
                            for (int smally = 0; smally < search.Height; smally++)
                            {
                                for (int smallx = 0; smallx < search.Width; smallx++)
                                {
                                    PixelData sourcePixel = *(PixelData*)(src + ((y + smally) * source.Stride + (x + smallx) * 3));
                                    PixelData searchPixel = *(PixelData*)(sec + smally * search.Stride + smallx * 3);

                                    if ((searchPixel.R < sourcePixel.R - variation || searchPixel.R > sourcePixel.R + variation) || (searchPixel.G < sourcePixel.G - variation || searchPixel.G > sourcePixel.G + variation) || (searchPixel.B < sourcePixel.B - variation || searchPixel.B > sourcePixel.B + variation))
                                        goto SecondLoop;
                                }
                            }

                            switch (mode)
                            {
                                case MatchMode.BottomLeft: list.Add(new Spot(x, y + search.Height)); break;
                                case MatchMode.BottomRight: list.Add(new Spot(x + search.Width, y + search.Height)); break;
                                case MatchMode.Mid: list.Add(new Spot(x + search.Mid.X, y + search.Mid.Y)); break;
                                case MatchMode.TopLeft: list.Add(new Spot(x, y)); break;
                                case MatchMode.TopRight: list.Add(new Spot(x + search.Width, y)); break;
                                case MatchMode.None: list.Add(new Spot(x, y)); break;
                                default: list.Add(new Spot(x, y)); break;
                            }

                        SecondLoop:;
                        }
                    }

                    return list;
                }
            }
        }

        public static Spot ImageNextTo(ImageData source, ImageData search, Spot pos, byte variation = 0, MatchMode mode = MatchMode.TopLeft)
        {
            List<Spot> list = ImageList(source, search, variation, mode);

            if (list.Count == 0)
                return Spot.Empty;

            if (list.Count == 1)
                return list[0];

            Spot bestMatch = Spot.Empty;

            for (int i = 0; i < list.Count; i++)
            {
                if (bestMatch.IsEmpty)
                {
                    bestMatch = list[i];
                    continue;
                }

                if ((Math.Abs(bestMatch.X - pos.X) > Math.Abs(list[i].X - pos.X)) &&
                    (Math.Abs(bestMatch.Y - pos.Y) > Math.Abs(list[i].Y - pos.Y)))
                    bestMatch = list[i];
            }

            return bestMatch;
        }
    }
}
