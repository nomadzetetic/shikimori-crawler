using Microsoft.EntityFrameworkCore;
using Shikimori.Agent.Models;
using Shikimori.Data;
using Shikimori.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Shikimori.Store
{
    public class DatabaseStore : IDatabaseStore
    {
        private const string NEXT_PAGE_URL_KEY = "NEXT_PAGE_URL";
        private const string NEXT_PAGE_URL_DEFAULT_VALUE = "https://shikimori.one/animes";

        private readonly ShikimoriDbContext _dbContext;

        public DatabaseStore(ShikimoriDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetNextPageUrlAsync()
        {
            var setting = await _dbContext.Settings.FirstOrDefaultAsync(x => x.Key == NEXT_PAGE_URL_KEY);
            return string.IsNullOrWhiteSpace(setting?.Value) ? NEXT_PAGE_URL_DEFAULT_VALUE : setting.Value;
        }

        public Task<Setting> SaveOrUpdateNextPageUrlAsync(string value) => SaveOrUpdateSettingAsync(NEXT_PAGE_URL_KEY, value);

        private async Task<Setting> SaveOrUpdateSettingAsync(string key, string value)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot);
            try
            {
                var setting = await _dbContext.Settings.FirstOrDefaultAsync(x => x.Key == key);

                if (setting == null)
                {
                    setting = new Setting
                    {
                        Key = key,
                        Value = value
                    };
                    await _dbContext.Settings.AddAsync(setting);
                }
                else
                {
                    setting.Value = value;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return setting;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SaveOrUpdateVideosAsync(List<VideoInfo> videosInfo)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot);
            try
            {
                foreach (var videoInfo in videosInfo)
                {
                    var video = await _dbContext.Videos.FirstOrDefaultAsync(x => x.Url == videoInfo.Url);

                    if (video == null)
                    {
                        video = new Video
                        {
                            Url = videoInfo.Url,
                            Description = videoInfo.Description,
                            Duration = videoInfo.Duration,
                            TitleEng = videoInfo.Title.Eng,
                            TitleRus = videoInfo.Title.Ru,
                            Genres = videoInfo.Genres,
                            ImageUrl = videoInfo.ImageUrl
                        };

                        await _dbContext.Videos.AddAsync(video);
                    }
                    else
                    {
                        video.Description = videoInfo.Description;
                        video.Duration = videoInfo.Duration;
                        video.TitleEng = videoInfo.Title.Eng;
                        video.TitleRus = videoInfo.Title.Ru;
                        video.Genres = videoInfo.Genres;
                        video.ImageUrl = videoInfo.ImageUrl;
                    }

                    await _dbContext.SaveChangesAsync();
                }
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
