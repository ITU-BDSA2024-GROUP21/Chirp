using System.Data;
using Microsoft.Data.Sqlite;

namespace Chirp.Infrastructure;

public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;
    
    public CheepService(ICheepRepository cheepRepository) {
        _cheepRepository = cheepRepository;
    }

    public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
    
    public async Task<Cheep> CreateCheep(string username,  string email, string message, string timestamp, int id)
    {
        AuthorDTO newAuthor = new AuthorDTO
        {
            Name = username,
            Email = email,
        };
        var author = await _cheepRepository.ConvertAuthors(newAuthor);
        
        CheepDTO newCheep = new CheepDTO
        {
            Author = username,
            Text = message,
            TimeStamp = timestamp,
            CheepId = id,
            AuthorId = author.AuthorId
            
        };
        var cheep = await _cheepRepository.ConvertCheeps(newCheep, newAuthor);
        return cheep;
    }

    public async Task<List<CheepDTO>> GetCheeps(int page)
    {
        var result = await _cheepRepository.GetCheeps(page);
        var cheeps = DTOConversion(result);
        return cheeps;
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page)
    {
        var result = await _cheepRepository.GetCheepsFromAuthor(author, page);
        var cheeps = DTOConversion(result);
        return cheeps;
    }

    public async Task<Author> GetAuthorByName(string name)
    {
        return await _cheepRepository.GetAuthorByName(name);
    }


    private static List<CheepDTO> DTOConversion(List<Cheep> cheeps)
    {
        var list = new List<CheepDTO>();
        foreach (var cheep in cheeps)
        {
            list.Add(new CheepDTO
            {
                Author = cheep.Author.Name,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString(),
                CheepId = cheep.CheepId,
                AuthorId = cheep.Author.AuthorId
            });
        }
        return list;
    }

    public async Task Follow(int followingAuthorId, int followedAuthorId)
    {
        await _cheepRepository.FollowAuthor(followingAuthorId, followedAuthorId);
    }

    public async Task<List<string>> GetFollowedAuthors(int authorId)
    {
        return await _cheepRepository.GetFollowedAuthorsAsync(authorId);
    }
    
    public async Task<List<CheepDTO>> GetCheepsFromFollowedAuthor(IEnumerable<string> followedAuthors, int authorId)
    {
        var cheep = await _cheepRepository.GetCheepsFromFollowedAuthorsAsync(followedAuthors, authorId);
        var cheeps = DTOConversion(cheep);
        return cheeps;
    }

    public async Task<bool> IsFollowing(int followingAuthorId, int followedAuthorId)
    {
        return await _cheepRepository.IsFollowing(followingAuthorId, followedAuthorId);
    }

    public async Task Unfollow(int followingAuthorId, int followedAuthorId)
    {
        await _cheepRepository.Unfollow(followingAuthorId, followedAuthorId);
    }

    public async Task DeleteAuthorAndCheepsByEmail(string email)
    {
        await _cheepRepository.DeleteAuthorAndCheepsByEmail(email);
    }
    
    public async Task CheckFollowerExistElseCreate(ApplicationUser user)
    {
        
            AuthorDTO newAuthor = new AuthorDTO
            {
                Name = user.UserName,
                Email = user.Email,
            };
            _cheepRepository.ConvertAuthors(newAuthor).Wait();
        
    }
    
    public async Task<Bio> CreateBIO(string username,  string email, string message, int id)
    {
        AuthorDTO newAuthor = new AuthorDTO
        {
            Name = username,
            Email = email,
        };
        var author = await _cheepRepository.ConvertAuthors(newAuthor);

        BioDTO newBio = new BioDTO
        {
            Author = username,
            Text = message,
            BioId = id,
            AuthorId = author.AuthorId
        };
        
        
        var bio = await _cheepRepository.ConvertBio(newBio, newAuthor);
        return bio;
    }
    
    private static BioDTO DTOConversionBio(Bio bio)
    {
        return (new BioDTO
            {
                Author = bio.Author.Name,
                Text = bio.Text,
                BioId = bio.BioId,
                AuthorId = bio.Author.AuthorId
            });
        
    }
    public async Task<BioDTO> GetBio(string author)
    {
        var result = await _cheepRepository.GetBio(author);
        var Bio = DTOConversionBio(result);
        return Bio;
    }
}