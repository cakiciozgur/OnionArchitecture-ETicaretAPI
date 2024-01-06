using ETicaretAPI.Infastructure.StaticServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infastructure.Services
{
    public class FileService 
    {
        //todo local storage ve diğer storage servisleri için genel bir yapıya büründürülecek
        private static async Task<string> FileRenameAsync(string path, string fileName, int? counter = 0)
        {
            var fileExtension = Path.GetExtension(fileName);
            var oldName = Path.GetFileNameWithoutExtension(fileName);
            var newName = NameOperation.CharacterRegulatory(oldName);

            if (counter > 0) newName += "-" + counter;

            bool exists = await Task.Run(() => File.Exists($"{path}\\{newName}{fileExtension}"));

            if (exists)
            {
                counter++;
                await FileRenameAsync(path, fileName, counter);
            }

            return newName + fileExtension;
        }
    }
}
