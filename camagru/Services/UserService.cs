using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MongoDB.Driver;
using camagru.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace camagru.Services;

public class UserService
{
    private readonly IMongoCollection<User> _usersCollection;

    private readonly string key;
    
    public UserService(
        IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings,
        IConfiguration configuration
        )
    {
        var mongoClient = new MongoClient(
            bookStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookStoreDatabaseSettings.Value.DatabaseName);
        
        _usersCollection = mongoDatabase.GetCollection<User>(
            bookStoreDatabaseSettings.Value.UsersCollectionName);
        
        this.key = configuration.GetSection("JwtKey").ToString();
    }
    
    public string Authenticate(string userName, string password)
    {
        var user = _usersCollection.Find<User>(
            user => user.Id == userName && user.Password == password).FirstOrDefault();
        
        if (user == null)
            return null;
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var tokenKey = Encoding.ASCII.GetBytes(key);
        
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id)
            }),
            
            Expires = DateTime.UtcNow.AddHours(1),
            
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }
    
    public List<User> GetUsers() =>
        _usersCollection.Find(user => true).ToList();
    
    public User GetUser(string userName) =>
        _usersCollection.Find<User>(user => user.Id == userName).FirstOrDefault();

    public User CreateUser(User user)
    {
        _usersCollection.InsertOne(user);
        return user;
    }
}
