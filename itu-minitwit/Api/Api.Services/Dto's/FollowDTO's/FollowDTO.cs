namespace Api.Services.Dto_s.FollowDTO_s;

public class FollowDTO
{

    public string? Follow { get; set; }


    public string? Unfollow { get; set; }

    public override string ToString()
    {
       return "FollowDTO{" +
              $"Follow='{Follow}'" +
              $", Unfollow='{Unfollow}'" +
              '}';
    }
}