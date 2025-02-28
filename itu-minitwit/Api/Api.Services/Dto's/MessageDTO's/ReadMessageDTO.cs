using System.ComponentModel.DataAnnotations;

namespace Api.Services.Dto_s.MessageDTO_s;

public class ReadMessageDTO
{
    public int MessageId { get; set; }
    public int AuthorId { get; set; }
    public string Content { get; set; }
    public int? PubDate { get; set; }
    public int? Flagged { get; set; }
}