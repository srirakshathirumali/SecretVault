using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using SecretVault.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Infrastructure.Services
{
    public class S3Service : IS3Service
    {
        private readonly IConfiguration _configuration;
        private readonly IMinioClient _minioClient;
        private readonly string bucketName;
        public S3Service(IConfiguration configuration, IMinioClient minioClient)
        {
             _configuration = configuration;
            _minioClient = minioClient;
            bucketName = _configuration["S3:Bucket"]!;
        }
        public async Task<string> GetPreSignedUrlAsync(Guid accountId, int month, int year)
        {
            var key = GetS3Key(accountId, month, year);

            var request = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(key)
            .WithExpiry(300);

            var response = await _minioClient.PresignedGetObjectAsync(request);
            return response;
        }

        public async Task<bool> StatementExistsAsync(Guid accountId, int month, int year)
        {
            try
            {
                var key = GetS3Key(accountId, month, year);

                var args = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(GetS3Key(accountId, year, month));

                await _minioClient.StatObjectAsync(args);
                return true;
            }
            catch (ObjectNotFoundException)
            {
                return false;
            }
            catch (BucketNotFoundException)
            {
                return false;
            }
        }

        public async Task<string> UploadStatementAsync(Guid accountId, string content, int month, int year)
        {
            await EnsureBucketExistsAsync();

            var key= GetS3Key(accountId,month,year);
            var bytes = Encoding.UTF8.GetBytes(content);

            using var stream = new MemoryStream(bytes);

            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(key)
                .WithStreamData(stream)
                .WithObjectSize(bytes.Length)
                .WithContentType("text/plain");

            await _minioClient.PutObjectAsync(args);
            return key;

        }
        private async Task EnsureBucketExistsAsync()
        {
            var existsArgs = new BucketExistsArgs()
            .WithBucket(bucketName);

            var exists = await _minioClient.BucketExistsAsync(existsArgs);

            if (!exists)
            {
                var makeArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);

                await _minioClient.MakeBucketAsync(makeArgs);
            }
        }

        private string GetS3Key(Guid accountId, int month, int year)
        {
            return $"statements/{accountId}/{year}-{month:D2}.txt";
        }
    }
}
