using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverLayApplicationSearch.Contract.Business.Service
{
    public interface IGoogleService : IService
    {
        /// <summary>
        /// Searches for the text via default browser and google.
        /// </summary>
        /// <param name="text">text</param>
        void Search(string text);
    }
}
