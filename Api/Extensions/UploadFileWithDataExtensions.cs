using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViewModel;

namespace Api.Extensions
{
    public static class UploadFileWithDataExtensions
    {
        public static async Task<KeyValuePair<IActionResult, FileDataViewModel>> GetUploadedFileWithData(this HttpRequest request)
        {
            var (key, value) = await GetUploadedFiles(request);
            return new KeyValuePair<IActionResult, FileDataViewModel>(key, value);
        }

        private static async Task<KeyValuePair<IActionResult, FileDataViewModel>> GetUploadedFiles(this HttpRequest request)
        {
            try
            {
                var fileCount = request.Form.Files.Count;

                if (fileCount > 1)
                    return new KeyValuePair<IActionResult, FileDataViewModel>(new BadRequestObjectResult("Multiple Files Not Allowed"), null);

                var files = await GetFiles(request.Form);
                return new KeyValuePair<IActionResult, FileDataViewModel>(new OkResult(), files);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<IActionResult, FileDataViewModel>(new BadRequestObjectResult(ex.Message), null);
            }
        }

        private static async Task<FileDataViewModel> GetFiles(IFormCollection formCollection)
        {
            var uploadDataViewModel = new FileDataViewModel
            {
                FormData = new List<KeyValuePair<string, string>>(),
                FileViewModel = new FileViewModel()
            };

            foreach (var item in formCollection.Files)
            {
                var data = new MemoryStream();
                await item.CopyToAsync(data);
                uploadDataViewModel.FileViewModel = new FileViewModel
                {
                    Name = item.FileName,
                    ContentType = item.ContentType,
                    Length = item.Length,
                    Data = data.ToArray()
                };
            }

            foreach (var formDataKey in formCollection.Keys)
            {
                uploadDataViewModel.FormData.Add(new KeyValuePair<string, string>(formDataKey, formCollection[formDataKey].FirstOrDefault()));
            }

            return uploadDataViewModel;
        }
    }
}
