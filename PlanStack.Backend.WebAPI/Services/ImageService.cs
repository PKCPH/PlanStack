using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace PlanStack.Backend.WebAPI.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment environment;

        public ImageService(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        #region SaveImage
        public string SaveImage(string componentName, IFormFile imgFile)
        {
            try
            {
                var rootPath = this.environment.ContentRootPath;

                var path = Path.Combine(rootPath, "Uploads");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var ext = Path.GetExtension(imgFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg", ".avif" };
                if (!allowedExtensions.Contains(ext))
                {
                    string msg = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
                    throw new IOException(msg);
                }

                var newFileName = componentName + ext;
                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imgFile.CopyTo(stream);
                stream.Close();
                return fileWithPath;

            }
            catch (Exception ex)
            {
                throw new IOException($"Error saving image '{componentName}'. {ex.Message}", ex);
            }
        }
        #endregion

        #region DeleteImage
        public void DeleteImage(string imgPath)
        {
            try
            {
                var folderPath = "Uploads\\";

                var contentPath = this.environment.ContentRootPath;
                var path = Path.Combine(contentPath, folderPath, imgPath);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Error deleting image '{imgPath}'.", ex);
            }
        }
        #endregion
    }
}