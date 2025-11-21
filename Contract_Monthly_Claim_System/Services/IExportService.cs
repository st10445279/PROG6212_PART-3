using Contract_Monthly_Claim_System.Models;

namespace Contract_Monthly_Claim_System.Services
{
    public interface IExportService
    {
        byte[] ExportClaimsToExcel(List<Claim> claims);
        byte[] ExportClaimsToPdf(List<Claim> claims);
    }
}
