using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PlanStack.Backend.Database.Repositories;

namespace PlanStack.Backend.Services
{
    public class BlueprintService
    {
        private readonly BlueprintBuildingStructureRepository _blueprintBuildingStructureRepository;

        public BlueprintService(
            BlueprintBuildingStructureRepository blueprintBuildingStructureRepository
        )
        {
            _blueprintBuildingStructureRepository = blueprintBuildingStructureRepository;
        }

        #region SaveImage
        public string SaveBuildingStructuresToBlueprint(string componentName, IFormFile imgFile)
        {
            try
            {


            }
            catch (Exception ex)
            {
                throw new IOException($"Error saving image '{componentName}'.", ex);
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