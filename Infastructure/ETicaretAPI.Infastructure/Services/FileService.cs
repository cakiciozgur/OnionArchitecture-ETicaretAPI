using ETicaretAPI.Application.Services;
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
    public class FileService : IFileService
    {
        readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        //private async Task<string> FileRenameAsync(string path, string fileName)
        //{
        //    return await Task.Run<string>(() =>
        //    {
        //        string oldName = Path.GetFileNameWithoutExtension(fileName);
        //        string extension = Path.GetExtension(fileName);
        //        string newFileName = $"{NameOperation.CharacterRegulatory(oldName)}{extension}";
        //        bool fileIsExists = false;
        //        int fileIndex = 0;
        //        do
        //        {
        //            if (File.Exists($"{path}\\{newFileName}"))
        //            {
        //                fileIsExists = true;
        //                fileIndex++;
        //                newFileName = $"{NameOperation.CharacterRegulatory(oldName + "-" + fileIndex)}{extension}";
        //            }
        //            else
        //            {
        //                fileIsExists = false;
        //            }
        //        } while (fileIsExists);

        //        return newFileName;
        //    });
        //}

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

        public async Task<bool> CopyToAsync(string fullPath, IFormFile file)
        {
            try
            {
                using FileStream fileStream = new(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();

                return true;
            }
            catch
            {
                //todo Log!
                throw;
            }
        }

        public async Task<List<(string fileName, string path)>> UploadAsync(string path, IFormFileCollection files)
        {
            string uploadPath = Path.Combine(_environment.WebRootPath, path);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            List<(string fileName, string path)> results = new();
            foreach (IFormFile file in files)
            {
                string fileNewName = await FileRenameAsync(uploadPath,file.FileName);
                string fullPath = Path.Combine(uploadPath, fileNewName);
                bool result = await CopyToAsync(fullPath, file);
                if (result)
                {
                    results.Add((fileNewName,fullPath));
                }
                else
                {
                    //todo eğer ki if geçerli değil ise burada dosyaların sunucuya yüklenirken hata alındığına dair uyarıcı bir exception oluşturulup fırlatılması gerekiyor
                }
            }
            return results;
        }
    }
}
