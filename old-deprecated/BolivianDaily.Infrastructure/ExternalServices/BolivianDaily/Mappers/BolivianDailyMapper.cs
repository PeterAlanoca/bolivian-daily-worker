using BolivianDaily.Domain.Entities;
using BolivianDaily.Infrastructure.ExternalServices.BolivianDaily.Dtos;

namespace BolivianDaily.Infrastructure.ExternalServices.BolivianDaily.Mappers;

public static class BolivianDailyMapper
{
    public static BolivianDailyNewsDto ToBolivianDailyDto(this News news)
    {
        return new BolivianDailyNewsDto
        {
            CategoryId = news.CategoryId,
            SourceId = news.SourceId,
            UserId = 1,
            Title = news.Title,
            Pretitle = news.Pretitle,
            Subtitle = news.Subtitle,
            Enter = news.Enter,
            Body = news.Body,
            Author = news.Author,
            PublicationDate = news.PublicationDate?.ToString("yyyy-MM-dd HH:mm:ss"),
            State = news.State ?? "A",
            Multimedia = news.Multimedia?.Select(m => new BolivianDailyMultimediaDto
            {
                Url = m.Url,
                Type = m.Type
            }).ToList() ?? new List<BolivianDailyMultimediaDto>()
        };
    }
}
