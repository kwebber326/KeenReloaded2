using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace KeenReloaded.Framework.Utilities
{
    public static class BitMapTool
    {
        public static Bitmap CombineBitmap(string[] files, int colCount, Color backColor)
        {
            //read all images into memory
            List<Bitmap> images = new List<Bitmap>();

            // Where each row starts to display the images
            List<int> YPosPerRow = new List<int>();
            // First row is known to be at Y position = 0 
            YPosPerRow.Add(0);

            Bitmap finalImage = null;
            try
            {
                int width = 0;
                int height = 0;
                int rowHeight = 0;
                int rowWidth = 0;

                // Calculate the number of rows required to display the files
                // per the colCount passed as argument..
                int rowCount = (files.Length / colCount) + 1;
                int index = 0;

                // Loop for each row
                for (int x = 0; x < rowCount; x++)
                {
                    // Loop for each column
                    for (int y = 0; y < colCount; y++)
                    {
                        // Should stop the loop if the bitmaps count is not
                        // exactly divisible for the colCount requested
                        if (index >= files.Length)
                            break;

                        //create a Bitmap from the file and add it to the list
                        Bitmap bitmap = new Bitmap(files[index]);

                        // recalculate the height/width for the current row
                        rowWidth += bitmap.Width;
                        rowHeight = bitmap.Height > rowHeight ? bitmap.Height : rowHeight;
                        images.Add(bitmap);
                        index++;
                    }
                    // Running height
                    height += rowHeight;
                    // Where to put in the Y axis the next row when merging
                    YPosPerRow.Add(height);

                    // Running width
                    width = (rowWidth > width ? rowWidth : width);

                    // reset for next loop
                    rowWidth = 0;
                    rowHeight = 0;
                }

                //create a bitmap to hold the combined image
                finalImage = new Bitmap(width, height);

                index = 0;
                //get a graphics object from the image so we can draw on it
                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(backColor);

                    // Again loop over rows/columns
                    for (int x = 0; x < rowCount; x++)
                    {
                        int offsetX = 0;
                        for (int y = 0; y < colCount; y++)
                        {
                            // Exit if not exactly divisible
                            if (index >= files.Length)
                                break;

                            using (Bitmap image = images[index])
                            {
                                g.DrawImage(image,
                                new Rectangle(offsetX, YPosPerRow[x], image.Width, image.Height));
                                offsetX += image.Width;
                            }
                            index++;
                        }
                    }
                }
                return finalImage;
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                    finalImage.Dispose();
                throw;
            }
            finally
            {
                //clean up memory
                foreach (Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }

        public static Bitmap DrawImageAtLocationWithDimensions(Image image, Rectangle area)
        {
            Bitmap bmp = new Bitmap(image, area.Size);

            return bmp;
        }

        public static Bitmap DrawBackgroundColor(Color color, Size dimensions)
        {
            Bitmap bmp = new Bitmap(dimensions.Width, dimensions.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(color);
            }
            return bmp;
        }

        public static Bitmap DrawImagesOnCanvas(Size canvas, Image backgroundImage, Image[] extraImages, Point[] locations)
        {
            Bitmap bmp = new Bitmap(canvas.Width, canvas.Height);
            try
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    if (backgroundImage != null)
                        g.DrawImage(backgroundImage, new Point(0, 0));

                    int count = extraImages.Length;
                    for (int i = 0; i < count; i++)
                    {
                        if (extraImages[i] != null)
                            g.DrawImage(extraImages[i], locations[i].X, locations[i].Y, extraImages[i].Width, extraImages[i].Height);
                    }
                }
                return bmp;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                bmp?.Dispose();
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }

        public static Bitmap DrawImagesOnCanvas(Size canvas, Image backgroundImage, Size backgroundImageSize, Image[] extraImages, Point[] locations)
        {
            Bitmap bmp = new Bitmap(canvas.Width, canvas.Height);
            try
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    if (backgroundImage != null)
                        g.DrawImage(backgroundImage, new Rectangle(new Point(0, 0), backgroundImageSize));

                    int count = extraImages.Length;
                    for (int i = 0; i < count; i++)
                    {
                        if (extraImages[i] != null)
                            g.DrawImage(extraImages[i], locations[i].X, locations[i].Y, extraImages[i].Width, extraImages[i].Height);
                    }
                }
                return bmp;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                bmp?.Dispose();
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }

        public static void WriteTextOnImage(Image originalImage, string text, Point locationOnImage, Font font, Brush color)
        {
            if (originalImage == null)
                return;
            try
            {
                using (Graphics g = Graphics.FromImage(originalImage))
                {
                    g.DrawString(text, font, color, locationOnImage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        public static Bitmap CropImage(Image image, Rectangle subSection)
        {
            var bitmap = new Bitmap(subSection.Width, subSection.Height);
            try
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(image, 0, 0, subSection, GraphicsUnit.Pixel);
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                bitmap?.Dispose();
                throw;
            }
            //finally
            //{
            //    image?.Dispose();
            //}
        }
    }
}
