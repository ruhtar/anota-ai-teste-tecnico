﻿using AnotaAi.Domain.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnotaAi.Domain.Entities
{
    public class Product
    {
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("category")]
        public string CategoryId { get; set; }

        [BsonElement("ownerId")]
        public string OwnerId { get; set; } = null!;

        public Product()
        {

        }

        public Product(ProductCreateDto productDto)
        {
            Title = productDto.Title;
            Description = productDto.Description;
            Price = productDto.Price;
            OwnerId = productDto.OwnerId;

            if (productDto.Category != null)
                CategoryId = new Category(productDto.Category);
        }
    }
}
