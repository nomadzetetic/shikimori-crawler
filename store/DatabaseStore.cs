using Microsoft.EntityFrameworkCore;
using Shikimori.Agent.Models;
using Shikimori.Data;
using Shikimori.Data.Models;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Shikimori.Store
{
    public class DatabaseStore : IDatabaseStore
    {
        private ShikimoriDbContext _dbContext;

        public DatabaseStore(ShikimoriDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Setting> GetSettingAsync(string key)
        {
            var setting = await _dbContext.Settings.FirstOrDefaultAsync(x => x.Key == key);
            return setting;
        }

        public async Task<Setting> SaveOrUpdateSettingAsync(string key, string value)
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

        public async Task<Video> SaveOrUpdateVideoAsync(VideoInfo videoPageInfo)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot);
            try
            {
                var video = await _dbContext.Videos.FirstOrDefaultAsync(x => x.Url == videoPageInfo.Url);

                if (video == null)
                {
                    video = new Video
                    {
                        Url = videoPageInfo.Url,
                        Description = videoPageInfo.Description,
                        Duration = videoPageInfo.Duration,
                        TitleEng = videoPageInfo.Title.Eng,
                        TitleRus = videoPageInfo.Title.Ru,
                        Genres = videoPageInfo.Genres.Select(x => new Genre
                        {
                            Key = x.Eng,
                            Name = x.Ru
                        }).ToList()
                    };
                    await _dbContext.Videos.AddAsync(video);
                }
                else
                {
                    video.Description = videoPageInfo.Description;
                    video.Duration = videoPageInfo.Duration;
                    video.TitleEng = videoPageInfo.Title.Eng;
                    video.TitleRus = videoPageInfo.Title.Ru;
                    video.Genres = videoPageInfo.Genres.Select(x => new Genre
                    {
                        Key = x.Eng,
                        Name = x.Ru
                    }).ToList();
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return video;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
