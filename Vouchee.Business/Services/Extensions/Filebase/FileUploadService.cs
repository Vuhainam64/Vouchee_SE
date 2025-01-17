using Microsoft.AspNetCore.Http;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Options;
using Vouchee.Data.Helpers;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Other;

namespace Vouchee.Business.Services.Extensions.Filebase
{
    public class FileUploadService : IFileUploadService
    {
        private readonly FirebaseSettings _firebaseSettings;

        public FileUploadService(IOptions<FirebaseSettings> firebaseSettings)
        {
            _firebaseSettings = firebaseSettings.Value;
        }

        public async Task<string> UploadFileToFirebase(IFormFile file, string uid, StoragePathEnum storagePathEnum)
        {
            try
            {
                var tokenDescriptor = new Dictionary<string, object>()
                {
                    {"permission", "allow" }
                };

                var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));
                string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid, tokenDescriptor);

                //var token = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.Email, _firebaseSettings.Password);
                var token = await auth.SignInWithCustomTokenAsync(storageToken);

                var task = new FirebaseStorage(_firebaseSettings.Bucket,
                                                    new FirebaseStorageOptions
                                                    {
                                                        AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                                                        ThrowOnCancel = true,
                                                    })
                                                    .Child("FILE")
                                                    .Child(storagePathEnum.ToString())
                                                    .Child(file.FileName)
                                                    .PutAsync(file.OpenReadStream());

                var downloadUrl = await task;

                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);

                return downloadUrl.ToString();
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new InvalidDataException("Lỗi khi upload hình ảnh lên firebase");
            }
        }

        public async Task<string> UploadImageToFirebase(IFormFile file, string uid, StoragePathEnum storagePathEnum)
        {
            try
            {
                var tokenDescriptor = new Dictionary<string, object>()
                {
                    {"permission", "allow" }
                };

                var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));
                string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid, tokenDescriptor);

                //var token = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.Email, _firebaseSettings.Password);
                var token = await auth.SignInWithCustomTokenAsync(storageToken);

                var task = new FirebaseStorage(_firebaseSettings.Bucket,
                                                    new FirebaseStorageOptions
                                                    {
                                                        AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                                                        ThrowOnCancel = true,
                                                    })
                                                    .Child("IMAGE")
                                                    .Child(storagePathEnum.ToString())
                                                    .Child(file.FileName)
                                                    .PutAsync(file.OpenReadStream());

                var downloadUrl = await task;

                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);

                return downloadUrl.ToString();
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new InvalidDataException("Lỗi khi upload hình ảnh lên firebase");
            }
        }

        public async Task<string> UploadVideoToFirebase(IFormFile file, string uid, StoragePathEnum storagePathEnum)
        {
            try
            {
                var tokenDescriptor = new Dictionary<string, object>()
                {
                    {"permission", "allow" }
                };

                var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));
                string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid, tokenDescriptor);

                //var token = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.Email, _firebaseSettings.Password);
                var token = await auth.SignInWithCustomTokenAsync(storageToken);

                var task = new FirebaseStorage(_firebaseSettings.Bucket,
                                                    new FirebaseStorageOptions
                                                    {
                                                        AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                                                        ThrowOnCancel = true,
                                                    })
                                                    .Child("VIDEO")
                                                    .Child(storagePathEnum.ToString())
                                                    .Child(file.FileName)
                                                    .PutAsync(file.OpenReadStream());

                var downloadUrl = await task;

                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);

                return downloadUrl.ToString();
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new InvalidDataException("Lỗi khi upload hình ảnh lên firebase");
            }
        }
    }
}
