// Create express app
var express = require("express")
var app = express()
var md5 = require("md5")
var db = require("./dbhandler")

var bodyParser = require("body-parser");
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

// Server port
var HTTP_PORT = 3232
// Start server
app.listen(HTTP_PORT, () => {
    console.log("Server running on port %PORT%".replace("%PORT%",HTTP_PORT))
});

// Endpoints
app.get("/", (req, res, next) => {
    res.json({"message":"Ok"})
    console.log("enter to web.")
});

// Get data quary.
app.get("/api/:field/:table/:condition", (req, res, next) => {
    var field =  req.params.field, table = req.params.table, condition = req.params.condition;

    if (condition == "null")
        condition = " ";

    var sql = "SELECT " + field + " FROM " + table + " " + condition;
    var params = []

    db.all(sql, params, (err, rows) => {
        if (err) // If there is an error.
        {
            console.log("Error select quary")
            res.status(400).json({"error":err.message});
            return;
        }
        // Else return the user data.
        console.log("Data was return. (select)")
        res.json({
            "message":"success",
            "data":rows
        })
    });
});

// Add new user.
app.post("/api/user/", (req, res, next) => {
    var errors=[]
    if (!req.body.password){errors.push("No password specified");}
    if (errors.length)
    {
        res.status(400).json({"error":errors.join(",")});
        console.log("Error adding user.")
        return;
    }

    var data = {
        name: req.body.name,
        password: md5(req.body.password)
    }

    var sql ='INSERT INTO Users (Name, Password) VALUES (?,?)'
    var params =[data.name, data.password]
    
    db.run(sql, params, function (err, result) {
        if (err)
        {
            res.status(400).json({"error": err.message})
            console.log("Error adding user.")
            return;
        }
        console.log("User was added.")
        res.json({
            "message": "success",
            "data": data,
            "id" : this.lastID
        })
    });
});

// Add new album
app.post("/api/album/", (req, res, next) => {
    var errors=[]
    if (!req.body.user_id){errors.push("No User_id specified");}
    if (!req.body.creation_date){errors.push("No Date specified");}
    if (errors.length)
    {
        res.status(400).json({"error":errors.join(",")});
        console.log("Error adding album.")
        return;
    }

    var data = {
        name: req.body.name,
        user_id: Number(req.body.user_id),
        date: req.body.creation_date
    }

    var sql ='INSERT INTO Albums (Name, User_id, Creation_date) VALUES (?,?,?)'
    var params =[data.name, data.user_id, data.date]
    
    db.run(sql, params, function (err, result) {
        if (err)
        {
            res.status(400).json({"error": err.message})
            console.log("Error adding album.")
            return;
        }
        console.log("New album added.")
        res.json({
            "message": "success",
            "data": data,
            "id" : this.lastID
        })
    });
});

// Add new photo
app.post("/api/picture/", (req, res, next) => {
    var errors=[]
    if (!req.body.album_id){errors.push("No Album_id specified");}
    if (!req.body.creation_date){errors.push("No Date specified");}
    if (!req.body.location){errors.push("No Location specified");}
    if (errors.length)
    {
        res.status(400).json({"error":errors.join(",")});
        console.log("Error adding photo.")
        return;
    }

    var data = {
        name: req.body.name,
        album_id: Number(req.body.album_id),
        date: req.body.creation_date,
        location: req.body.location
    }

    var sql ='INSERT INTO Pictures (Name, Location, Creation_date, Album_id) VALUES (?,?,?,?)'
    var params =[data.name, data.location, data.date, data.album_id]
    
    db.run(sql, params, function (err, result) {
        if (err)
        {
            res.status(400).json({"error": err.message})
            console.log("Error adding photo.")
            return;
        }
        console.log("New photo added.")
        res.json({
            "message": "success",
            "data": data,
            "id" : this.lastID
        })
    });
});

// Add new tag
app.post("/api/tag/", (req, res, next) => {
    var errors=[]
    if (!req.body.picture_id){errors.push("No Picture_id specified");}
    if (!req.body.user_id){errors.push("No User_id specified");}
    if (errors.length)
    {
        res.status(400).json({"error":errors.join(",")});
        console.log("Error adding tag.")
        return;
    }

    var data = {
        user_id: Number(req.body.user_id),
        picture_id: Number(req.body.picture_id)
    }

    var sql ='INSERT INTO Tags (Picture_id, User_id) VALUES (?,?)'
    var params =[data.picture_id, data.user_id]
    
    db.run(sql, params, function (err, result) {
        if (err)
        {
            res.status(400).json({"error": err.message})
            console.log("Error adding tag.")
            return;
        }
        console.log("New tag added.")
        res.json({
            "message": "success",
            "data": data,
            "id" : this.lastID
        })
    });
});

// Update user data
app.patch("/api/user/:id", (req, res, next) => {
    var data = {
        password :md5(req.body.password)}

    db.run(
        `UPDATE Users set
           password = COALESCE(?,password) 
           WHERE id = ?`,
        [data.password, req.params.id],
        function (err, result)
        {
            if (err){
                res.status(400).json({"error": res.message})
                console.log("Error update user.")
                return;
            }
            console.log("User update.")
            res.json({
                message: "success",
                data: data,
                changes: this.changes
            })
    });
});

// Update album data
app.patch("/api/album/:id", (req, res, next) => {
    var data = {name: req.body.name}

    db.run(
        `UPDATE Albums set 
           name = COALESCE(?,name) 
           WHERE User_id = ?`,
        [data.name, req.params.id],
        function (err, result)
        {
            if (err){
                res.status(400).json({"error": res.message})
                console.log("Error update album.")
                return;
            }
            console.log("Album update.")
            res.json({
                message: "success",
                data: data,
                changes: this.changes
            })
    });
});

// Update photo data
app.patch("/api/picture/:id", (req, res, next) => {
    var data = {
        name: req.body.name,
        location: req.body.location}

    db.run(
        `UPDATE Pictures set 
           name = COALESCE(?,name),
           location = COALESCE(?,location)
           WHERE Album_id = ?`,
        [data.name, data.location, req.params.id],
        function (err, result)
        {
            if (err){
                res.status(400).json({"error": res.message})
                console.log("Error update photo.")
                return;
            }
            console.log("Photo update.")
            res.json({
                message: "success",
                data: data,
                changes: this.changes
            })
    });
});

// Delete user by id
app.delete("/api/user/:id", (req, res, next) => {
    var sql = 'DELETE FROM Users WHERE id = ' +req.params.id
    var params = []

    db.run(sql, params, function (err, result) {
        if (err)
        {
            res.status(400).json({"error": err.message})
            console.log("Error delete user.")
            return;
        }
        console.log("Delete user.")
        res.json({"message":"deleted", changes: this.changes});
    });
});

// Delete album by name
app.delete("/api/album/:name", (req, res, next) => {
    var sql = 'DELETE FROM Albums WHERE Name = "' + req.params.name + '"'
    var params = []

    db.run(sql, params, function (err) {
        if (err)
        {
            res.status(400).json({"error": err.message})
            console.log("Error delete album.")
            return;
        }

        console.log("Delete album.")
        res.json({"message":"deleted", changes: this.changes});
    });
});

// Delete photo by album id + user name
app.delete("/api/picture/:album_id/:name", (req, res, next) => {
    var sql = 'DELETE FROM Pictures WHERE Name = "' + req.params.name + '" AND Album_id = ' + req.params.album_id
    var params = []

    db.run(sql, params, function (err) {
        if (err)
        {
            res.status(400).json({"error": err.message})
            console.log("Error delete photo.")
            return;
        }

        console.log("Delete photo.")
        res.json({"message":"deleted", changes: this.changes});
    });
});

// Delete tag by userid + picture id
app.delete("/api/tag/:user_id/:picture_id", (req, res, next) => {
    var sql = 'DELETE FROM Tags WHERE User_id = ' + req.params.user_id + ' AND Picture_id = ' + req.params.picture_id
    var params = []

    db.run(sql, params, function (err) {
        if (err)
        {
            res.status(400).json({"error": err.message})
            console.log("Error delete tag.")
            return;
        }

        console.log("Delete tag.")
        res.json({"message":"deleted", changes: this.changes});
    });
});

// Check if pass is correct
app.post("/api/check/", (req, res, next) => {
    var errors=[]
    if (!req.body.password){errors.push("No password specified");}
    if (errors.length)
    {
        console.log("Error checking user.")
        res.status(400).json({"error":errors.join(",")});
        return;
    }

    var data = {
        name: req.body.username,
        password: md5(req.body.password)
    }

    var sql ='SELECT * FROM Users WHERE Name = ?'
    var params =[data.name]
    
    db.all(sql, params, function (err, result) {
        if (err)
        {
            res.status(400).json({"error": err.message})
            console.log("Error checking user.")
            return;
        }   

        console.log("User checked.")
        if (result.length == 0)
        {
            res.json({
                "message": "success",
                "data": "false"
            });
        }
        else if (result[0].Password == data.password)
        {
            res.json({
                "message": "success",
                "data": "true"
            });
        }
        else
        {
            res.json({
                "message": "success",
                "data": "false"
            });
        }
    });
});

// Default response for any other request
app.use(function(req, res){
    res.status(404);
});