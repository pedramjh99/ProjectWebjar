using Hangfire;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectWebjar.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectWebjar.Repository
{
    public class CommentRepository 
    {
        private readonly IMongoDatabase db;
        private readonly IMongoCollection<Comment> commentCollection;

        public CommentRepository()
        {
            var client = new MongoClient();
            db = client.GetDatabase("ProjectWebjardb");
            commentCollection = db.GetCollection<Comment>("Comment");
        }

        public void Add(Comment comment)
        {
            commentCollection.InsertOne(comment);
        }

        public List<Comment> GetList()
        {
            var cm =  commentCollection.Find(new BsonDocument()).ToList();
            return cm;
        }

        public List<Comment> GetList(int productId)
        {
            return commentCollection.Find(x => x.ProductId == productId).ToList();     
        }
    }
}
