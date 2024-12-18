using System.Data;
using Chirp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;

namespace Chirp.Infrastructure.Services;

// This is our nooterService which is used for communication between ChirpCore, ChirpWeb and ChirpInfrastructure.
// So they only have to communicate with the service instead of the respective repositories
public class NooterService : INooterService
{
    private readonly INootRepository _nootRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IBioRepository _bioRepository;
    private readonly IFollowRepository _followRepository;
    
    public NooterService(INootRepository nootRepository, IAuthorRepository authorRepository, IBioRepository bioRepository, IFollowRepository followRepository) {
        _nootRepository = nootRepository;
        _authorRepository = authorRepository;
        _bioRepository = bioRepository;
        _followRepository = followRepository;
    }

    // This is a method for converting unixTimeStamp to a dateTimeString
    public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
    
    // This is to create a cheep/noot
    public async Task<Cheep> CreateCheep(string username,  string email, string message, string timestamp, int id)
    {
        AuthorDTO newAuthor = new AuthorDTO
        {
            Name = username,
            Email = email,
        };
        var author = await _authorRepository.ConvertAuthors(newAuthor);
        
        CheepDTO newCheep = new CheepDTO
        {
            Author = username,
            Text = message,
            TimeStamp = timestamp,
            CheepId = id,
            AuthorId = author.AuthorId
            
        };
        var cheep = await _nootRepository.ConvertNoots(newCheep, newAuthor);
        return cheep;
    }

    // This is for retrieving the cheeps/noots from a given page
    // calls the respective repository
    public async Task<List<CheepDTO>> GetCheeps(int page)
    {
        var result = await _nootRepository.GetNoots(page);
        var cheeps = DTOConversion(result);
        return cheeps;
    }

    // This is for retrieving the cheeps/noots from a given author and page
    // calls the respective repository
    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page)
    {
        var result = await _nootRepository.GetNootsFromAuthor(author, page);
        var cheeps = DTOConversion(result);
        return cheeps;
    }

    // This is for retrieving the cheeps/noots from a given author
    // calls the respective repository
    public async Task<List<Cheep>> GetNootsWithoutPage(string author)
    {
        var result = await _nootRepository.GetNootsWithoutPage(author);
        return result;
    }

    // This is to get a specific author by its a name, calls the respective repository
    public async Task<Author> GetAuthorByName(string name)
    {
        return await _authorRepository.GetAuthorByName(name);
    }

    // This is to delete a noot, calls the respective repository
    public async Task DeleteNoot(int nootId)
    {
        await _nootRepository.DeleteNoot(nootId);
    }
    
    // This is for converting cheeps/noots to a DTO
    private static List<CheepDTO> DTOConversion(List<Cheep> cheeps)
    {
        var list = new List<CheepDTO>();
        foreach (var cheep in cheeps)
        {
            list.Add(new CheepDTO
            {
                Author = cheep.Author.Name,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
                CheepId = cheep.CheepId,
                AuthorId = cheep.Author.AuthorId
            });
        }
        return list;
    }

    // This is for following a specific author
    // calls the respective repository
    public async Task Follow(int followingAuthorId, int followedAuthorId)
    {
        await _followRepository.FollowAuthor(followingAuthorId, followedAuthorId);
    }

    // This is for retrieving which authors a specific author follows
    // calls the respective repository
    public async Task<List<string>> GetFollowedAuthors(int authorId)
    {
        return await _followRepository.GetFollowedAuthors(authorId);
    }
    
    // This is for retrieving all the cheeps/noots from a followed author
    // calls the respective repository
    public async Task<List<CheepDTO>> GetCheepsFromFollowedAuthor(IEnumerable<string> followedAuthors, int authorId)
    {
        var cheep = await _nootRepository.GetNootsFromFollowedAuthors(followedAuthors, authorId);
        var cheeps = DTOConversion(cheep);
        return cheeps;
    }

    // This is to check if a given author is following another given author
    // calls the respective repository
    public async Task<bool> IsFollowing(int followingAuthorId, int followedAuthorId)
    {
        return await _followRepository.IsFollowing(followingAuthorId, followedAuthorId);
    }

    // This is to get an author to unfollow another given author
    public async Task Unfollow(int followingAuthorId, int followedAuthorId)
    {
        await _followRepository.Unfollow(followingAuthorId, followedAuthorId);
    }

    // This is to delete author by its email
    // calls the respective repository
    public async Task DeleteAuthorAndCheepsByEmail(string email)
    {
        await _authorRepository.DeleteAuthorByEmail(email);
    }
    
    // Checks if an author exists, and if it does not exists, then it will be created
    public Task CheckFollowerExistElseCreate(ApplicationUser user)
    {
            AuthorDTO newAuthor = new AuthorDTO
            {
                Name = user?.UserName!,
                Email = user?.Email!,
            };
            _authorRepository.ConvertAuthors(newAuthor).Wait();
            return Task.CompletedTask;
    }
    
    // This is a method for creating a bio
    public async Task<Bio> CreateBio(string username,  string email, string message, int id)
    {
        AuthorDTO newAuthor = new AuthorDTO
        {
            Name = username,
            Email = email,
        };
        var author = await _authorRepository.ConvertAuthors(newAuthor);

        BioDTO newBio = new BioDTO
        {
            Author = username,
            Text = message,
            BioId = id,
            AuthorId = author.AuthorId
        };
        
        
        var bio = await _bioRepository.ConvertBio(newBio, newAuthor);
        return bio;
    }
    
    // This is a method for convert the bio to a BioDTO
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
    
    // This is for retrieving the bio, calls respective repository
    public async Task<BioDTO> GetBio(string author)
    {
        var result = await _bioRepository.GetBio(author);
        var bio = DTOConversionBio(result!);
        return bio;
    }

    //Checks if the author has a bio, calls respective repository
    public async Task<bool> AuthorHasBio(string author)
    {
        var result = await _bioRepository.AuthorHasBio(author);
        return result;
    }

    // This is for deleting a bio, calls respective repository
    public async Task DeleteBio(Author author)
    {
        await _bioRepository.DeleteBio(author);
    }

}