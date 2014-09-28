var mongo = require('mongodb');
var express = require('express');
var monk = require('monk');
var db =  monk('localhost:27017/test');
var app = new express();
var collectionName = 'twitches';

// Test function. Returns the number you send it
app.get('/return/:num', function(req, res){
	var id = req.params.num;
	res.send(id);
});

// Get all twitches
app.get('/allTwitches', function(req,res){
	var collection = db.get(collectionName);
	collection.find({},{},function(e,docs){
		res.json(docs);
	})
});

// Get twitches by species
app.get('/twitchBySpecies/:name', function(req,res){
	var collection = db.get(collectionName);
	// Find all items in the collection that contain the queried name
	collection.find({ "twitch.species" : {$regex : ".*" + req.params.name + ".*"} },{limit:20},function(e,docs){
		// Return the results as a JSON object
		res.json(docs);
	})
});

// Get all twitches posted at the given location
app.get('/twitchByLocation/:name', function(req,res){
	var collection = db.get(collectionName);
	// Find all items in the collection that contain the queried name
	collection.find({ "twitch.address.location" : {$regex : ".*" + req.params.name + ".*"} },{limit:20},function(e,docs){
		// Return the results as a JSON object
		res.json(docs);
	})
});

// Get all databases
app.get('/',function(req,res){
  db.driver.admin.listDatabases(function(e,dbs){
      res.json(dbs);
  });
});

// Get all collections
app.get('/collections',function(req,res){
  db.driver.collectionNames(function(e,names){
    res.json(names);
  })
});

// Get 20 items in the collection specified
app.get('/collections/:name',function(req,res){
  var collection = db.get(req.params.name);
  collection.find({},{limit:20},function(e,docs){
	res.json(docs);
  })
});

var server = app.listen(3000, function() {
    console.log('Listening on port %d', server.address().port);
});