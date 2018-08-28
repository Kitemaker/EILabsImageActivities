using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System;
using System.IO;

namespace EILabs.ImageActivities
{
    /// <summary>
    /// Activity to convert  image file type
    /// </summary>
    public class Convert_Image: CodeActivity
    {
        /// <summary>
        /// Image file path to be converted
        /// </summary>
        [Category("Input")]        
        [RequiredArgument]
        [Description("Image file path to be converted")]        
        public InArgument<string> ImagePath { get; set; }

        /// <summary>
        /// Folder path for the output files if left blank source file folder shall be used
        /// </summary>
        [Category("Input")]    
        [Description("Folder path for the output files if left blank source file folder shall be used")]
        public InArgument<string> OutputFolder { get; set; }


        /// <summary>
        /// Target Type 
        /// </summary>
        [Category("Input")]
        [RequiredArgument]
        [Description("Enter file Type to convert into supported format: jpg , png , bmp , gif")]
        public InArgument<string> Convert_To_Type { get; set; }


        /// <summary>
        /// Converted File Path 
        /// </summary>
        [Category("Output")]
        [RequiredArgument]
        [Description("Path of converted file")]
        public InArgument<string> ConvertedFilePath { get; set; }


        protected override void Execute(CodeActivityContext context) 
        {
            try
            {               
                string outFolder = OutputFolder.Get(context);
                string _imagePath =  ImagePath.Get(context);
                string _targetType = Convert_To_Type.Get(context).ToLower();
                string _destPath = _imagePath; // default value   

                if (System.IO.File.Exists(ImagePath.Get(context)))
                {
                    var sourceImg = System.Drawing.Image.FromFile(_imagePath);
                    if (Directory.Exists(outFolder) == false)
                    {
                        outFolder = Path.GetDirectoryName(_imagePath);
                    }
                   
                   _destPath = outFolder +"\\" + Path.GetFileNameWithoutExtension(_imagePath) + "." + _targetType;
                    

                     
                    switch (_targetType)
                    {
                        case "jpg":
                            sourceImg.Save(_destPath, ImageFormat.Jpeg);
                            break;
                        case "png":
                            sourceImg.Save(_destPath, ImageFormat.Png);
                            break;
                        case "bmp":
                            sourceImg.Save(_destPath, ImageFormat.Bmp);
                            break;
                        case "gif":
                            sourceImg.Save(_destPath, ImageFormat.Gif);
                            break;
                        default:
                            Console.WriteLine("Conversion Type Not Supported. Supported types are  jpg, png, gif, bmp");
                            break;
                    }
                    ConvertedFilePath.Set(context, _destPath);
                    Console.WriteLine("File saved as " + _destPath);
                    
                }
           
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Source" + ex.Source);
            } 

        }
    }
}
