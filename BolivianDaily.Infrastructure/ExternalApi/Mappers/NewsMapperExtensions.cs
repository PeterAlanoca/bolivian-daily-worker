using BolivianDaily.Domain.Entities;
using BolivianDaily.Infrastructure.ExternalApi.Dtos;

namespace BolivianDaily.Infrastructure.ExternalApi.Mappers;

public static class NewsMapperExtensions
{
    public static NewsSubmissionDto ToSubmissionDto(this News newsItem)
    {
        return new NewsSubmissionDto
        {
            CategoryId = newsItem.CategoryId,
            SourceId = newsItem.SourceId,
            UserId = 1, // Constant for now
            Title = newsItem.Title,
            Pretitle = newsItem.Pretitle,
            Subtitle = newsItem.Subtitle,
            Enter = newsItem.Enter,
            Body = newsItem.Body,
            Author = newsItem.Author,
            PublicationDate = newsItem.PublicationDate?.ToString("yyyy-MM-dd HH:mm:ss"),
            State = newsItem.State ?? "A",
            Multimedia = newsItem.Multimedia?.Select(m => m.ToMultimediaDto()).ToList() ?? new List<MultimediaSubmissionDto>()
        };
    }

    private static MultimediaSubmissionDto ToMultimediaDto(this Multimedia multimedia)
    {
        return new MultimediaSubmissionDto
        {
            Url = multimedia.Url,
            Type = multimedia.Type
        };
    }
}
