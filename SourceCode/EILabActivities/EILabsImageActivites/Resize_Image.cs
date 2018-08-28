using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System;
using System.IO;

namespace EILabs.ImageActivities
{
    public class Resize_Image: CodeActivity
    {
        /// <summary>
        /// Image file path to be resized
        /// </summary>
        [Category("Input")]        
        [RequiredArgument]
        [Description("Image file path to be resized")]        
        public InArgument<string> ImagePath { get; set; }

        /// <summary>
        /// Folder path for the output files if left blank source file folder shall be used
        /// </summary>
        [Category("Input")]
        [DefaultValue("")]

        [Description("Folder path for the output files if left blank source file folder shall be used")]
        public InArgument<string> OutputFolder { get; set; } = "";


        /// <summary>
        /// Horizontal Res0lution 
        /// </summary>
        [Category("Input")]
        [DefaultValue(100)]
        [Description("Enter Horizontal Res0lution")]
        public InArgument<int> Size_X { get; set; } = 100;

        /// <summary>
        /// Vertical Resolution 
        /// </summary>
        [Category("Input")]
        [DefaultValue(100)]
        [Description("Enter Vertical Resolution")]
        public InArgument<int> Size_Y { get; set; } = 100;


        /// <summary>
        /// "True" for percentage Reduction "False" for Absolute values
        /// </summary>
        [Category("Input")]
        [RequiredArgument]
        [DefaultValue(false)]
        [Description("Enter True to use Percentage Reduction false for absolute reduction")]
        public InArgument<bool> Percent_Reduction { get; set; } = false;

        /// <summary>
        /// Percentage Reduction
        /// </summary>
        [Category("Input")]
        [DefaultValue(100)]
        [Description("Enter Value of Percentage Reduction")]
        public InArgument<int> Percentage_Value { get; set; } = 100;


        /// <summary>
        /// Resized Image path
        /// </summary>
        [Category("Output")]  
        [Description("Resized Image path")]
        public InArgument<string> ResizedImagePath { get; set; }



        protected override void Execute(CodeActivityContext context) 
        {
            try
            {
                int sizex = Size_X.Get(context);
                int sizey = Size_Y.Get(context);
                string outFolder = OutputFolder.Get(context);
                string _imagePath = ImagePath.Get(context);
                string destPath = _imagePath; // default value
                int percentage = Percentage_Value.Get(context);
                bool usePercentage = Percent_Reduction.Get(context);


                if (System.IO.File.Exists(ImagePath.Get(context)))
                {
                    if (Directory.Exists(outFolder) == false)
                    {
                        outFolder = Path.GetDirectoryName(_imagePath);
                    }
                    
                    var sourceImg = System.Drawing.Image.FromFile(_imagePath);          
                    
                   destPath = outFolder + "\\" + System.IO.Path.GetFileNameWithoutExtension(_imagePath)
                        + "_" + sizex.ToString() + "x" + sizey.ToString() + System.IO.Path.GetExtension(_imagePath);

                    // Get the size 
                    Console.WriteLine("Resizing file  " + _imagePath);
                    System.Drawing.Rectangle destRect ;
                    Bitmap destImage;

                    if (usePercentage != true)
                    {
                        destRect = new System.Drawing.Rectangle(0, 0, sizex, sizey);
                        destImage = new Bitmap(sizex, sizey);
                        destPath = outFolder + "\\" + System.IO.Path.GetFileNameWithoutExtension(_imagePath)
                       + "_" + sizex.ToString() + "x" + sizey.ToString() + System.IO.Path.GetExtension(_imagePath);
                    }
                    else
                    {
                        if (percentage > 0 && percentage <= 100)
                        {
                            int newsizex = Convert.ToInt32(sourceImg.Width * percentage / 100);
                            int newsizey = Convert.ToInt32(sourceImg.Height * percentage / 100);
                            destRect = new System.Drawing.Rectangle(0, 0, newsizex, newsizey);
                            destImage = new Bitmap(newsizex, newsizey);
                            destPath = outFolder + "\\" + System.IO.Path.GetFileNameWithoutExtension(_imagePath)
                                + "_" + newsizex.ToString() + "x" + newsizey.ToString() + System.IO.Path.GetExtension(_imagePath);

                        }
                        else
                        {
                            Console.WriteLine("Wroing percentage is defined taking 100% ");
                            destRect = new System.Drawing.Rectangle(0, 0, sizex, sizey);
                            destImage = new Bitmap(sizex, sizey);
                            destImage = new Bitmap(Convert.ToInt32(sizex * percentage / 100), Convert.ToInt32(sizey * percentage / 100));
                            destPath = outFolder + "\\" + System.IO.Path.GetFileName(_imagePath);

                        }

                    }
                   
                    destImage.SetResolution(sourceImg.HorizontalResolution, sourceImg.VerticalResolution);

                    using (var graphics = Graphics.FromImage(destImage))
                    {
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        using (var wrapMode = new ImageAttributes())
                        {
                            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                            graphics.DrawImage(sourceImg, destRect, 0, 0, sourceImg.Width, sourceImg.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }

                    destImage.Save(destPath);
                    destImage.Dispose();
                    sourceImg.Dispose();
                    Console.WriteLine("Saved resized image file " + destPath);
                    ResizedImagePath.Set(context, destPath);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Source" + ex.Source);
            } 

        }
    }
}
