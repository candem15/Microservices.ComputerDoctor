using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CatalogService.Api.Core.Domain;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace CatalogService.Api.Infrastructure.Contexts
{
    public class CatalogContextSeed
    {
        public async Task SeedAsync(CatalogContext context, IWebHostEnvironment environment, ILogger<CatalogContextSeed> logger)
        {
            var policy = Policy.Handle<SqlException>()
                            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3),
                                onRetry: (exception, timeSpan, retry, ctx) =>
                                {
                                    logger.LogWarning(exception, "Exeception Type: {exception} occurred!");
                                });
            var setupDirPath = Path.Combine(environment.ContentRootPath, "Infrastructure", "Setup", "SeedFiles");
            var picturePath = "Pics";

            await policy.ExecuteAsync(() => ProcessSeeding(context, setupDirPath, picturePath));
        }

        private async Task ProcessSeeding(CatalogContext context, string setupDirPath, string picturePath)
        {
            if (!context.CatalogBrands.Any())
            {
                await context.CatalogBrands.AddRangeAsync(GetCatalogBrands());
                await context.SaveChangesAsync();
            }
            if (!context.CatalogTypes.Any())
            {
                await context.CatalogTypes.AddRangeAsync(GetCatalogTypes());
                await context.SaveChangesAsync();
            }
            if (!context.CatalogItems.Any())
            {
                await context.CatalogItems.AddRangeAsync(GetCatalogItems());
                await context.SaveChangesAsync();
                //GetCatalogItemsPictures(setupDirPath, picturePath);
            }
        }

        private IEnumerable<CatalogType> GetCatalogTypes()
        {
            return new List<CatalogType>()
            {
                new CatalogType {Type="Memory"},
                new CatalogType {Type="Power Supply"},
                new CatalogType {Type="Gpu"},
            };
        }

        private IEnumerable<CatalogBrand> GetCatalogBrands()
        {
            return new List<CatalogBrand>()
            {
                new CatalogBrand {Brand="CORSAIR"},
                new CatalogBrand {Brand="G.SKILL"},
                new CatalogBrand {Brand="Thermaltake"},
                new CatalogBrand {Brand="EVGA"},
                new CatalogBrand {Brand="MSI"},
                new CatalogBrand {Brand="Acer"},
                new CatalogBrand {Brand="GIGABYTE"},
            };
        }


        private IEnumerable<CatalogItem> GetCatalogItems()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem {CatalogTypeId=1,CatalogBrandId=2,AvailableStock=100,Description="G.SKILL TridentZ RGB Series 32GB (2 x 16GB) 288-Pin PC RAM DDR4 3200 (PC4 25600) Intel XMP 2.0 Desktop Memory Model F4-3200C16D-32GTZR",Name="G.SKILL TridentZ RGB Series 32GB",PictureFileName="Tridentz Rgb",PictureUri="Tridentz Rgb.png",Price=99 },
                new CatalogItem {CatalogTypeId=1,CatalogBrandId=2,AvailableStock=100,Description="G.SKILL Trident Z5 RGB Series 32GB (2 x 16GB) 288-Pin PC RAM DDR5 6000 (PC5 48000) Desktop Memory Model F5-6000J3636F16GX2-TZ5RS",Name="G.SKILL Trident Z5 RGB Series 32GB",PictureFileName="Tridentz Z5 Rgb",PictureUri=" Z5 Rgb.png",Price=170 },
                new CatalogItem {CatalogTypeId=1,CatalogBrandId=1,AvailableStock=100,Description="CORSAIR Vengeance RGB Pro SL 32GB (2 x 16GB) 288-Pin PC RAM DDR4 3200 (PC4 25600) Desktop Memory Model CMH32GX4M2E3200C16",Name="CORSAIR Vengeance RGB Pro SL 32GB",PictureFileName="CORSAIR Vengeance",PictureUri="CORSAIR Vengeance.png",Price=99 },
                new CatalogItem {CatalogTypeId=2,CatalogBrandId=1,AvailableStock=100,Description="CORSAIR RMx Series (2021) RM750x CP-9020199-NA 750 W ATX12V / EPS12V 80 PLUS GOLD Certified Full Modular Power Supply",Name="CORSAIR RMx Series (2021) RM750x",PictureFileName="CORSAIR RM750x",PictureUri="CORSAIR RM750x.png",Price=100 },
                new CatalogItem {CatalogTypeId=2,CatalogBrandId=3,AvailableStock=100,Description="Thermaltake Smart Series 500W SLI/CrossFire Ready Continuous Power ATX 12V V2.3 / EPS 12V 80 PLUS Certified Active PFC Power Supply Haswell Ready PS-SPD-0500NPCWUS-W",Name="Thermaltake Smart Series 500W",PictureFileName="Thermaltake Smart",PictureUri="Thermaltake Smart.png",Price=35 },
                new CatalogItem {CatalogTypeId=2,CatalogBrandId=4,AvailableStock=100,Description="EVGA 700 GD 100-GD-0700-V1 700 W ATX12V / EPS12V 80 PLUS GOLD Certified Non-Modular Active PFC Power Supply",Name="EVGA 700 GD 100-GD-0700-V1",PictureFileName="EVGA 700 GD",PictureUri="EVGA 700 GD.png",Price=110 },
                new CatalogItem {CatalogTypeId=3,CatalogBrandId=5,AvailableStock=100,Description="MSI Ventus GeForce RTX 3060 12GB GDDR6 PCI Express 4.0 Video Card RTX 3060 Ventus 2X 12G OC",Name="MSI Ventus GeForce RTX 3060 12GB",PictureFileName="MSI Ventus",PictureUri="MSI Ventus.png",Price=365 },
                new CatalogItem {CatalogTypeId=3,CatalogBrandId=6,AvailableStock=100,Description="Acer Predator BiFrost Arc A770 16GB GDDR6 PCI Express 4.0 x16 Video Card Predator BiFrost Intel Arc A770 OC",Name="Acer Predator BiFrost Arc A770",PictureFileName="Acer Predator BiFrost",PictureUri="Acer Predator BiFrost.png",Price=399 },
                new CatalogItem {CatalogTypeId=3,CatalogBrandId=7,AvailableStock=100,Description="GIGABYTE EAGLE OC GeForce RTX 4080 16GB GDDR6X PCI Express 4.0 x16 ATX Video Card GV-N4080EAGLE OC-16GD",Name="GIGABYTE EAGLE OC GeForce RTX 4080",PictureFileName="GIGABYTE EAGLE",PictureUri="GIGABYTE EAGLE.png",Price=1279 }
            };
        }

        private void GetCatalogItemsPictures(string contentPath, string picturePath)
        {
            picturePath ??= "pics";

            if (picturePath != null)
            {
                DirectoryInfo directory = new DirectoryInfo(picturePath);
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                string zipFileCatalogItemPictures = Path.Combine(contentPath, "CatalogItems.zip");
                ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
            }
        }
    }
}