using MongoDB.Driver;
using camagru.Models;
using Microsoft.Extensions.Options;

namespace camagru.Services;

public class UserService
{
    private readonly IMongoCollection<User> _usersCollection;
    
    public UserService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            bookStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookStoreDatabaseSettings.Value.DatabaseName);
        
        _usersCollection = mongoDatabase.GetCollection<User>(
            bookStoreDatabaseSettings.Value.UsersCollectionName);
    }
    
    public List<User> GetUsers() =>
        _usersCollection.Find(user => true).ToList();
    
    public User GetUser(string id) =>
        _usersCollection.Find<User>(user => user.Id == id).FirstOrDefault();
    
    public User CreateUser(User user)
    {
        _usersCollection.InsertOne(user);
        return user;
    }
}