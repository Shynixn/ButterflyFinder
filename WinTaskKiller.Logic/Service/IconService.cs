using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WinTaskKiller.Logic.Contract;

namespace WinTaskKiller.Logic.Service
{
    public class IconService : IIconService
    {
        /// <summary>
        /// Loads the image source for the given filePath. Returns a default image source if not found.
        /// </summary>
        /// <param name="filePath">Filepath of the exe or icon.</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        public ImageSource GetImageSourceFromFilePath(string filePath)
        {
            Icon icon;

            try
            {
                icon = Icon.ExtractAssociatedIcon(filePath);
                if (icon == null)
                {
                    throw new ArgumentException();
                }
            }
            catch (Exception e)
            {
                icon = Icon.ExtractAssociatedIcon(Environment.GetFolderPath(Environment.SpecialFolder.Windows) +
                                                  "//notepad.exe");
            }

            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }
    }
}