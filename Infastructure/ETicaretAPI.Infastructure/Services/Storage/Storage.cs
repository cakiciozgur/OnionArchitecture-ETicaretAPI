using ETicaretAPI.Infastructure.StaticServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infastructure.Services.Storage
{
    public class Storage
    {
        protected delegate bool HasFile(string pathOrContainerName, string fileName);
        protected async Task<string> FileRenameAsync(string pathOrContainerName, string fileName,HasFile hasFileMethod, int? counter = 0)
        {
            var fileExtension = Path.GetExtension(fileName);
            var oldName = Path.GetFileNameWithoutExtension(fileName);
            var newName = NameOperation.CharacterRegulatory(oldName);

            if (counter > 0) newName += "-" + counter;

            if (hasFileMethod(pathOrContainerName, fileName))
            {
                counter++;
                await FileRenameAsync(pathOrContainerName, fileName, hasFileMethod, counter);
            }

            return newName + fileExtension;
        }
    }
}
