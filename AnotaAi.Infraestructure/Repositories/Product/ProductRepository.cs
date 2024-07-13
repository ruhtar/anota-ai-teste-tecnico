﻿using AnotaAi.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AnotaAi.Infraestructure.Repositories;

public interface IProductRepository
{
    Task DeleteAsync(string id, CancellationToken cancellationToken);
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);
    Task<Product> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task InsertAsync(Product product, CancellationToken cancellationToken);
    Task UpdateAsync(string id, Product product, CancellationToken cancellationToken);
}

public class ProductRepository : IProductRepository
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Product> _productsCollection;

    public ProductRepository(IConfiguration configuration)
    {
        _client = new MongoClient(configuration.GetConnectionString("Mongo"));
        _database = _client.GetDatabase("catalog");
        _productsCollection = _database.GetCollection<Product>("products");
    }


    public async Task InsertAsync(Product product, CancellationToken cancellationToken)
    {
        await _productsCollection.InsertOneAsync(product, null, cancellationToken);
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _productsCollection.Find(Builders<Product>.Filter.Empty).ToListAsync(cancellationToken);
    }

    public async Task<Product> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq("_id", ObjectId.Parse(id));
        return await _productsCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(string id, Product product, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq("_id", ObjectId.Parse(id));

        var updateBuilder = Builders<Product>.Update;
        var updateDefinitions = new List<UpdateDefinition<Product>>();

        if (!string.IsNullOrEmpty(product.Description))
            updateDefinitions.Add(updateBuilder.Set(p => p.Description, product.Description));

        if (!string.IsNullOrEmpty(product.Title))
            updateDefinitions.Add(updateBuilder.Set(p => p.Title, product.Title));

        if (product.Price != null)
            updateDefinitions.Add(updateBuilder.Set(p => p.Price, product.Price));

        if (!string.IsNullOrEmpty(product.CategoryId))
            updateDefinitions.Add(updateBuilder.Set(p => p.CategoryId, product.CategoryId));

        if (!string.IsNullOrEmpty(product.OwnerId))
            updateDefinitions.Add(updateBuilder.Set(p => p.OwnerId, product.OwnerId));

        var update = updateBuilder.Combine(updateDefinitions);

        await _productsCollection.UpdateOneAsync(filter, update, null, cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq("_id", ObjectId.Parse(id));
        await _productsCollection.DeleteOneAsync(filter, cancellationToken);
    }
}
