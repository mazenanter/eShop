namespace eShop.Helpers
{
    public class ImageHelper
    {
        public async static Task<string> UploadImage(IFormFile imageUrl,string folderName)
        {
            List<string> AllowEx = new List<string> { ".png",".jpg",".jpeg"};
            var MaxLength = 5 * 1024 * 1024;
            string extension = Path.GetExtension(imageUrl.FileName).ToLower();
            if (!AllowEx.Contains(extension))
                throw new Exception("Invalid image extension. Only .png, .jpg, .jpeg are allowed.");
            if (imageUrl.Length > MaxLength)
                throw new Exception("File size exceeds the 5MB limit.");
           
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName);
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageUrl.CopyToAsync(stream);
                }
               
            
            return $"/images/{folderName}/{fileName}";
        }
        public static void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            var localPath = imageUrl.TrimStart('/');
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", localPath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
