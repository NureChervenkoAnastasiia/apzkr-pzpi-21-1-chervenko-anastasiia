using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastifyAPI.Helpers;

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly string _backupFolderPath;

        public BackupController()
        {
            _backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
        }

        /// <summary>
        /// Runs a process asynchronously.
        /// </summary>
        /// <param name="fileName">The name of the file to be executed.</param>
        /// <param name="arguments">The arguments to pass to the process.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task RunProcessAsync(string fileName, string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                await process.WaitForExitAsync();
            }
        }

        /// <summary>
        /// Generates a backup file name backup_TastifyDB_ + date and time of backup.
        /// </summary>
        /// <returns>
        /// The generated backup file name.
        /// </returns>
        private string GetBackupFileName()
        {
            return $"backup_TastifyDB_{DateTime.Now:yyyyMMddHHmmss}";
        }

        /// <summary>
        /// Creates a backup folder asynchronously.
        /// </summary>
        /// <param name="backupFileName">The name of the backup folder.</param>
        /// <returns>
        /// The path of the created backup folder.
        /// </returns>
        private async Task<string> CreateBackupFolderAsync(string backupFileName)
        {
            var backupFolder = Path.Combine(_backupFolderPath, backupFileName);
            Directory.CreateDirectory(backupFolder);
            await Task.Delay(2000);
            return backupFolder;
        }

        /// <summary>
        /// Gets the latest BSON file in the backup folder.
        /// </summary>
        /// <param name="backupFolder">The path of the backup folder.</param>
        /// <returns>
        /// The path of the latest BSON file.
        /// </returns>
        private string GetLatestBsonFile(string backupFolder)
        {
            var bsonFiles = Directory.GetFiles(backupFolder, "*.bson");
            return bsonFiles.FirstOrDefault();
        }

        /// <summary>
        /// Creates a backup of the database.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return a status HTTP 200 OK with success message.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// The created backup file.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("export-data")]
        public async Task<IActionResult> CreateBackup()
        {
            try
            {
                var backupFileName = GetBackupFileName();
                var backupFolder = await CreateBackupFolderAsync(backupFileName);

                var processArgs = $"--db TastifyDB --out \"{backupFolder}\"";
                await RunProcessAsync("mongodump", processArgs);

                backupFolder = Path.Combine(backupFolder, "TastifyDB");
                var bsonFile = GetLatestBsonFile(backupFolder);

                if (string.IsNullOrEmpty(bsonFile))
                {
                    return StatusCode(500, $"Failed to create backup: Backup file not found.");
                }

                var fileContents = System.IO.File.ReadAllBytes(bsonFile);
                var contentType = "application/octet-stream";

                return File(fileContents, contentType, Path.GetFileName(bsonFile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to create backup: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the list of available backups.
        /// </summary>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return a status HTTP 200 OK with the list of avaliabe backups.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// The list of available backups.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("list")]
        public IActionResult GetBackupList()
        {
            try
            {
                if (!Directory.Exists(_backupFolderPath))
                {
                    return Ok(new string[] { });
                }

                var backupFolders = Directory.GetDirectories(_backupFolderPath)
                    .Select(Path.GetFileName)
                    .OrderByDescending(folder => folder);

                return Ok(backupFolders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to get backup list: {ex.Message}");
            }
        }

        /// <summary>
        /// Restores a database from a backup.
        /// </summary>
        /// <param name="backupFileName">The name of the backup file to restore.</param>
        /// <remarks>
        /// This endpoint requires Administrator role.
        /// If the operation is successful, it will return a status HTTP 200 OK with success message.
        /// If an error occurs during the operation, it will return a 500 Internal Server Error response with error message.
        /// </remarks>
        /// <returns>
        /// A status HTTP 200 OK with message indicating the result of the operation.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("import-data")]
        public async Task<IActionResult> RestoreBackup([FromForm] string backupFileName)
        {
            try
            {
                var backupFilePath = Path.Combine(_backupFolderPath, backupFileName);
                var processArgs = $"--drop --db TastifyDB \"{backupFilePath}\"";
                await RunProcessAsync("mongorestore", processArgs);

                return Ok("Database restored successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to restore database: {ex.Message}");
            }
        }
    }
}
