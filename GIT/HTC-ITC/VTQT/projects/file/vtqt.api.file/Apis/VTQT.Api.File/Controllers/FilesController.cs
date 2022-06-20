using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Api.File.FormFile;
using VTQT.Core;
using VTQT.Services.File;
using VTQT.SharedMvc.Master;
using VTQT.Web.Framework.Controllers;

namespace VTQT.Api.File.Controllers
{
    [Route("file")]
    [ApiController]
    public class FilesController : AdminApiController
    {
        #region Fields
        private readonly IFilesService _fileService;

        #endregion

        #region Ctor

        public FilesController(
            IFilesService fileService)
        {
            _fileService = fileService;
        }

        #endregion

        #region Download File

        [Route("download-file")]
        [HttpGet]
        public async Task<IActionResult> DownloadAsync([Required] string subidFile)
        {
            if (subidFile is null)
            {
                throw new ArgumentNullException(nameof(subidFile));
            }

            var check = await _fileService.GetByIdAsync(subidFile);

            if (check is null || string.IsNullOrEmpty(check.FileName) || string.IsNullOrEmpty(check.Path))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.File"))
                });
            var fileName = check.FileName;
            //Build the File Path.
            string path = CommonHelper.MapPath(check.Path + "/" + check.FileName);

            //Read the File data into Byte Array.
            byte[] bytes = await System.IO.File.ReadAllBytesAsync(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }

        #endregion

        #region Upload File

        [Route("create-image")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> CreateImage([FromForm] List<IFormFile> filesadd)
        {
            if (filesadd == null || filesadd.Count == 0)

                return Ok(new XBaseResult
                {
                    data = " No image selected ",
                    success = false
                });
            var listEntity = new List<Core.Domain.File.Files>();

            string createFolderDate = DateTime.Now.ToString("yyyy/MM/dd");
            var path = CommonHelper.MapPath(@"/wwwroot/uploads/" + createFolderDate + "");
            CreateFolder(path);
            if (path == null)
                path = "image";
            var filePaths = new List<string>();
            string sql = "";
            foreach (var formFile in filesadd)
            {
                if (!FormFileExtensions.IsImage(formFile) && !FormFileExtensions.IsExcel(formFile) && !FormFileExtensions.IsWord(formFile) && !FormFileExtensions.IsZipRar(formFile))
                {
                    if (sql.Length == 0)
                        sql = $"File width name {formFile.FileName} is not type word, excel, zip or image, rar";
                    else
                        sql = sql + $" .File width name {formFile.FileName} is not type word, excel, zip or image, rar";
                }
                else
                {
                    if (formFile.Length > 0 && formFile.Length <= 250000000)
                    {
                        var filePath = CommonHelper.MapPath(path);
                        filePaths.Add(filePath);
                        var randomname = DateTime.Now.ToFileTime() + Path.GetRandomFileName().Replace(".","") + Path.GetExtension(formFile.FileName);
                        var fileNameWithPath = string.Concat(filePath, "\\", randomname);
                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                            var tem = new SharedMvc.Master.Models.FileModel();
                            tem.FileName = randomname;
                            tem.Path = @"/wwwroot/uploads/" + createFolderDate + "";
                            tem.Extension = GetExtension(randomname);
                            tem.MimeType = formFile.ContentType;
                            tem.Size = formFile.Length;
                            var entity = tem.ToEntity();
                            listEntity.Add(entity);
                        }
                    }
                    else
                    {
                        sql = sql + $" The file width name {formFile.FileName}  must be > 0 and <25M ! ";
                    }
                }

            }
            var listRes= new List<Core.Domain.File.Files>();
            if (listEntity.Count() > 0)
            {
                await _fileService.InsertRangeAsync(listEntity);
                foreach (var item in listEntity)
                {
                    var tem = new SharedMvc.Master.Models.FileModel();
                    tem.FileName = item.FileName;
                    tem.Path = item.Path;
                    var entity = tem.ToEntity();
                    listRes.Add(entity);
                }
            }
            return Ok(new
            {
                error = sql,
                result = true,
                res=listRes,
            });
        }

        public void CreateFolder(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine(" Path already exists ! ");
                }
                else
                    Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }


        private static string GetExtension(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Unknown";
            var Cut = name.Split(".");
            return Cut[Cut.Length - 1];
        }
        #endregion

        #region Unities

        public static bool IsZipValid(string path)
        {
            try
            {
                using (var zipFile = ZipFile.OpenRead(path))
                {
                    var entries = zipFile.Entries;
                    return true;
                }
            }
            catch (InvalidDataException)
            {
                return false;
            }
        }

        #endregion
    }
}