using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OverLayApplicationSearch.Logic.Lib
{
    public static class ImageSourceExtension
    {
        /// <summary>
        /// Generates an imageSource out of an Icon.
        /// </summary>
        /// <param name="icon">icon</param>
        /// <returns>imageSource</returns>
        public static ImageSource GenerateImageSource(this Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        /// <summary>
        /// Generates an icon from the given filePath.
        /// </summary>
        /// <param name="path">path</param>
        /// <returns>icon</returns>
        public static Icon GenerateIcon(this string path)
        {
            return Icon.ExtractAssociatedIcon(path);
        }

        /// <summary>
        /// Generates an imageSource from the given filePath.
        /// </summary>
        /// <param name="path">path</param>
        /// <returns>imageSource</returns>
        public static ImageSource GenerateImageSource(this string path)
        {
            return GenerateImageSource(GenerateIcon(path));
        }
    }
}