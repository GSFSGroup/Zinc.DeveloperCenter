using System;
using System.Threading.Tasks;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.IntegrationTests.Web.Mothers
{
    internal static class TestData
    {
        internal static async Task InsertData(
            string tenantId,
            IApplicationRepository applicationRepository,
            IArchitectureDecisionRecordRepository adrRepository)
        {
            await applicationRepository.Save(new Domain.Model.Application(
                tenantId,
                "Zinc.Templates",
                "https://github.com/GSFSGroup/Zinc.Templates",
                "A template for new projects... .NET projects to be specific")).ConfigureAwait(false);

            await applicationRepository.Save(new Domain.Model.Application(
                tenantId,
                "Molybdenum.Earnings",
                "https://github.com/GSFSGroup/Molybdenum.Earnings",
                "A service to calculate earnings based on curves for insurance and insurance-like financial products.")).ConfigureAwait(false);

            // Zinc.Templates ADRs
            await adrRepository.Save(new Domain.Model.ArchitectureDecisionRecord(
                tenantId,
                "Zinc.Templates",
                "dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md",
                "Homer Simpson",
                DateTimeOffset.Parse("5/21/2018", null, System.Globalization.DateTimeStyles.AssumeLocal),
                Data.Migrations.EmbeddedResources.EmbeddedResource.Read("Migration_2022072101_TestData_adr_01.md"))).ConfigureAwait(false);

            await adrRepository.Save(new Domain.Model.ArchitectureDecisionRecord(
                tenantId,
                "Zinc.Templates",
                "dotnet-5.0/docs/RedLine/adr-0002-organize-solution-into-layers.md",
                "Bart Simpson",
                DateTimeOffset.Parse("6/19/2019", null, System.Globalization.DateTimeStyles.AssumeLocal),
                Data.Migrations.EmbeddedResources.EmbeddedResource.Read("Migration_2022072101_TestData_adr_02.md"))).ConfigureAwait(false);

            await adrRepository.Save(new Domain.Model.ArchitectureDecisionRecord(
                tenantId,
                "Zinc.Templates",
                "dotnet-5.0/docs/RedLine/adr-0003-implement-outbox-on-web-requests.md",
                "Lisa Simpson",
                DateTimeOffset.Parse("7/4/2020", null, System.Globalization.DateTimeStyles.AssumeLocal),
                Data.Migrations.EmbeddedResources.EmbeddedResource.Read("Migration_2022072101_TestData_adr_03.md"))).ConfigureAwait(false);

            // Molybdenum.Earnings ADRs
            await adrRepository.Save(new Domain.Model.ArchitectureDecisionRecord(
                tenantId,
                "Molybdenum.Earnings",
                "docs/App/adr-0001-event-sourcing.md",
                "Marge Simpson",
                DateTimeOffset.Parse("1/21/2022", null, System.Globalization.DateTimeStyles.AssumeLocal),
                Data.Migrations.EmbeddedResources.EmbeddedResource.Read("Migration_2022072101_TestData_adr_04.md"))).ConfigureAwait(false);

            await adrRepository.Save(new Domain.Model.ArchitectureDecisionRecord(
                tenantId,
                "Molybdenum.Earnings",
                "docs/App/adr-0002-select-an-infinite-scroll-library.md",
                "Barney Gumble",
                DateTimeOffset.Parse("2/13/2022", null, System.Globalization.DateTimeStyles.AssumeLocal),
                Data.Migrations.EmbeddedResources.EmbeddedResource.Read("Migration_2022072101_TestData_adr_05.md"))).ConfigureAwait(false);
        }
    }
}
