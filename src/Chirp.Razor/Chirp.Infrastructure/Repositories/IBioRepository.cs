namespace Chirp.Infrastructure.Repositories;

public interface IBioRepository
{
    public Task<Bio> ConvertBio(BioDTO newBio, AuthorDTO newAuthor);
    public Task<Bio?> GetBio(string author);
    public Task<bool> AuthorHasBio(string author);
    public Task DeleteBio(Author author);
}