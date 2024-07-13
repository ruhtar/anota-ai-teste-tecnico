﻿using AnotaAi.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AnotaAi.Infraestructure.Repositories;

public interface ICategoryRepository
{
    Task DeleteAsync(string id, CancellationToken cancellationToken);
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task<Category> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task InsertAsync(Category category, CancellationToken cancellationToken);
    Task UpdateAsync(string id, Category category, CancellationToken cancellationToken);
}

public class CategoryRepository : ICategoryRepository
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Category> _categoryCollection;

    public CategoryRepository()
    {
        _client = new MongoClient("mongodb://localhost:27017"); //TODO: alterar
        _database = _client.GetDatabase("catalog");
        _categoryCollection = _database.GetCollection<Category>("categories");
    }

    public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken)
        => await _categoryCollection.Find(Builders<Category>.Filter.Empty).ToListAsync(cancellationToken);

    public async Task InsertAsync(Category category, CancellationToken cancellationToken)
        => await _categoryCollection.InsertOneAsync(category, null, cancellationToken);

    public async Task UpdateAsync(string id, Category category, CancellationToken cancellationToken)
    {
        var filter = Builders<Category>.Filter.Eq(c => c.Id, id);
        var updateBuilder = Builders<Category>.Update
            .Set(c => c.Title, category.Title)
            .Set(c => c.Description, category.Description)
            .Set(c => c.OwnerId, category.OwnerId);

        await _categoryCollection.UpdateOneAsync(filter, updateBuilder, null, cancellationToken);
    }

    public async Task<Category> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<Category>.Filter.Eq("_id", ObjectId.Parse(id));
        return await _categoryCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<Category>.Filter.Eq("_id", ObjectId.Parse(id));
        await _categoryCollection.DeleteOneAsync(filter, cancellationToken);
    }
}
