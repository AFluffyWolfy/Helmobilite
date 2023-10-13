namespace Helmobilite.Utils
{
    public class FilesUtils
    {
        /// <summary>
        /// Sauvegarde une image dans sous-dossier donnée du dossier Img
        /// Attention, si le dossier n'existe pas il y aura une erreur
        /// </summary>
        /// <param name="file">Fichier à sauvegarder</param>
        /// <param name="directory">Sous dossier</param>
        /// <param name="fileName">Nom du fichier</param>
        /// <param name="webRootPath">Path du dossier wwwroot</param>
        /// <returns>Si tout va bien le chemin relatif vers l'image sinon null.</returns>
        public static async Task<string> SavePicture(IFormFile file, string directory, string fileName, string webRootPath)
        {
            try
            {
                var fileRelativePath = GetPath(directory, fileName + ".jpg");
                var fileAbsolutePath = Path.Combine(webRootPath, fileRelativePath);
                using (var fileStream = new FileStream(fileAbsolutePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return Path.Combine("/", fileRelativePath);
            }
            catch (IOException copyError)
            {
                Console.WriteLine(copyError.Message);
            }

            return null;
        }
        private static string GetPath(string dir, string picture)
        {
            return Path.Combine("XXXX", "Img", dir, picture);
        }
    }
}
