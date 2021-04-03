using System.Threading.Tasks;
using System.Windows.Media;

namespace WinTaskKiller.Logic.Contract
{
    public interface IIconService
    {
        /// <summary>
        /// Loads the image source for the given filePath. Returns a default image source if not found.
        /// </summary>
        /// <param name="filePath">Filepath of the exe or icon.</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        ImageSource GetImageSourceFromFilePath(string filePath);
    }
}