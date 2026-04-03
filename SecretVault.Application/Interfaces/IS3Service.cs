using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.Interfaces
{
    public interface IS3Service
    {
        Task<string> UploadStatementAsync(Guid accountId,string content,int month ,int year );
        Task<bool> StatementExistsAsync(Guid accountId, int month, int year);
        Task<string>GetPreSignedUrlAsync(Guid accountId, int month, int year);

    }
}
