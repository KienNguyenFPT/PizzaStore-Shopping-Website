using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.Operations;

namespace SignalRAssignment.Services
{
    public class UploadImagesService
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        public UploadImagesService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public string uploadImage(IFormFile url, string path)
        {
            string imageString = null;

            if (url != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, path);
                imageString = Guid.NewGuid().ToString() + "_" + url.FileName;
                string filePath = Path.Combine(uploadsFolder, imageString);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    url.CopyTo(fileStream);
                }
            }
            return imageString;
        }

        public List<string> uploadListImage(IFormFileCollection listFile, string path)
        {
            List<string> listImageString = new List<string>();
            string imageString = null;
            if (listFile != null)
            {
                foreach (IFormFile file in listFile)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, path);
                    imageString = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadsFolder, imageString);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    listImageString.Add(imageString);
                }

            }
            return listImageString;
        }
    }
}
